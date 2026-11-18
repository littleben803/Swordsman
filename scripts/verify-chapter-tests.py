#!/usr/bin/env python3
"""Per-chapter, process-isolated chapter start/save checks. Standard library only."""
import argparse
import hashlib
import shutil
import json
from pathlib import Path
import subprocess
import sys
import time
from concurrent.futures import ThreadPoolExecutor, as_completed


def stages(value):
    result = []
    for part in value.split(','):
        if '-' in part:
            first, last = map(int, part.split('-'))
            result.extend(range(first, last + 1))
        else:
            result.append(int(part))
    if not result or any(n < 0 or n > 35 for n in result):
        raise argparse.ArgumentTypeError('Stage indices must be 0 through 35.')
    return list(dict.fromkeys(result))


def export_fixtures(root, args):
    catalog_path = root / 'Assets/ChapterTests/presets.json'
    built_catalog = args.app.resolve() / 'Contents/Resources/Data/StreamingAssets/SwordsmanChapterTests/presets.json'
    if catalog_path.read_bytes() != built_catalog.read_bytes():
        raise RuntimeError('Source presets differ from the checked app. Rebuild/recheck before exporting.')
    catalog = json.loads(catalog_path.read_text())
    verified = []
    for stage in args.stages:
        report = json.loads((args.output.resolve() / ('stage%02d.json' % stage)).read_text())
        required = ['openingReachedPlayerTurn', 'checkpointValidated', 'battleReloadReady', 'stateRoundTrip', 'ordinarySaveRoundTrip', 'pathIsolated']
        known_camp34 = False
        if stage == 34 and args.allow_known_camp34 and report.get('runtimeErrors') == ['ArgumentException: The Object you want to instantiate is null.']:
            log = (root / 'Logs' / ('chapter-tests-%s-stage34.log' % args.output.name)).read_text()
            known_camp34 = 'at GameCampUI.show ' in log and 'at GameSceneCamp.Start ' in log and not (root / 'Assets/Resources/prefab/camp/image/camp34.prefab').exists()
        effective_errors = [] if known_camp34 else report.get('runtimeErrors')
        ordinary_ok = report.get('ordinarySaveRoundTrip') or report.get('ordinarySaveBlocker', '').startswith('missing-original-camp-prefab:')
        if report.get('error') or effective_errors or not ordinary_ok or not all(report.get(k) for k in required if k != 'ordinarySaveRoundTrip'):
            raise RuntimeError('Stage %d has not passed original-opening and save checks.' % stage)
        preset = catalog['entries'][stage]
        profile = Path('stage%02d' % stage) / ('level%02d' % preset['suggestedLevel'])
        source = args.population_root / profile
        metadata = json.loads((source / 'BattleStart.json').read_text())
        binary = (source / 'BattleStart.dat').read_bytes()
        if metadata['schemaVersion'] != 2 or metadata['stage'] != stage or metadata['level'] != preset['suggestedLevel'] or len(binary) != 102400 or int.from_bytes(binary[:4], 'little') != stage or hashlib.sha256(binary).hexdigest() != metadata['binaryHash']:
            raise RuntimeError('Stage %d frozen checkpoint failed integrity checks.' % stage)
        verified.append((profile, source, metadata))
    # Validate the whole selection before writing any fixture.
    for profile, source, metadata in verified:
        target = root / 'Assets/ChapterTests/Checkpoints' / profile
        target.mkdir(parents=True, exist_ok=True)
        for name in ['BattleStart.dat', 'BattleStart.json']:
            shutil.copy2(source / name, target / name)
    print('Exported %d verified frozen checkpoints; Unity will generate missing .meta files.' % len(verified), flush=True)
    return 0


def main():
    root = Path(__file__).resolve().parents[1]
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--app', type=Path, default=root / 'Builds/macOS/Swordsman-ChapterTests-AllChapters.app')
    parser.add_argument('--stages', type=stages, default=stages('0-35'))
    parser.add_argument('--output', type=Path, default=root / 'Builds/ChapterTests/reports/creation')
    parser.add_argument('--jobs', type=int, choices=range(1, 5), default=1, help='Independent player processes; Unity builds remain serial.')
    parser.add_argument('--allow-known-camp34', action='store_true', help='Export only the verified battle snapshot from the separately documented original camp34 load failure; never marks camp load as passed.')
    parser.add_argument('--export-only', action='store_true', help='Validate existing creation reports and archive their frozen snapshots for packaging; does not run players.')
    parser.add_argument('--population-root', type=Path, default=Path.home() / 'Library/Application Support/DefaultCompany/Swordsman/ChapterTests/v2/Population')
    parser.add_argument('--package', action='store_true', help='Read bundled fixtures in a separate PackageValidation directory (implies --reuse).')
    parser.add_argument('--reuse', action='store_true', help='Check persisted/packaged snapshots instead of replaying openings.')
    args = parser.parse_args()
    exe = args.app.resolve() / 'Contents/MacOS/Swordsman'
    if not exe.is_file():
        parser.error('Build the chapter test app before running checks.')
    if args.export_only:
        return export_fixtures(root, args)
    output = args.output.resolve()
    output.mkdir(parents=True, exist_ok=True)
    logs = root / 'Logs'
    logs.mkdir(exist_ok=True)
    results = []
    for stage in args.stages:
        (output / ('stage%02d.json' % stage)).unlink(missing_ok=True)
    (output / 'summary.json').write_text(json.dumps({'results': [], 'complete': False, 'requestedStages': args.stages}, indent=2) + '\n')
    def verify(stage):
        report = output / ('stage%02d.json' % stage)
        report.unlink(missing_ok=True)
        command = [str(exe), '-batchmode', '-swordsman-chapter-tests-check',
                   '-swordsman-chapter-tests-stage-check', str(stage),
                   '-swordsman-chapter-tests-report', str(report),
                   '-logFile', str(logs / ('chapter-tests-%s-stage%02d.log' % (output.name, stage)))]
        if args.package:
            command.append('-swordsman-chapter-tests-package-check')
        if args.reuse or args.package:
            command.append('-swordsman-chapter-tests-reuse')
        started = time.monotonic()
        with subprocess.Popen(command, cwd=root, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL) as process:
            try:
                code = process.wait(timeout=300)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
                code = -1
        data = json.loads(report.read_text()) if report.exists() else {'stage': stage, 'error': 'Process produced no report.'}
        data['processExitCode'] = code
        passed = code == 0 and not data.get('error') and not data.get('runtimeErrors')
        return {'stage': stage, 'passed': passed, 'seconds': round(time.monotonic() - started, 2), 'report': str(report), 'error': data.get('error') or '; '.join(data.get('runtimeErrors', [])), 'ordinarySaveBlocker': data.get('ordinarySaveBlocker', '')}
    with ThreadPoolExecutor(max_workers=args.jobs) as pool:
        for future in as_completed([pool.submit(verify, stage) for stage in args.stages]):
            result = future.result()
            results.append(result)
            results.sort(key=lambda r: r['stage'])
            (output / 'summary.json').write_text(json.dumps({'results': results, 'complete': len(results) == len(args.stages), 'requestedStages': args.stages}, ensure_ascii=False, indent=2) + '\n')
            print('stage=%02d %s elapsed=%.1fs %s' % (result['stage'], 'PASS' if result['passed'] else 'FAIL', result['seconds'], result['error'] or result.get('ordinarySaveBlocker', '')), flush=True)
    return 0 if all(r['passed'] for r in results) else 1


if __name__ == '__main__':
    sys.exit(main())
