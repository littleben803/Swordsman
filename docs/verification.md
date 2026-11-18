# 验证指南

## 2026-11-18 新章剧情开场更新版v2交付

当前用户源 `/Users/ideabox/Desktop/11月18日.mp4` 原样覆盖Movie_00，SHA256 `6c8dba3d0dd8a54ef162c3a8620071a5d5764cee7b51997aaec302bc32099edb`，72,528,644字节，1280×800、30fps、2294帧。视频76.466667秒，AAC双声道44.1kHz音轨/容器76.532971秒，未转码或改时长。ffmpeg完整音视频解码通过；包内VideoClip元数据核对及resources.resource引用段SHA与用户源完全一致，原.meta/GUID保持。

通过唯一 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 更新唯一AllChapters应用，Succeeded、0错误/0警告，1031.406秒；预先刷新唯一包内的派生HD Bundle为既有验证UnityFS源，避免上一轮增量留存SWLZF001问题。本轮共享、Splash、HC、LZFSE与codesign deep/strict全部通过，没有重建HD源、清理缓存或修改构建脚本。Player静态冒烟1819资源/472探针/2005图集探针通过；原生1280×800窗口从初章初始播放新版，核看山峦镜头，等待自然结束后确认第零幕洞窟剧情及应靈華对话，测试进程正常退出，视频异常扫描未见错误。未重复四场景检查，因本轮源码、场景与配置均未改变；不将上一轮检查写成本轮实时结果。主观音效及最终观感待用户确认。

493份输入保护仅Movie_00视频改变，运行/Editor代码、场景、工程配置、原.meta和其他视频SHA不变。Git状态与本轮开始完全一致，保留已有123项变更，无异常新增。仅更新本文、静态MVP和唯一清单；新增本地日志/源视频回退副本/核对记录于 `Logs/movie00-opening-v2-20261118/`。应用磁盘占用4,292,396 KiB，无新增应用、依赖、分支、提交或推送。


## 2026-11-18 新章剧情开场视频交付

用户成片原样替换 `Assets/Resources/movie/Movie_00.mp4`，SHA256 `c51d1ed23cac048aea9c376f80d750e3369aea4f4a2580a128170e457d40dd6e`，1280×800、30fps、2297帧、76.566667秒，H.264/AAC。完整ffmpeg解码无错误，原.meta/GUID保持。运行时仅将Movie_00加入既有1280×800输出通道；Title、其他视频、场景、Prefab、ProjectSettings/Packages、剧情/存档未改。

唯一构建入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，最终日志 `Logs/movie00-opening-20261118/build-retry.log`：Succeeded、0错误/0警告，321.068秒。首轮Player打包后遇到旧包内SWLZF001增量留存，导致共享后处理拒绝格式；从原始验证UnityFS源刷新该失败包条目后，按同一入口重跑，通过纹理共享、Splash、HC、LZFSE及签名验证，未更改构建脚本/压缩策略，无临时应用副本。首轮错误及恢复SHA留档。

最终包内Movie_00的resources.resource字节段与源视频SHA完全一致，VideoClip为1280×800/30fps/2297帧；codesign deep/strict通过。静态冒烟1819项、472探针、2005图集探针通过；四场景Missing Script=0。原生窗口以1280×800运行，主菜单初章初始可播放新片，实际核看叙事/骑兵/工作室画面；自然结束后接入第零幕洞窟战斗剧情和應奉仁对话，验证进程正常退出。未做全剧情通关或主观音效验收。应用 `du -sk`为4,292,396 KiB。

493份输入保护复核仅视频和GameMovieManager一处条件变化；全部场景、工程配置、原.meta及其他视频保持。新增本地证据在 `Logs/movie00-opening-20261118/`，更新本文及静态MVP/唯一清单，无新增产品源码、依赖、分支、提交或推送。Git既有123项状态保持，无异常新增。用户最终视觉/音效确认待完成。


## 2026-11-17 设计师v2片头与末帧交付

唯一Development入口重建`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，Succeeded、0错误/0警告，耗时1352.030秒；codesign deep/strict通过。正式`Title.mp4` SHA256为`95b353fc4aca77c9d32edb58e755d49e64bac2337b7efaf01cc3eea7b9a24fd8`。v2静态图无损压缩29.64%，Title_t0有效区域全不透明，Title_t1及图集其他区域逐RGBA不变。实际Player启动报告`passed=true`，静态资源冒烟通过1819项资源、472探针、2005图集探针。证据在`Logs/title-user-final-v2-20261117/`，用户实机视觉与音效待确认。

## 2026-11-16 标题背景全不透明与实黑字标交付

用户美术GO后，Title批准图集切换至 `Builds/HD4X/static-mvp/approved/AtlasVersions/title-opacity-20261116-v1/Texture2D/Title.png`，最终SHA256 `bf2c4946ba2e8fe1dd52f556907e965ba48bce88cedf3bd28201237d51b6cb9f`。归档、菜单底板及背景其他RGB保护检查通过。先前候选的AppKit PNG输出带有gAMA=2.19998，首次构建提取纹理时发现颜色被Unity转换；已仅规范色彩元数据为与原图相同的sRGB，IDAT和RGBA像素保持，重新构建并核对最终应用，不交付中间偏色包。

唯一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新唯一AllChapters应用。最终Development构建Succeeded，0错误/0警告，524.043秒；HD缓存1819项仅Title重导入1项，其余1818项复用。最终两个容器无损HC检查通过，`codesign --verify --deep --strict`通过。

通过现有容器解析工具提取最终包实际Title原始RGBA，再与批准稿对比：背景有效矩形左上坐标 `[2,1,1286,801]` 全部像素相等、差异0、Alpha范围255..255，黑字笔画保持纯黑。图集有效背景外的透明留边会被既有Unity Alpha扩色处理，未把整个图集文件字节相等作为结论。`-swordsman-static-mvp-check`通过1819资源、472探针、2005图集探针；标题过渡检查完成3轮、984帧、errors=0，截图已检查；现有键盘检查50项通过、errors=0。62份场景/Packages/ProjectSettings保护文件SHA保持。Git发现并行任务新增LZFSE验证源码/meta及文档，本轮未触碰，不将其当作本轮新增或回退。

证据目录 `Logs/title-opacity-20261116/`：`build-final.log`、`player-final-title.check.json`、`runtime/report.json`、`capture/report.json`；归档记录 `Builds/HD4X/static-mvp/work/title-opacity-repair-20261116/archive-integration.json`。用户实机视觉及完整剧情验收仍待确认；本轮未改变玩法、存档格式、场景或工程配置。

## 2026-11-16 无损 LZ4HC 压缩与唯一应用交付

本阶段完成。仅修改容器压缩与构建后处理串联，48张混合图集、原版角色动画/播放表及全部源美术保持。唯一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，未生成其他应用、历史整包副本或新增第三方依赖；没有有损压缩。

Player使用 `BuildOptions.CompressWithLz4HC`；先执行现有共享存储、启动后处理，再统一处理它们改写的快速LZ4块并重新ad-hoc签名。Unity 2021.3.16f1c1的 `AssetBundle.RecompressAssetBundleAsync(..., BuildCompression.LZ4)` 实测拒绝HC（探针未替换应用），因此使用同版本Editor自身导出的 `LZ4_compress_HC`，通过有界stdio协议为新增 `scripts/recompress_unityfs.py` 提供编码。原HC块原样复制；每个改写块校验round-trip，最终校验完整解压流SHA256、字节数、节点目录/偏移/长度及文件哈希后才替换容器，最后重签并deep/strict验证。该原生符号依赖当前锁定Editor，未来升级须重新验证；正式Player没有新增压缩库或运行依赖。共享/启动脚本的处理逻辑未改，仅补充最终HC阶段说明。

最终普通文件逻辑字节合计 **4,702,414,743字节（4.702415 GB / 4.379465 GiB）**；上一包4,920,750,428字节，减少 **218,335,685字节（218.336 MB，4.4370%）**。主容器1,118,391,295→902,144,743字节；HD容器3,302,880,225→3,300,791,092字节。共享存储仍消除1,086,230,720字节重复原始像素，不将其再次计入本轮收益。

最终Development构建Succeeded，0错误/0构建警告，高清缓存1819项复用、重导入0。构建入口计时925.549秒，含Unity启动/退出的命令墙钟934.005秒（15分34秒）；二者均包含共享/启动后处理、最终HC校验和签名。最终两个容器HC处理及校验合计67.283秒。构建前验证157.294秒另计；前置兼容性探针不计入构建耗时。主要耗时来自Unity的HC编码，运行栈采样已确认，不把它误判为等待锁。

重压前后主容器4,078,919,764字节、HD容器9,498,648,032字节解压数据完全一致，合计13,577,567,796字节；19个节点目录/偏移/长度一致。跨重建另行核对：主容器仅BuildSettings对象内16字节构建元数据变化，用上一包该节点替换后整个解压流SHA与旧包探针一致；HD全部解压数据与上一包一致。9330张原版纹理/91641个Sprite、1819张HD纹理/2329个Sprite数量保持，全部纹理像素、尺寸、格式和采样设置保持。压缩前后的完整字节一致性与跨构建元数据差异分开记录。

实时验证：`SwordsmanHd4xBuild.ValidateAfterRetirement`完成19项必需检查、1971条描述/3804绑定、Editor与Development Player脚本编译、四场景Missing Script=0；唯一构建入口通过。最终包9组基础检查、`scripts/verify-chapter-tests.py --package --jobs 3 --output Logs/lz4hc-20261116/package-all36`的36幕起点/隔离存读档、5～35幕31组场景资源检查、回合横幅/对话箭头/小地图/攻击信息检查全部通过。本轮没有退出异常；原版Camp34缺失的既有限制保持，战斗起点通过不等同于普通营地读取通过。

5张1280×800真实GPU设置/键盘截图与上一包逐像素一致（差异0）。原生窗口实际观察标题、F9章节台、第21幕资源查看/镜头移动/返回选幕及番外篇，高清地图和原版人物显示正常，退役HD标记未出现；测试应用正常退出。未将batch黑图作为画质证据，未宣称全剧情通关或代替用户美术验收。

195053份受保护文件中195013份哈希不变，预期仅3份已有源码/脚本及37份Automation/PackageValidation隔离测试档改变，无异常变化/缺失；原生退出后另外复核258份非测试外部文件/存档一致。场景/Prefab、工程配置/Packages、原版动画与播放表、48混合图集及meta、HD源Bundle/缓存保持。新增重压Python源码1份，修改构建入口及两个后处理脚本说明，同步本验证记录；其他Git既有修改保留，无提交/推送。仓库级diff检查仍有既有场景YAML尾随空白，本阶段这些场景文件哈希不变，未顺手格式化。

证据入口：`Logs/lz4hc-20261116/delivery.json`；构建与计时 `build.log`、`build-timing.json`，一致性 `archive-verification.json`，截图 `screenshot-comparison.json`，保护 `protection-result.json`，原生观察 `native-ui.json`。HC逐容器报告位于 `Logs/Lz4HC/builds/20261116T1150063635840Z/`。本阶段停止，不进入混合图集拆分或其他资源清理。

## 2026-11-16 已迁移OLD资源清理与唯一应用交付

用户明确授权清理611张已完整高清替换旧图和对应无引用Sprite/meta。先复核外部硬引用0，再经Unity AssetDatabase删除611 PNG、1971 Sprite.asset、2582 meta，共5164文件，源文件合计426861041字节。删除集合与冻结来源记录逐项相同；未删除空目录/目录meta、48张混合图集、原版角色/法术/招式、非本批资源或原始生成输入/批准稿。清单 `Logs/hd-cleanup-20261116/delete-files.json`；611/48范围仍以机器清单 `referenceMigration` 为准。

旧像素删除前冻结 `docs/hd-retired-source-geometry.json`（611纹理来源哈希、meta/依赖哈希与原Sprite的Rect/Pivot/PPU/Border/路径），SHA由Editor源码锁定，内容不含旧像素。部分旧图集名与Sprite前缀不同，按1971个审计Sprite的真实纹理归集，未用名称搜索推断完整范围。构建器以同字段几何记录取代旧Sprite对象：删除前验证全部1819项输入指纹与旧流程相同（漂移0），所有批准图片/现有导入器/Bundle均验证一致，只重登记新代码配方，重导入0、高清Bundle不变。删除后默认构建流程仍 `mode=reused assets=1819 imported=0 unchanged=1819 removed=0`；不依赖锁定复用参数才能构建，也不要求恢复旧PNG。48张混合图集继续用原有活动资源路径。

必要代码范围：构建器/缓存、引用迁移验证器、必需资源验证器与Development场景检查5份既有C#；新增SourceGeometry和Retirement两份Editor工具及.meta、上述冻结JSON。旧编辑器探针改用不含像素的临时几何探针，运行场景检查改读高清路径和冻结约束；不把已删除资源当运行依赖。Runtime引用ID/别名仍是稳定字符串，保留不需要旧meta文件存在。未修改正式玩法、剧情、数值、存档格式、场景/Prefab、ProjectSettings/Packages、Shader/插件或有损纹理设置。

命令入口：`SwordsmanHd4xBuild.ExportRetiredSourceGeometry`（仅清理前）、`RebaseRetiredGeometryCache`（仅清理前）、`DeleteRetiredSourcesAndValidate`、`ValidateAfterRetirement`。清理后19项必需资源检查、1971条冻结几何/高清描述与3804绑定校验通过；Editor与Development Player脚本编译通过，四场景Missing Script=0。通过唯一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新唯一 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，没有临时或历史应用副本。最终Development Succeeded、0错误/9条既有警告；既有无损LZ4、共享存储、启动后处理与ad-hoc签名流程保持，codesign deep/strict和后处理verify-only通过。

应用普通文件逻辑字节合计 **4,920,750,428字节（4.920750 GB / 4.582806 GiB）**；上一包5,122,673,502字节，减少 **201,923,074字节（201.923 MB，3.9418%）**。本次主资源Texture2D 9794→9330（464个），Sprite 92569→91641（928个，含Resources自动导入的Sprite）；另外147张完整旧图集及1507个旧Sprite已在上一轮从包中剔除，故本轮不重复计算。HD仍1819纹理/2329 Sprite，安装Bundle SHA与上一包完全相同；原生保留纹理逐项尺寸/格式/像素字节无变化。不是用构建summary.size或源PNG磁盘大小代替实际应用大小。

最终包9组基础运行、36幕包内起点/隔离存读档、第5～35幕31组地图/标题/人物图/原版单位检查、回合横幅/对话箭头/小地图/攻击信息回归均完成。并发第24幕在检查已输出passed后，退出阶段出现一次-11原生异常，日志含“Thread may have been prematurely finalized”；单进程3次重跑均正常。原因未确认，不将其隐去或声称已修复；首轮日志与重跑结果见 `all-scenes/stage24.initial-crash.log`、`stage24-exit-reruns.json`。原Camp34缺失限制仍保留，36幕战斗起点通过不代表普通Camp34营地读取通过。

5张1280×800设置/键盘（100%/150%）截图与上一包完全一致，差异像素0。原生窗口核看标题/章节入口、F9打开测试台、第21幕资源查看和镜头移动、返回标题及重新打开选幕，高清画面与原版人物正常，HD标记保持移除。实际观察为本次CUA会话和 `native-ui.log`；未使用被启动遮罩覆盖或macOS batch黑图作为画质证据。不是全剧情自然通关或用户美术确认。

保护复核200214文件：5164项预期删除、5份既有代码/1份缓存指纹及37份隔离测试档预期变化，其余195007哈希不变，无意外缺失/变动；48混合图集及其全部Sprite/meta、高清源Bundle/缓存像素/meta、正式存档保持。Git既有修改保留，仅新增2个Editor源码/.meta及冻结JSON，无异常新增、无提交推送；仍遵守大资源本地不入Git，不改.gitignore。完整交付 `Logs/hd-cleanup-20261116/delivery.json`，原始与批准输入继续按MVP保留，本阶段未扩大清理范围。

## 2026-11-16 高清引用迁移唯一应用交付

本阶段完成。用户批准重建唯一应用并验证运行、章节测试台和画质；611张旧资源保留，48张混合图集不动，不做有损压缩。通过唯一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，未创建其他应用或整包备份。复用锁定SHA的原HD Bundle，保留既有LZ4、像素存储共享和启动后处理/签名流程。最终Development构建Succeeded，0错误/9条既有警告，以 `Logs/hd-reference-player-20261116/build-delivery.log` 为准。

最终应用普通文件逻辑字节合计 **5,122,673,502字节（5.122674 GB / 4.770861 GiB）**；上一共享存储包5,151,116,882字节，减少 **28,443,380字节（28.443 MB，0.5522%）**。本地旧文件未删；解除依赖后，Unity自动从主资源文件中剔除147张完整旧图集及1507个旧Sprite。464张Resources旧贴图继续按原自动打包规则保留。原生资源Texture2D 9941→9794、Sprite 94076→92569；HD Bundle仍1819 Texture2D/2329 Sprite，安装派生Bundle SHA仍 `ea82aa4054acf90641c84779e12bc594c594ec39d8b6daefb00fb9991bcc806e`。保留原生纹理逐项尺寸/格式/像素SHA无变化；只移除了上述147张完整替换图集。不是用Unity后处理前的summary.size或DMG下载大小代替实际.app统计。

运行验证发现并修正29条描述表Sprite名称大小写：优先沿用原序列化Sprite名称，而非PNG文件名，数据其余字段完全不变。Editor验证器同步对原.asset名称进行检查。扩展场景检查的第31、34幕原先未加载后续剧情图层就检查其地图，已仅补充Development检查器的showLayer/unShowLayer，不改关卡或剧情。必要修正共3份既有文件，新增源码0、删除资产0；改前副本、差异和首轮失败证据保留于本轮Logs，不备份整个应用。

最终包重新验证：9组基础运行检查（键盘、472资源探针/2005图集切片探针、测试台、对话、难度、第21幕/番外篇/RPG、固定地形）；36幕包内战斗起点与隔离存读档；第5～35幕31组实际地图/标题/人物图/原版单位检查，均通过。补充回合横幅30帧/54切片、对话箭头14帧、小地图世界尺寸及攻击信息检查通过。四场景/1971描述/3804绑定与Player脚本编译在名称修正后复核通过。第34幕仍有原版Camp34缺失的已知限制，战斗起点检查通过不代表普通营地读取通过。

5张1280×800设置/键盘（含100%/150%）截图与上一包对比，只有退役HD标记/F8区域内13316像素改变，区域外差异0。原生窗口实际核看标题与章节入口、F9打开测试台、第21幕资源查看及镜头移动、F9返回选幕、番外篇资源查看；高清地图与原版人物显示正常，HD标记消失，测试进程正常退出。快速非batch场景截图可能被启动遮罩盖住，macOS batch截图为黑图，均明确排除画质证据；相关文件归入 `invalid-batch-captures/`，不将其描述为地图截图。实际原生观察记录于 `native-ui.json`、本次CUA会话及 `native-ui.log`。受控检查与资源查看不替代全剧情自然通关/用户美术验收。

签名 `codesign --verify --deep --strict` 和启动后处理 `--verify-only` 通过。200214份保护文件中200174份哈希一致，预期仅3份源码/描述文件及37份Automation/PackageValidation隔离测试档发生变化，无缺失、无异常变化；全部旧PNG/Sprite/.meta、48张混合图集、正式存档、HD源Bundle/导入缓存、场景/Prefab、玩法、ProjectSettings/Packages与共享脚本保持。Git原有修改保留，无异常新增、无提交或推送。完整报告 `Logs/hd-reference-player-20261116/delivery.json`；数据见 `app-after.json`、`texture-comparison.json`、`screenshot-comparison.json`、`protection-result.json`、`package-all36/summary.json`、`all-scenes/summary.json`、`player/ui-regression-summary.json`。本阶段停止，未进入旧资源清理。

## 2026-11-16 高清引用迁移第2阶段（未删旧资源，未重建应用）

用户批准迁移611张已完整高清替换旧图的引用。核对结果为464张独立贴图、147张完整图集，48张混合图集排除。611/48精确路径仅登记于 `static-art-mvp-resources.json.referenceMigration`。本节为最新源码与序列化资产状态；第1阶段及下文记录保留当时含义。

新增 `Assets/Resources/SwordsmanHD4X/References.json`，1971条描述包含资源ID、路径别名和几何约束，无Texture2D/Sprite硬引用、无复制高清像素。新增 `SwordsmanHd4xReferences`、`SwordsmanHd4xReferenceBindings`、`SwordsmanHd4xResources` 三份运行时脚本；按ID从现有HD Bundle取图，464条精确旧路径直接取高清，其余路径保持Resources原行为。场景绑定先于普通Awake；Prefab加载入口支持在实例化前检查动画数组。高清缺失或几何错误仍显式失败，不回退OLD。

通过新增Editor工具 `SwordsmanHd4xReferenceMigration` 迁移141个Prefab、4场景共3804处Sprite字段：原字段位置清空，ID绑定在加载时恢复。字段名/类型、数组长度、帧索引、SAF播放表、时序、布局和其他序列化数据保持。Editor完整保存副本保留于本轮 `unity-full-save/`，其无关重序列化已恢复为原文本，仅保留Editor生成的引用字段变动和145个绑定组件，未手工生成GUID。21份既有C#调整集中于加载调用、Resolver、Development验证与小地图按实际PPU换算尺寸，未改玩法。

验证命令：`Unity -batchmode -nographics -quit -projectPath /Users/ideabox/repo/Swordsman -executeMethod SwordsmanHd4xReferenceMigration.Validate -logFile Logs/hd-reference-stage2-20261116/unity-validation-final.log`。1971条描述与原Sprite的世界尺寸/Pivot/Border逐项一致，464条路径直接到高清；3804处绑定恢复及141个Prefab加载入口通过。141个Prefab/4场景的AssetDatabase递归依赖中无本批旧资源，全量Prefab/四场景对应Sprite硬引用扫描残留0。Editor及无高清宏的Development Player脚本编译通过，四场景Missing Script=0。

证据：`Logs/hd-reference-stage2-20261116/validation.json`、`remaining-hard-references.json`、`serialization-scope.json`、`protected-after.json` 和 `delivery.json`；既有源码与145个序列化资产完整清单在交付报告中。未新增第三方依赖，Unity自动生成新资源.meta。

保护复核200444份文件：166份预期变动（21份既有C#、141个Prefab、4场景），其余200278份哈希一致，无缺失或异常变化；旧资源、正式存档、HD缓存和唯一应用保持。新增4份C#、1份轻量JSON及Unity生成的.meta，未删除文件；同步4份文档。Git既有修改保留，新增路径均在本阶段范围内。Resources素材与Prefab仍遵循本地不入Git策略，未修改.gitignore或强制添加。编译保留19条去重历史C#警告及UnityEngine.UI重名提示，未扩展修复。

本次未删改旧PNG、旧Sprite/.meta或48张混合图集；未改动画帧表、玩法/剧情/数值/存档、Shader/插件、ProjectSettings/Packages、高清Bundle/缓存、像素共享后处理。Debug/Development应用构建与Player画质验收未运行，遵守用户限定的编译/场景验证范围。唯一应用仍是上次构建，当前安装包体积没有实际减少；Resources里的旧文件仍自动打包，解除引用不等于已从包内移除。后续清理/排除与构建需另行阶段授权。

## 2026-11-16 高清常驻第1阶段（源码验证，未重建应用）

本节覆盖下文历史记录中的高清宏、OLD回退和标记开关说明；历史日志只证明对应旧构建，不再作为当前缺包验收方法。

本阶段移除 `SWORDSMAN_HD4X` 条件编译及构建注入、已批准高清资源加载失败时的OLD回退、旧回退检查参数与分支，并删除运行时HD标记组件及其.meta。F8/鼠标标记开关、标记偏好读写与“HD 重制版”文字随组件退役；不删除已有用户偏好数据。章节测试台、F9、章节测试宏、原版角色/动画和未选中的图集切片保留正常原版路径。已批准高清资源缺失、Bundle拒绝或几何不匹配抛出 `HD_REQUIRED:`，不静默返回OLD。原资源仍提供现有几何与序列化引用，尚未迁移或清理。

修改：Resolver、Omissions运行时宏外壳、Development场景检查、HD构建器宏常量与唯一Player构建入口的宏数组；新增 `Assets/Editor/SwordsmanHd4xRequiredValidation.cs` 和Unity生成的.meta。验证器只读实际资源，内存注入故障并恢复，不改Bundle文件，不保存场景、资源或偏好。原611张旧资源、48张混合图集、Texture2D/Sprite/动画表、场景/Prefab、ProjectSettings/Packages、存档、无损共享后处理与HD缓存保持。

命令（Unity路径见本指南基线）：`Unity -batchmode -nographics -quit -projectPath /Users/ideabox/repo/Swordsman -executeMethod SwordsmanHd4xRequiredValidation.Validate -logFile Logs/hd-required-stage1-20261116/unity-validation.log`。Editor编译通过；19项检查涵盖实际地图/UI/人物图世界几何、正常原版资源、空可选引用、无Overlay类型，以及缺高清单图/UI/人物图、错误几何、重复失败不返回OLD。四场景Missing Script=0；通过 `PlayerBuildInterface.CompilePlayerScripts` 以Development和两个章节测试宏编译macOS Player脚本，不注入高清宏。输出在 `Logs/hd-required-stage1-20261116/player-scripts/`，不是.app。

保护复核覆盖200446份文件：仅5份预期源码变动、2份标记组件文件删除，其他200439份哈希不变，唯一应用、HD Bundle/缓存、全部旧资源、场景和存档均保持。git状态保留本轮开始前的既有修改；新增仅验证器及.meta，无异常新增。全仓库 `git diff --check` 仍报告原有level0场景尾空格（该场景本轮哈希不变），本阶段增量无尾空格。编译仍有历史代码警告及UnityEngine.UI重名提示，未扩展处理。

本阶段未执行Debug/Development应用构建或运行画质验收，遵守用户“完成编译和场景验证后汇报”的边界。唯一应用仍为上一阶段版本，包内旧标记须待后续获准重建才消失。本次不声称应用体积减少，未进入611张资源引用迁移/清理阶段。当前结果见 `Logs/hd-required-stage1-20261116/passed.txt`、`unity-validation.log` 与 `protection-result.json`。

## 2026-11-16 重复像素存储共享交付

用户批准先验证重复像素共享，再重建唯一应用；全部Texture2D、Sprite、名称、几何、采样/格式、动画播放表保留，不删除旧资源，不做有损压缩。确认用户正常退出并核实进程不存在后，使用唯一入口直接更新唯一 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，没有其他应用或完整应用副本。

最终普通文件逻辑字节求和：5,607,788,954 → **5,151,116,882字节**（5.151 GB / 4.797 GiB），实际减少 **456,672,072字节，8.1435%**。基础容器共享2,715个重复存储区段，减少299,276,296字节；高清容器共享565个区段，减少157,395,775字节，另有最终封装1字节差异。Unity Development构建Succeeded，0错误/0警告。`SWORDSMAN_MAC_BUILD size`仍为Unity原始BuildSummary、即共享后处理前体积；最终交付体积以本节和delivery.json实测为准。

新增 `scripts/share_texture_storage.py`：仅对同一流中像素SHA256与格式/采样等完整贴图设置一致的区段共享存储，保持对象，修改对应Texture2D流偏移及UnityFS块表/目录长度。未涉及的压缩块原样复制，保留原LZ4HC；受影响块重新封装，不足256字节的短尾保存为未压缩块。首轮候选遇到系统压缩器拒绝短尾后未交付，已修复；前/后置块表、跨块字段、相邻节点、整块移除、短尾及重叠拒绝测试通过。版本限定2021.3.16f1c1、序列化格式22；未知布局、非零容器内容摘要和Mesh流拒绝处理，不静默跳过。仅用Python标准库与macOS自带libcompression，无新增第三方依赖。两份候选完整验证后才替换本次构建的应用文件，随后沿用原启动后处理和签名校验；任一步失败不记录最终构建成功。

新增Editor验证 `SwordsmanHd4xBuild.ValidateTextureStorageCandidate`，源码 `Assets/Editor/SwordsmanTextureStorageValidation.cs` 及Unity生成.meta。Unity先后加载原/候选Bundle，1819资产、2329个HD Sprite的4148条贴图/几何描述一致，14张代表贴图完整GPU读回SHA一致，覆盖同稿地图、营地、Cursor与败阵重复帧；四场景Missing Script=0。最终包内HD容器SHA与该GPU验证候选完全一致。

最终包全部11760个Texture2D的逐对象像素哈希、格式、尺寸、采样及名称与共享前一致；96405个Sprite保留。共享前后序列化数据除Texture2D流偏移外归一化SHA完全一致，包含动画表、Sprite网格、Rect/Pivot/PPU/border、对象ID及引用；对象类别数量保持。原始流中约1.838 GB重复字节退出物理存储，但没有删除其对应对象、动画帧或像素内容，也不声称GPU内存减少。

Player显式检查通过：1819项HD资产、472几何探针/2005图集Sprite探针、36幕资源入口及隔离存档、50项键盘界面、31项对话、67项难度结算、第21幕资源几何，以及第3幕三种攻击/技能入口的18项地形界面检查和70帧原动画。五张1280×800真实设置/键盘/工作室截图与本轮新采集基线逐RGBA比较，差异像素均0，已查看实际截图。启动后处理 `--verify-only` 与codesign deep/strict通过。该范围不等于全剧情自然通关或用户美术/操作手感验收。

200166份保护文件无缺失、无异常变动；本轮构建入口修改及Automation/stage01/level03/Save0.dat隔离测试档重写为预期变化，其他200164份哈希保持，正式存档保持。旧PNG/Meta、批准稿、Sprite、原播放表、业务源码、场景/Prefab、ProjectSettings/Packages与HD构建缓存不改；原始HD Bundle SHA仍为 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`。应用内派生容器因共享存储而具有新SHA，不再用源Bundle整文件SHA判断该派生容器；用逐对象像素与元数据等价证明连接两者。旧交付记录中的源/应用Bundle SHA相同仅对应各自历史构建。

修改范围：`Assets/Editor/SwordsmanMacBuild.cs` 在原成功构建后、原启动后处理/签名前调用共享脚本，及本文；新增上述脚本、Editor验证器和.meta。未改原启动后处理脚本、未提交或推送。当前证据 `Logs/texture-storage-sharing-20261116/delivery.json`，源/候选与最终逐对象证明分别见 `base-candidate.json`、`hd-candidate.json`、`final-equivalence.json`；Unity/GPU见 `unity-candidate.json`，截图见 `screenshot-comparison.json`，保护见 `protected-after.json`，实际Player日志/报告在 `player/`。生产后处理记录位于 `Logs/TextureStorage/builds/`。后续构建自动复用同一流程；本轮完成，不自动进入旧资源删除。

## 2026-11-16 唯一应用无损压缩交付

用户批准仅做无损压缩、适配现有后处理并重新构建唯一应用；旧资源保留，不做纹理有损压缩。通过唯一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，无其他应用或历史应用副本。Development构建Succeeded，0错误、0警告；构建前脚本编译与四场景Missing Script=0。最终按普通文件逻辑字节求和：压缩前9,780,649,416字节，压缩后5,607,788,954字节（5.608 GB / 5.223 GiB），减少4,172,860,462字节（42.664%）；不是DMG下载体积。

仅修改构建入口的一项 `CompressWithLz4`、`scripts/macos_splash_postprocess.py` 的UnityFS兼容与本文；未新增源码文件或第三方依赖。兼容原独立布局，压缩布局使用macOS自带libcompression；本次后处理只重压第0块，解压内容仍只修改原有三个目标字节。跨块写入、前置/后置块表、相邻节点保持、重复执行、截断拒绝及LZ4互解检查通过；实际构建后处理、`--verify-only` 和codesign deep/strict均通过。没有改变签名身份、引擎库或工程中的启动设置。

容器全部7份 `.resS` 贴图/网格数据解压后SHA256与压缩前完全一致；包内HD Bundle保持锁定SHA `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`。容器外272份既有文件哈希一致（包括程序集、音视频及Bundle）；可执行文件签名、CodeResources和boot.config随重签名/封装更新。序列化索引有容器引用与构建标识变化，不将整包或全部索引描述为逐字节相同。五张1280×800真实Player设置/键盘/工作室截图与压缩前逐RGBA比较，差异像素均0；已查看实际截图。第21幕资源/几何检查通过，但早期截图仍有启动遮罩、无头截图无效，均排除出画质证据；正常图形应用已启动并观察章节菜单，不冒充全剧情自然通关或用户美术验收。

实际新包通过1819项HD资产、472几何探针、2005图集Sprite探针、36幕资源入口及隔离存档检查；50项虚拟键盘/界面、31项对话、67项难度/结算检查均通过。200144份资源、Meta、场景、脚本、缓存、配置及存档保护文件无缺失，200143份哈希保持；唯一变化是既有Automation/stage01/level03/Save0.dat隔离测试档被预期重写，正式存档保持。原图/高清PNG、纹理格式、业务、播放表、场景、Prefab、ProjectSettings、Packages均未修改；未删除旧资源、未提交或推送。

证据入口：`Logs/lossless-compression-20261116/delivery.json`，详细字节/节点哈希见 `resource-equivalence.json`，画面对比见 `screenshot-comparison.json`，保护检查见 `protected-after.json`；原应用只保留哈希基线与截图，不保留完整副本。复核工具与运行命令见同目录 `verify-resources.py`、`run-player-checks.py`、`build.log`、`compile-scenes.log` 和 `player/`。容器后处理工作流见下方“macOS启动标识后处理”。本轮验收完成；后续可由用户复验实际操作手感，不自动进入旧资源删除或有损压缩。

## 2026-11-16 剧情对话1.5倍与奖励toast缩时交付

用户批准默认正常/快速1.5倍、仅加速剧情对话，新增难度下方的单项设置并记住选择；随后批准金钱/物品提示停留2秒、保持0.3秒淡出。收到已退出回复并确认无游戏进程后，通过唯一构建入口直接更新唯一AllChapters Development应用，无副本。构建Succeeded、0错误、9既有警告；脚本与4场景Missing Script=0。

Editor31项通过后，实际新包31项对话/提示计时、61项键盘/界面、67项难度/结算及战斗档兼容检查全部通过，共159项。Player测得剧情出字比约1.51、自动翻页正常4.99秒/快速3.35秒；普通/经验消息仍为1倍率。分页手动继续及新消息状态重置通过；toast正常/快速设置下均为2秒停留、0.3秒淡出、约2.3秒结束，手动关闭回调只触发一次。鼠标新值点击、虚拟Ent与共用确认路由、简繁文案、位置、偏好重载、旧键缺省正常和原尺寸禁用导航通过。实际新包设置页截图已核看。不是操作系统物理按键注入测试，也不等于全剧情自然通关。

兼容：新增全局fastDialogue偏好，缺省0；旧存档字段、旧偏好键含义、剧情/战斗/奖励数值/选择框、动画、Time.timeScale不变。仅两个剧情事件入口用showDialogue；经验/系统提示继续showText。复用原行材质和字体，运行时增行，不保存场景/Prefab，不新增第三方依赖。所有验证偏好完整还原，不写正式槽位；67项测试仅使用既有EasyValidation隔离目录。603项保护文件（含现有存档、场景、工程配置）保持；签名deep/strict通过，包内HD Bundle锁定SHA保持。

新增：Assets/Scripts/ChapterTests/SwordsmanDialogueChecks.cs（共享验证与显式Player入口）、Assets/Editor/SwordsmanDialogueValidation.cs及Unity生成.meta。修改：GameSetting、GameHelpUI、GameImageText、GameMsgBoxUI、GameMsgBoxTextUI、GameBattleEventManager、GameRPGEventManager、GameBattleGetItemUI；同步原键盘/难度检查的导航索引与新偏好保护，并更新本验证/架构/状态文档。既有未提交工作保留，无异常Git文件，无提交或推送。

证据：Logs/dialogue-speed-20261116/delivery.json、build.log、editor-check.json、player/各报告和设置截图；复现Player入口为-swordsman-dialogue-report <绝对JSON路径>（图形模式，可传1280x800窗口参数），普通启动不运行检查。完整剧情手感等待用户实机验收。


## 2026-11-15 方案二与统一熟练度规则已更新唯一应用

用户要求构建，确认原应用进程已退出后，通过唯一入口SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters直接更新AllChapters.app，无临时/历史应用副本。构建Succeeded，0错误、9既有警告；脚本与4场景Missing Script=0。此前“源码完成、未打包”仅对应各自历史阶段，当前已入包。

当前Player实际通过67项难度/奖励结算、61项键盘/界面检查：简单经验110%、承伤90%，输出110%和回合HP10%/MP5%保持；三档重试后都不入账本关熟练度，之前总值不扣。方案二Ent/Esc与带横线翻页箭头、原回调和按需显隐已入包，实际新包截图已核看。82项Editor源验证为补充证据，不冒充Player测试。

签名deep/strict通过，Bundle SHA256仍为7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b；603项保护文件一致，包括现有存档、场景和工程设置。原PNG、输入配置、剧情条件、存档字段、五蕴成长和99级上限未改。本轮未新增/修改业务代码，仅构建、验证及更新归档文档；既有工作保留，无异常Git文件、无提交/推送。

当前证据：Logs/keyboard-labels-build-20261115/delivery.json、build.log、easy-check.json、keyboard-check.json及captures/。完整自然通关平衡、视觉和操作手感等待用户实机验收。


## 2026-11-15 方案二按键及统一重试规则（源码完成，未打包）

用户确认第二套局部效果图。四个控件内文改为带横线的上翻箭头、下翻箭头、Esc、Ent，保留80x80点击区、原确认/返回与Q/E翻页映射、按需显隐和位置。继续复用原金属圆环/半透明棕色RGBA纹理，以无字网格及原生文字/图标显示；不改PNG/Importer/场景，不从生成预览中裁图替代原有透明度。批准预览归档Builds/HD4X/static-mvp/work/virtual-controls-skins-20261114/keyboard-labels-20261115/approved-preview.png。

同时用户批准简单经验110%、承伤90%，并取消简单重试熟练度特例：三档均只在ReloadBattleCount==0时入账本关点数。之前总值保留，不重试的正常奖励/原剧情条件、99级与五蕴保持。原败阵提示无需特化修改。

SwordsmanPagingButtonsValidation.Run使用独立输出参数-swordsman-paging-output运行，82项Editor Play Mode检查通过：Ent/Esc文字尺寸与原回调/真实射线、无旧字叠印、翻页图标与原回调、弹窗/列表/商店显隐、100%/150%边界、三档经验实际产生/到账与伤害比例；实际GameBattleJudgment.UpdateProficiency在三模式和0/1/3次重试夹具中入账/不入账并清空阶段计数。奖励检查沿用源码已有Debug.LogError(24/25)诊断，不是脚本编译失败。旧检查夹具曾缺AI=None、导致经验条件不满足，已修正后重新通过。实际Editor截图已核对，不是Player新包画面。

修改GameVirtualKeyboardSkin、GameVirtualKeyboardPageFace、GameBattleUnit、GameEasyRules、GameBattleJudgment及对应Editor/Player专项检查期望、文档/清单。没有新增代码文件、依赖或存档字段；原资源/场景/工程设置/实际存档/当前唯一应用未覆盖。Player/Debug构建未运行：本轮用户未要求重打包，保留正在使用的应用。日志与截图位于Logs/keyboard-labels-20261115/。既有未提交工作保留，无提交/推送；等待用户指示后合并构建。


## 2026-11-15 温和简单档、X/Y及“中等”文案合并交付

用户确认参数后，统一更新唯一AllChapters Development应用，无临时/历史应用副本。简单档输出110%、承伤80%、经验120%、回合回复HP10%/MP5%；Normal=0显示中等，Hard=1不变。X/Y按需显示及原金属RGBA网格皮肤入包，翻页时贴右单列，不需要翻页的界面恢复原A/B位置。下方“源码完成、未打包”段落对应历史阶段，现已入包。

Unity脚本与4场景Missing Script=0；构建Succeeded，0错误、9既有警告。实际新包67项难度检查和59项键盘/翻页检查全部通过；包含新经验到账、连续升级/99级、回合回复、原两档公式、原五蕴成长、旧二进制战斗档往返、护送NPC、XY真实顶层射线命中及原回调、确认隐藏/恢复/关闭释放。Editor先前44项源代码检查补充城镇商店状态和150%边界，不代替Player结果。当前实际截图已检查，透明底面可透出场景，无旧A字叠印。

签名deep/strict通过，包内HD Bundle仍为7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b，603项保护文件哈希保持，包括现有存档/场景/工程配置。未改变等级上限、五蕴、剧情、存档字段、签名身份或输入配置，无新增依赖。只调整简单档策略/经验入口、同步专项检查，并把前序已验证UI修改合并打包；既有未提交修改保留，无异常Git文件，无提交/推送。

证据：Logs/easy-balance-20261115/delivery.json、build.log、easy-check.json、keyboard-paging-check.json及captures/。尚未证明整部游戏自然通关平衡，等待用户实机反馈；已获得等级不自动回退。


## 2026-11-15 X/Y按需显示与金属皮肤（源码完成，未打包）

X/Y仅在营地物品/炼化、RPG物品/商店列表可处理翻页时显示。列表关闭、确认/物品操作弹窗、对话/过场、暂停、标题和战斗时隐藏；商店开场列表尚未展开也隐藏。隐藏时仅释放翻页输入，Q/E映射、原翻页步长和A/B确认/返回不变。

四键目标均80×80；翻页界面贴右边缘单列，自上而下X/Y/B/A（逻辑锚点x=-45，y=297/213/129/45，键间4单位）。非翻页界面恢复A(-111,45)、B(-45,111)。相比两行布局减少对底部物品说明的覆盖；100%和150%下四个点击区均不重叠、不出屏。X/Y以原A素材的金属圆环和半透明棕色像素绘制原生UI网格，再叠独立字母；没有新增/改写PNG，保留原RGBA透底。新图形继承原UI层，并禁止EasyTouch重新显示带A字的底图，避免叠字。

验证：SwordsmanPagingButtonsValidation.Run，真实Editor Play Mode的44项检查通过，包含实际UI射线命中、原指针回调、物品列表实际翻页、营地/炼化/城镇列表、商店展开/确认状态、关闭释放、两档尺寸和布局复原；商店部分为界面状态夹具，不是完整买卖剧情流程实测。初轮检查脚本的访问级别、弹窗关闭接口和反射字段定位问题已修正；实际画面暴露的旧底图重叠及UI层问题已修复并重新通过。检查只改变内存，不写游戏存档或偏好，不保存场景。

新增GameVirtualKeyboardPageFace.cs、Editor/SwordsmanPagingButtonsValidation.cs及Unity生成.meta；修改GameTouchRightUI、GameVirtualKeyboardSkin、GameTouchManager及GameShopUI的显示查询。原PNG、场景/Prefab、插件、输入配置、翻页业务、战斗/剧情/存档格式不改。脚本编译与4场景Missing Script=0；Player/Debug构建未运行，当前唯一应用不覆盖，等待用户指示后和“中等”文案合并打包。日志/真实Editor截图：Logs/virtual-keyboard-xy-20261115/。无提交或推送，既有未提交工作保留。


## 2026-11-15 三档难度已入唯一测试包

用户批准简单／普通／困难方案。唯一Development包构建成功（0错误、9既有警告）；4场景Missing Script=0，实际新包67项难度检查、48项虚拟键盘/设置检查、36关资源入口及普通/战斗/旧档只读副本兼容通过。签名深度严格校验通过。自动化测试槽更新属于检查输出，正式及手动章节存档、场景、工程配置和原资源保持。规则与验收范围见[difficulty-mode-proposal.md](difficulty-mode-proposal.md)；日志与截图位于Logs/easy-mode-20261115/。完整自然通关和难度平衡等待用户实机验收。

复现：统一Player构建入口；新包显式参数 `-swordsman-easy-check <绝对报告路径>` 在ChapterTests/v2/EasyValidation隔离运行，退出前还原偏好；`-swordsman-virtual-keyboard-report <绝对报告路径>` 验证实际指针处理器；`-swordsman-chapter-tests-check -swordsman-chapter-tests-report <绝对报告路径>` 沿用36关与存档兼容检查。前后两种战斗检查可加 `-batchmode -nographics`；截图检查使用图形Player。三项检查逐次运行，不让设置偏好互相干扰。


## 2026-11-13 地图修订已归档并统一更新唯一测试包

上方全部修订已获用户美术GO，本次Map13/Map20/Map20-1/Map21/Map22-1归档完成，并与此前Map01/02/03/04/12/13-1及Map18四组8帧一起入包。唯一AllChapters应用直接更新，没有中间或历史应用副本。Unity脚本与4场景Missing Script=0；Development构建成功，0错误、9既有警告，670.311秒。高清缓存1819项仅19项改变，1800项复用；整包更新并验证约586.104秒，不等同于全部图片重导入。

签名深度严格校验通过，应用身份未变；包内Bundle与构建输出及缓存SHA一致。实际新应用静态检查1819项、472探针、2005个图集Sprite通过；18/21/22幕实际场景检查通过且saveWrites=0，108份既有存档哈希保持。原资源/场景/业务/工程设置保护检查通过，Git路径状态与开始一致。运行加载检查不代替用户整体视觉/剧情验收，用户实机待确认。

证据：`Builds/HD4X/static-mvp/work/map-revisions-final-archive-20261113/delivery.json`、`build-verification.json`及`run-context.json`定位日志。本段覆盖此前“其余地图完成后再构建”的延期状态。

## 2026-11-13 最新状态：map03接缝修复已美术GO并归档

用户确认“验收ok，GO”。已将版本 `dfcc8ada7e2c1fc0` 原样替换至approved map03，更新同名JSON、构建器一处批准SHA、唯一清单和评审美术GO标记；保留待入包状态。原版/Sprite/GUID、map01/02已批准稿、RPG-map02、Bundle、导入缓存与应用均未变。未启动Unity，未运行Debug/Bundle/Player；继续等其余地图修完再统一构建。证据 `Builds/HD4X/static-mvp/work/map03-seam-repair-20261113/approved-resource-verification.json`。下文待确认/approved未改描述仅对应制作阶段历史。

## 2026-11-13 最新状态：map02 汾陽道已美术GO并归档

用户确认“验收ok，GO”。已将版本 `94c0e0cc0baf1e84` 原样替换至approved map02，更新同名JSON、构建器一处批准SHA、唯一清单和评审美术GO标记；保留待入包状态。原版/Sprite/GUID、map01已批准稿、RPG-map02、Bundle、导入缓存与应用均未变。未启动Unity，未运行Debug/Bundle/Player；继续等其余地图修完再统一构建。证据 `Builds/HD4X/static-mvp/work/map02-fenyang-inscription-20261113/approved-resource-verification.json`。下文待确认/approved未改描述仅对应制作阶段历史。

## 2026-11-13 map01 美术GO归档验证（未构建）

执行 `Builds/HD4X/static-mvp/work/map01-full-geometry-redraw-20261113/approve-resource.py`：确认用户批准版本 `a1e0c6a583917a08` 与候选/评审一致后，逐字节替换原approved PNG；更新同名JSON与构建器唯一map01批准SHA。静态检查确认候选/approved/评审SHA完全一致、构建器其他内容不变、原几何字段及源/原生分块哈希不变。Bundle、唯一应用及稳定导入缓存目录的路径/类型/大小/修改时间指纹前后一致。

Debug、Bundle、Player均未运行：用户明确要求其他地图修完后统一构建。当前证据 `approved-resource-verification.json`；下节制作验证对应归档前阶段，其中verify.py要求旧approved未变的断言仅用于该阶段，归档后不要重跑旧拼接/发布流水线来覆盖已批准成果。后续正常打包由稳定缓存检测approved新SHA，不清空缓存。

## 2026-11-13 map01 离线拼接修复验证

任务 `Builds/HD4X/static-mvp/work/map01-full-geometry-redraw-20261113/`。依次使用现有 `/Users/ideabox/Applications/ImageRestoration/env/bin/python` 执行 `assemble.py`、`assembly-fix-20261113/check-objects.py`、`publish-review.py`、`verify.py`；`verification.json` 的输出哈希由当前 `assembly-verification.json` 同步。旧 `rebuild-floor.py` 与 `protect-lettering.py` 已退役并拒绝运行，禁止再执行地砖整层覆盖和旧字回贴。没有安装依赖或新增生成。

当前版本 `a1e0c6a583917a08`：14处完整区域回读差异0，15张原生分块SHA不变；原PNG/旧approved/原Sprite来源与世界尺寸、Alpha/外部黑区及覆盖完整性检查通过。十组原尺寸局部高清侧直接取当前最终PNG，评审PNG与HTTP响应逐字节一致。运行 `node assembly-fix-20261113/check-review.cjs`，隔离无界面Chrome从总入口进入第一幕和map01，检查本版、OLD/HD自然尺寸4096、完整加载及100%显示尺寸；`review-fit.png`只作布局和页面证据。CUA连接超时，未检查用户当前浏览器窗口。

Debug构建未运行：本轮只交离线完整对照；未覆盖原版、approved、Unity资源、Bundle或唯一应用。用户美术GO和运行验收仍为false，待用户确认后再入包，运行时仍须核对独立桌面覆盖片。证据为任务 `assembly-verification.json`、`assembly-fix-20261113/object-integrity.json`、`final-verification.json`、`review-publication.json` 和 `browser-check.json`。

## 当前安装包规则（2026-11-12，覆盖下文历史产物说明）

用户明确只保留 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。历史、实验、普通/高清预览、独立测试及CacheBenchmark应用已全部退役并清理，不保留整包回退副本。唯一构建入口为 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，Unity直接输出到该路径；不得先生成另一份应用再复制覆盖，不再创建其他 `.app` 名称。旧入口及 `-swordsman-cache-benchmark` 参数已移除/拒绝。

资源Bundle、稳定导入缓存、原版资源、批准稿/原始生成输入、评审和最小验证证据继续保留。下文旧应用名、独立输出及保留说明仅是历史记录，不是当前保留清单或后续打包指令。本轮清理未改变资源、美术或用户实机验收状态。


## 公司 Logo 启动页检查（2026-11-13）

`SwordsmanStartupPrototypeValidation.ConfigureAndValidate` 只配置获准的 `StartupPrototype/PortraitStill.png` 导入器并验证：批准PNG哈希、原GUID、2560×1600/RGBA32、NPOT None、无mipmap、Alpha和1280×800完整画布等比排版，再检查四主场景。普通重复验证用 `Validate`。启动覆盖层不包含额外文字或Loading动画。

图形Editor执行 `SwordsmanStartupPrototypeValidation.RunPlayMode -swordsman-startup-capture`（不带prototype启用参数或章节测试标志），不加batchmode/nographics/quit，自动验证公司Logo、无人物VideoPlayer、原片头首内容帧后撤层、跳过/缺片单次回调并退出。默认启用回归报告 `Logs/startup-company-logo-default-20261113/editor-playmode.json`，历史图片接入报告保留原位。

唯一应用通过 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 直接更新。实际应用验证附带 `-swordsman-startup-capture -swordsman-startup-capture-directory <绝对目录> -swordsman-startup-report <绝对JSON路径>`，正常片头结束后输出纹理/绘制/内容首帧/撤层顺序并退出。手动验收不带这些参数，不自动退出。不设置额外停留，不以Editor计时冒充独立应用冷启动，不宣称引擎可绘制前已有公司Logo。

本轮实测：编译、图片/GUID/几何、四场景、Editor实际首内容帧/撤层/跳过/缺片通过。最终唯一应用构建Succeeded，0错误/9既有警告，1819项高清缓存复用、0重导入；最终Player报告passed=true、退出0，启动截图右上角背景像素检查确认HD标记已隐藏，用户原批准画面已显示。严格签名检查、2044项保护文件哈希、唯一应用和Git状态路径检查通过；引擎Development水印保留，2026-11-13用户实机验收通过并确认沿用本版。最终结果见[启动记录](startup-logo-prototype.md)和 `Logs/startup-company-logo-20261113/`，旧日期结果不作为本轮验证。

默认macOS启动条件已解除开发包/测试标志/启用参数依赖。`SwordsmanStartupPrototypeValidation.ValidateReleaseScripts` 仅编译实际macOS正式脚本（None、无额外标志），并从新程序集反射检查Supported及Enabled；分别在batch/nographics和图形进程执行。已通过supported=true、批处理enabled=false、图形enabled=true；四主场景Missing Script=0。Editor无启用参数的正常Logo→原片头及跳过/缺片回归已通过，证据 `Logs/startup-company-logo-default-20261113/`。首次常规图形进程卡在引擎初始化，重跑仅附加 `-force-gfx-direct`，不变更工程配置。

正式包实际验收仍需检查：不带Development/AllowDebugging、水印不显示、公司Logo→原片头→菜单完整运行。本轮不构建任何Player、不覆盖已验收唯一应用，不把脚本编译或Editor检查描述为正式包实机通过。

## 高清资源构建缓存（2026-11-12）

`SwordsmanHd4xBuild.BuildAndValidateApprovedBundle` 自动检测输入变化。缓存命中时校验并复用原Bundle；图片或切片/导入参数变化时，只重新导入受影响图片，再构建同一个Bundle。没有拆包，没有修改运行时加载入口，批准稿和原版源文件的SHA准入校验保持。

- 图片与Unity生成的 `.meta` 固定保存在 `Assets/SceneDependencyCacheSwordsmanHD/`；清单位于 `Library/SwordsmanHD4X/build-cache.json`，保存逐项输入、图片/导入配置及Bundle哈希。两者均由现有Git排除规则保护，不修改 `.gitignore`，不提交、不作为美术归档。
- 指纹包括批准PNG、原版PNG及meta、实际源Sprite依赖与Rect/Pivot/PPU/border、切片选择/倍率/纹理尺寸、导入规则、Unity版本和目标平台。普通运行时代码变化不使全部图片导入缓存失效。
- 导入目录长期保持稳定，其内使用 `ImportedResources`，严格禁止 `Resources` 子目录，避免Player自动收录。仅在构建时使用Unity内置 `AssetBundleBuild.addressableNames` 把物理缓存路径映射回既有 `assets/__swordsmanhd4xstaging/...` 包内名称；未引入Addressables包。包内路径、运行时加载代码、原版资源和原版GUID保持；有效缓存沿用自身GUID。
- 图片缓存损坏时只修复对应项；清单缺失、截断、不兼容或无法证明输出有效时重建。缓存状态只在Bundle校验成功后发布。批准输入哈希不符直接报错，不把未批准图片自动接入。
- `SWORDSMAN_HD_CACHE`日志记录 `mode`、`assets`、`imported`、`unchanged`、`removed`、输入检查与总耗时。未变命中应为 `mode=reused imported=0`。首次缓存建立仍需导入和构建；同一个大Bundle有变化时仍可能整包重新压缩。
- 原有 `-swordsman-reuse-hd-bundle-sha256` 手动锁定校验入口保留；日常使用默认入口即可自动检测。

当前耗时对比也直接使用唯一AllChapters入口与同一个输出路径，按每次日志记录耗时。先确认游戏已正常退出；不为对比另建应用名、不复制完整应用。此前CacheBenchmark参数已退役，误传时构建入口会报错。

缓存建立后，可单独运行 `-executeMethod SwordsmanHd4xBuild.ValidateApprovedBundleCache`，附加 `-swordsman-hd-cache-report <Logs下绝对JSON路径>`。该检查覆盖未变、新增、修改、移除、导入参数变化、坏清单/错误Bundle哈希拒绝，并实际损坏及修复一张可再生缓存图片；原版及批准图片保持。真实新图改图在隔离副本中模拟，逐项选择检查不冒充新增美术已接入。只有显式运行该Editor入口才执行这些检查。

两处均为可再生缓存，可在Unity关闭时删除后重建；不要在日常打包前清空。若残留旧版 `Assets/__SwordsmanHd4xStaging/` 或缓存内出现 `Resources` 子目录，工具报错，不继续Player构建。禁止把图片反复移出/移回Assets：本轮实测该做法会导致未变图片也重新导入，已退回。耗时与最终构建验证记录见本节后续交付记录。

### 本轮实测结果（2026-11-12）

同一本机、Unity 2021.3.16f1c1、1819项高清输入及相同全章节Development宏/窗口，使用上述显式对比输出参数，未使用手动SHA复用参数。最终稳定目录版首次建立/迁移缓存耗时812.445秒（13分32秒）；随后未变完整打包203.397秒（3分23秒），约为首次的1/4。此前原流程约24分钟是历史日志估算，作为参考，不能当作严格重复测量。

未变构建日志为 `mode=reused assets=1819 imported=0 unchanged=1819`，Unity实际PNG导入记录为0；批准输入及缓存检查102.487秒、含Sprite/Bundle校验的高清阶段共130.783秒，其余约72.6秒为编辑器启动、应用生成、复制、签名及清理等。只做本步骤后仍保留这些校验成本。

10项缓存检查通过；实际单张缓存损坏修复只记录TREASURE_t0一张PNG导入，另外1818项没有重导入，修复后1819项缓存记录（包括meta哈希）完全一致，Bundle SHA保持。新增/修改图片及导入参数/移除的检测在隔离副本与两项样本中验证选择范围；整批1819项另做未变与单项修复全量检查，没有修改批准图。

最终对比包构建Succeeded，0错误/0警告；1819资源/2005图集Sprite探针、36幕入口和存读档隔离、4场景Missing Script=0、深度严格签名检查通过。包内与构建输出Bundle SHA相同，缓存目录未出现在Player使用资产清单内，应用大小未因缓存增加。4022个未授权修改区域的目标保护文件哈希不变。当前用户运行中的AllChapters未覆盖，输出为独立CacheBenchmark应用。

完整证据：`Logs/hd-build-cache-20261112-170145/report.md`、`delivery.json`、`cold-v2-timing.json`、`warm-v2-timing.json`、`validation-v2-checks.json`与最终验证日志。此前移出/移回Assets的方案在真实单项测试中引起未变图片重新导入，已退回并停止该实验；其日志仅为诊断证据，不计入最终耗时结果。


- 更新日期：2026-11-12
- Unity 基线：`2021.3.16f1c1`

## 环境前提

当前本机 Unity Editor：

```text
/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity
```

这是当前机器路径，不写入工程配置。若不存在，应通过 Unity Hub 确认相同版本安装位置，不自动换用其他版本。

完整运行还要求本地存在未由 Git 管理的 `Assets/Resources/`、`Assets/Sprite/`、`Assets/Texture2D/`。

## macOS 窗口与画幅基线

- macOS 构建和受控冒烟测试的基线为 `1280 × 800`。全章节高清测试包的双倍窗口试验默认使用 `2560 × 1600`，画幅同为 `16:10`。
- 该测试包普通启动时将窗口设为双倍尺寸，覆盖上次退出时记住的全屏状态。显式传入窗口参数时，以参数为准。
- 游戏逻辑视口保持 `320 × 200`。验证目标是按整数倍显示原画幅，不扩大可见逻辑范围，不拉伸，也不裁切上下内容。
- 冒烟启动时显式传入窗口参数，避免本机历史 `PlayerPrefs` 或上一次退出时的窗口状态干扰结果：

```bash
open "Builds/macOS/Swordsman-ChapterTests-AllChapters.app" --args \
  -screen-width 1280 \
  -screen-height 800 \
  -screen-fullscreen 0
```

- 正式构建还必须把相同尺寸写入正式构建默认配置，使用户首次启动且没有命令行参数时也得到 `1280 × 800`。当前工程仍是 `1920 × 1080`、非 Windowed 配置，这一项尚未实施，不能仅凭冒烟参数认定正式默认配置已完成。
- 验证时记录实际渲染区域和原生窗口内容区；macOS 标题栏不计入 `1280 × 800` 游戏内容尺寸。

## 一级：文本和仓库检查

适用于纯文档或明确不影响 Unity 的修改：

```bash
git diff --check
git diff --stat
git status --short --branch
```

检查无关变化、本地资源或构建产物、`.meta` 异常、大量序列化变化和敏感信息。

## 二级：脚本编译和场景验证

关闭正在占用项目的 Unity Editor 后执行：

```bash
UNITY_EDITOR="/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity"
"$UNITY_EDITOR" \
  -batchmode -nographics -quit \
  -projectPath "/Users/ideabox/repo/Swordsman" \
  -executeMethod SwordsmanSceneValidation.ValidateScenes \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/swordsman-scene-validation.log"
```

成功标准：退出码为 0；日志包含 `SWORDSMAN_SCENE_VALIDATION_COMPLETE`、`scenes=4`、`missingScripts=0`；没有 C# 编译错误。该工具只验证场景可加载和 Missing Script，不等于玩法测试。

## 三级：macOS Development Build

```bash
UNITY_EDITOR="/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity"
"$UNITY_EDITOR" \
  -batchmode -nographics -quit \
  -projectPath "/Users/ideabox/repo/Swordsman" \
  -executeMethod SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/swordsman-macos-development-build.log"
```

输出为 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。成功标准：退出码为 0；日志包含 `SWORDSMAN_MAC_BUILD result=Succeeded` 和 `errors=0`；应用存在且可启动。此脚本启用 Development 和 AllowDebugging，不是发布构建。

### macOS 启动标识后处理（2026-11-11）

`SwordsmanMacBuild` 的 Development 入口共用 `scripts/macos_splash_postprocess.py`，在构建成功后执行、最终成功日志之前完成。它使用当前编辑器自带的 `binary2text` 重新定位 `globalgamemanagers` 中的 PlayerSettings 和 BuildSettings 对象，再按已验证的 `2021.3.16f1c1` 前段布局读取三个目标字段。不使用上次构建的绝对偏移，不修改工程中的 Splash 设置、许可文件或引擎库。

2026-11-16起，唯一Development构建增加 `BuildOptions.CompressWithLz4`，对Player数据做无损容器压缩；不改变纹理格式、高清Bundle压缩方案或源资源。后处理同时支持原独立文件布局和 `data.unity3d` UnityFS布局，后者通过macOS自带libcompression读取目标节点，只重压承载三个目标字节的分块，其余压缩分块原样复制，更新块表与容器长度后原子替换。保留原目标节点及变化分块作为最小证据；版本、压缩类型、目录边界或非零内容摘要不符合已支持布局时拒绝交付。`--verify-only`对两种布局均检查目标字段和应用签名。容器无损性还应单独核对解压后贴图数据SHA256与构建前基线，并做Player运行和实际截图检查。

后处理检查文件版本、对象边界、产品名、四个场景顺序、字段布局和初始状态；不匹配则构建入口报错，不回退为带Logo的成功包。修改数据后重新进行本地 ad-hoc 签名，并执行 `codesign --verify --deep --strict`。数据备份、对象位置、修改前后哈希和结果保存在 `Logs/UnitySplash/builds/`；失败时恢复原数据并标记该包不可交付。只适用于当前 macOS Development 流程，不扩展为商店签名/发布能力。

用户验收始终使用统一入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，输出并更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。不为去Logo新增独立应用名或构建入口；已移除本次临时 NoUnityLogo 入口，已验证的新包已更新到统一路径。高清Bundle沿用相同准入和哈希校验。启动人工验收仍需观察Logo、黑屏和正常进入菜单，字段和签名通过不代替实际画面验证。

机制、隔离试验和适用边界见 [启动画面调研与验证](unity-splash-screen-research.md)。不新增第三方依赖；调用现有 `/usr/bin/python3` 标准库和当前 Unity 编辑器工具。

静态高清与章节测试统一使用上述AllChapters入口及唯一应用，不再另建普通、HD4X或UIReview应用。高清Bundle按缓存自动检测与校验，F8显示开关保持。

Development Player 可用以下参数验证22项静态资源组（28个探针）、20种动态/未知/未选切片拒绝替换，以及2项几何不匹配回退；该参数缺省时不执行自检，也不影响正式默认路径：

```bash
"Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman" \
  -batchmode -nographics -swordsman-static-mvp-check \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/static-mvp-runtime-smoke.log"
```

成功标记为 `SWORDSMAN_STATIC_MVP_SMOKE passed mode=static statics=22 portraitSlices=33 newPortraitSlices=3 propSlices=1 probes=28 excluded=20 geometryFallback=2`。缺包或无效包检查仅在隔离构建产物内临时替换Bundle，增加 `-swordsman-static-mvp-expect-fallback`，应输出 `mode=fallback`；测试后必须恢复新静态Bundle并核验哈希。本轮额外验证不完整的前一版8项静态包被拒绝，测试后删除临时备份；后续负向测试使用临时无效/不完整包，不恢复退役实验资源。不得为测试修改原资源或全局宏。

当前Bundle精确包含10张单图、8个头像图集（33表情）、1个固定桌面片图集及3个UI图集（仅Title_t0、MSG_BOX_t0、HelpWindow_0）。UI浮点几何和border按世界单位验证；标题/帮助直接Image初始化必须另行核对实际显示，HelpWindow仍为Simple。构建前后必须比较ProjectSettings原文件哈希，避免Unity序列化规范化留下配置差异。

实机验收在同一镜头检查幕标题、静态地图/前景、对话头像的构图、遮挡、边缘和文字布局，并确认场景角色、NPC、怪物、全部动作/光影/粒子仍是原版。自动自检不能代替完整剧情和整体视觉验收。

### RPG真实加载入口检查

合成解析器探针不覆盖`GameRPGManager.showLayer`是否调用解析器。Development静态预览额外支持：

```bash
"/Users/ideabox/repo/Swordsman/Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman" \
  -batchmode -nographics -swordsman-rpg-map-check \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/rpg-map-fix-actual-scene.log"
```

该参数仅在`SWORDSMAN_HD4X && DEVELOPMENT_BUILD`启用；默认不执行。新进程只在内存中选择RPG 1，进入真实level1场景，经`GameSceneRPG.Start → showLayer`检查实际SpriteRenderer。成功标记：`SWORDSMAN_RPG_MAP_CHECK passed mode=static scene=1 texture=4096x8192 ppu=4 world=1024x2048`。检查不调用存档写入，不改剧情数据；须核验现有存档哈希不变。

在测试进程内临时缺包或损坏包，附加`-swordsman-static-mvp-expect-fallback`，应为`mode=fallback`；必须finally还原包并核验哈希。去掉`-batchmode -nographics`并加`-swordsman-rpg-map-review`可保留新检查进程、查看武器铺区域；退出该检查进程，不关闭玩家原有进程或写入存档。实际场景检查和助手观察仍不代替用户运行视觉确认。

### 第一幕真实加载入口检查（2026-11-01）

第一幕离线资源全部确认后统一验证，Development静态包额外支持：

```bash
"/Users/ideabox/repo/Swordsman/Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman" \
  -batchmode -nographics -swordsman-chapter01-check \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/chapter01-actual-scene.log"
```

成功标记为`SWORDSMAN_CHAPTER01_CHECK passed mode=static scene=3 map=1 portraits=8 prop=1 hideShow=passed originalUnits=12 saveWrites=0 fixture=true`。新进程只在内存中选择Stage01，加载真实level3，经`showLayer`、`GameAnimation`、`GameMsgBoxUI.showText`检查地图、新8表情及桌面片；执行原组件的隐藏/重新显示路径。不执行完整剧情或回合、不调用存档写入，不能视为完整第一幕玩法验收。

该入口与RPG检查均只在`SWORDSMAN_HD4X && DEVELOPMENT_BUILD`编译、且显式传参时启用。去掉`-batchmode -nographics`，附加`-swordsman-chapter01-review -screen-width 1280 -screen-height 800 -screen-fullscreen 0`可保留该检查进程观察；可附`-swordsman-chapter01-screenshot <Logs或Builds下绝对PNG路径>`保存当前实际渲染帧。该画面为Development检查场景，不是完整剧情自然推进截图；用户整幕实机确认独立记录。普通启动不自动检查。

缺包/坏包检查在临时克隆的隔离应用内逐次执行，附加`-swordsman-static-mvp-expect-fallback`；应输出`mode=fallback`。finally还原克隆Bundle并核验哈希，测试后删除克隆；不改用户正在运行的应用或原资源。构建和验证前后检查本地资源、场景、配置、普通应用及存档哈希，保持字节一致。

## 人工冒烟测试

按变更范围选择，基础路径建议覆盖：

1. 启动并进入标题界面。
2. 验证键盘方向/WASD、Return 和 Escape 的相关交互。
3. 开始新游戏或读取本地存档。
4. 在 RPG 场景完成移动或事件。
5. 进入营地并打开相关界面。
6. 进入战斗，完成至少一回合或目标操作。
7. 正常离开战斗并返回预期场景。
8. 涉及存档时完成保存、退出、重启和读取。
9. 确认音乐、音效、视频和主要贴图没有缺失。
10. 分别对正式构建和 Debug/Development 构建确认游戏内容区为 `1280 × 800`、画幅为 `16:10`，不存在横向拉伸、上下裁切或额外逻辑视野。

默认测试不使用真实云账号、支付账号或线上服务。

## 验证矩阵

| 修改类型 | 最低验证 |
| --- | --- |
| Markdown | diff check、链接、Git 状态 |
| 独立 C# 工具 | Unity 编译、工具专项测试 |
| `MonoBehaviour`/序列化字段 | 编译、场景验证、相关冒烟 |
| 场景或 Prefab | 场景验证、Development Build、相关流程 |
| 资源路径或资源 | Development Build、引用路径冒烟 |
| 场景顺序或切换 | 四场景验证、构建、完整切换链路 |
| 存档 | 旧档读取、新档写入、重启读取、失败测试 |
| 网络/云/激活/支付 | 专项方案、明确授权、隔离环境、合规检查 |
| ProjectSettings/插件/Unity | 全量导入、场景验证、构建、平台专项测试 |

## 失败与日志

- `Logs/` 不提交 Git；历史日志只能证明过去状态。
- 汇报时注明“本轮运行”或“既有日志”。
- 警告不得自动批量修复，应判断是否由本次修改引入。
- 失败时保留关键错误，判断是本次修改、资源缺失、版本不匹配还是已有问题，只修复已批准范围。
- 不默认删除 `Library/`、重导全资源或升级 Unity；这些操作必须先说明并获得确认。

## 章节测试台专项验证

独立测试包的构建、命令行入口、隔离存档规则与检查范围见 [章节测试台](chapter-test-desk.md)。36 关资源入口加载不代表全剧情通过；新增或调整逐幕预设后，应重新执行专项检查，并人工验证对应剧情与战斗起点。

全章节快照检查另记录开场到达玩家回合、包内安装、跨进程读取与前后状态一致性；普通营地读取独立记录。原始索引34的 Camp34 Prefab 缺失列为营地读取阻塞，不能以战斗快照可读替代该项通过。详见章节测试台文档的逐幕结果。


### 第二幕AllChapters统一验证（2026-11-01）

当前用户指定产物为`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，构建沿用`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`；完整命令见[章节测试台](chapter-test-desk.md)。本轮没有重建旧`Swordsman-HD4X.app`，该旧二进制仍保留第一幕19项包；检查该阶段22项时必须使用AllChapters目标，不能引用旧包日志作为新结果。

新增第二幕实际入口检查：

```sh
Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman \
  -batchmode -nographics -swordsman-chapter02-check \
  -swordsman-chapter-tests-package-check \
  -logFile /Users/ideabox/repo/Swordsman/Logs/chapter02-actual-scene.log
```

成功标记为`SWORDSMAN_CHAPTER02_CHECK passed mode=static scene=3 map=1 title=1 portraits=9 newPortraits=3 reusedPortraits=6 originalUnits=14 saveWrites=0 fixture=true testDesk=true`。仅在高清、Development与章节测试宏同时存在且显式传参时运行，经现有测试台inspect进入真实Stage02；检查地图、实际幕标题、9张对话图片、世界尺寸/Pivot与原版单位初始化，不推进剧情或保存。使用package路径参数可保持检查目录独立。缺包/坏包仅在临时应用克隆中测试，增加`-swordsman-static-mvp-expect-fallback`，应为`mode=fallback`；不修改原目标应用Bundle。

去掉`-batchmode -nographics`，增加`-swordsman-chapter02-review -screen-width 1280 -screen-height 800 -screen-fullscreen 0`可观察实际渲染；附加`-swordsman-chapter02-screenshot <Builds或Logs下绝对PNG路径>`可保存检查帧。该图为测试入口，不能称为正常剧情/通关截图。检查进程只结束自身，不能关闭用户已有游戏进程。

本轮还重跑了第一幕入口、RPG入口、测试台36幕入口/隔离存档回归，并用`python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter02-package-regression`核对36个包内起点。新包构建Succeeded、0错误/9警告，196714个保护文件不变；临时高清导入、StreamingAssets、回退克隆均清理。索引34的Camp34缺失限制仍保留；所有自动结果和路径仅在`evidence/chapter02-integration-validation.json`汇总。最终第二幕正常剧情和人物动作同屏仍须用户实机确认。


### 第三、四幕AllChapters交付验证（2026-11-01）

当前目标仍为`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，已按同一构建入口携带34项静态纹理/54头像表情重建；旧HD预览应用保留原版本。Development构建Succeeded、0错误/9警告，四场景Missing Script=0。完整证据为`Builds/HD4X/static-mvp/evidence/chapter0304-integration-validation.json`，包含归档哈希、55探针/24排除/2几何回退、实际入口、回退、存档隔离及36幕包内起点回归。

```sh
Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman \
  -batchmode -nographics -swordsman-chapter0304-check -swordsman-chapter0304-stage 3 \
  -swordsman-chapter-tests-package-check \
  -logFile /Users/ideabox/repo/Swordsman/Logs/chapter0304-chapter3-actual-scene.log
```

将stage参数改成4检查第四幕；RPG02使用`-swordsman-rpg02-map-check`。这些仅显式Development资源检查，不推进正常剧情。去掉batchmode/nographics，附加`-swordsman-chapter0304-review -screen-width 1280 -screen-height 800 -screen-fullscreen 0`观察检查帧；可用`-swordsman-chapter0304-screenshot <Builds或Logs下绝对PNG路径>`保存，不能当成通关截图。缺包/坏包回退仅在临时克隆中验证，不损坏交付包。

15项资源/场景/回退检查通过；测试台36幕入口与隔离存档回归、36个包内合成战斗起点重载通过。Camp34既有限制仍单列，普通营地读取35项通过、1项阻塞。196717个保护文件不变。完整第三/四幕正常剧情及全部分支仍待用户实机确认；已打开正常测试台选中第三幕，Enter开始，F9返回后选第四幕。自动检查参数不用于用户手动验收实例。


### 2026-11-02 当前46项AllChapters复验包

本次运行`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，日志`Logs/chapter0304-floor-development-build.log`实时结果Succeeded、0错误/9警告；`SwordsmanSceneValidation.ValidateScenes`四场景Missing Script=0。当前46纹理与其Bundle SHA256、13归档输出、保护文件及各检查日志统一见`Builds/HD4X/static-mvp/evidence/chapter0304-floor-integration-validation.json`；前述34项报告为修复前历史范围。

执行`work/chapter-03-04-floor-integration/run-checks.py`（相对static-mvp目录），21项正常与缺包/坏包回退均通过；资源烟测67探针/28排除/3几何回退，错误单图缓存几何须回退。`capture-runtime.py`调用实际特写入口，第三/四幕各18个UI检查保持512×256、第三幕70原版入场帧检查通过，并生成9张1280×800实际渲染截图供助手核看；这仍不是用户正常剧情确认。Fixture仅存在于Development章节测试条件编译及显式启动参数下。

复现地形检查：`Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman -batchmode -nographics -swordsman-fixed-terrain-check -swordsman-fixed-terrain-stage 3 -swordsman-chapter-tests-package-check -logFile /Users/ideabox/repo/Swordsman/Logs/fixed-terrain-stage3.log`；stage换4核验第四幕。附加`-swordsman-fixed-terrain-screenshots`与绝对输出目录并去掉batchmode/nographics才采集真实GPU截图。缺包/坏包只对自建临时克隆测试，不破坏交付包。

另运行`-swordsman-chapter-tests-check`及`python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter0304-floor-package-regression`，36幕战斗起点通过；索引34原Camp34缺失，普通营地读取继续单列。`protect.py after`验证196723文件路径/数量/哈希未变，普通包/旧预览/已有手动及正式存档保持。用户正常剧情及全部分支未验证，唯一清单保留待确认；第零～第二幕18张须待用户复验GO再制作。


### 2026-11-02 完整石拱重绘替换及AllChapters复验包

用户已确认连续重绘候选并授权替换。当前map03批准PNG SHA256为`29664dffcc571647e2f25cb0614159232698b13ec8546612e5ef05013ff90997`，与已确认候选完全一致；原生生成、提示词、两张参考和合成掩码归入`inputs/maps/chapter-03-arch-continuous-redraw/`。包内仍46项，其他45项批准PNG/元数据不变。代码仅更新`Assets/Editor/SwordsmanHd4xBuild.cs`的一处批准哈希，不改解析器、检查器或业务逻辑。旧map03/JSON保留在本批integration/rollback，等待用户实机通过后再收尾。

本次重新执行`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，Succeeded、0错误/0警告（以本次日志为准，不沿用上次9警告）；日志`Logs/chapter03-arch-continuous-development-build.log`。`work/chapter-03-arch-continuous-integration/run-checks.py`六项正常资源/第三幕真实入口及缺包坏包回退通过；`capture-runtime.py`在新包实际渲染第三幕18个攻击/技能UI选帧，重放70原版入场帧，人物Sprite与原Prefab一致，单位HP/MP/位置不变、检查不写手动存档。生成6张1280×800检查截图，助手核看进场10/15/18帧，未见此前矩形黑凸块或原灰地缝；这些是受控检查截图，不代表用户正常剧情通过。

`protect.py before/after`验证196725保护文件路径/数量/哈希不变，原版资源/人物动作、其他代码、场景/Prefab/Shader、工程配置、其他应用及已有手动/正式存档保持；其他45项批准资源与元数据、打包Bundle哈希核对通过。缺包/坏包临时克隆已清理，无新ImageGen调用、依赖或网络能力；本次未重复36幕起点回归，上一轮报告保留其历史范围。原索引34缺失Camp34限制未处理。当前唯一接入报告`Builds/HD4X/static-mvp/evidence/chapter03-arch-continuous-integration-validation.json`；用户正常剧情实机复验仍待确认，第零～第二幕18张继续未制作。


### 2026-11-02 第零～第二幕18张固定地形直接接入交付

用户确认第三、四幕及石拱本批实机没有问题，并授权第零～第二幕18张“好了后直接替换，然后我再实机验证”。第三、四幕正常剧情/固定地形/石拱本批用户验收已通过，不能推广到所有剧情分支。新18张按用户明确直接接入授权制作、检查、归档并构建，不虚构逐张离线美术GO或新实机通过；唯一清单新三工作包保持待确认，其他幕未提前生产。

内置ImageGen逐张18次调用，三幕各6张；源图逐张提供，采用对应幕已确认地图作为材质风格参考，不复制其布局。原生来源最低密度2.475倍，RGB在轮廓外留完整材质，一次下采样装配为2048×1024/PPU4；原1024×512/PPU2世界几何、Alpha、透明区RGBA、选帧/偏移保持，UI实际尺寸512×256。第一幕账册小印记及第二幕石牌字迹以窄字迹掩码保留，背景材质重绘。每项保留完整提示词、原生生成、输入、来源、PNG与源几何/哈希。评审`Builds/HD4X/static-mvp/work/chapter-00-02-fixed-terrains/index.html`含18张完整当前最终PNG、完整4096×1024原尺寸对照、局部及黑灰白底；全部右侧逐RGBA通道回读零差异。助手核看全部18完整候选与原图，并补看白底边缘/原生局部；机器通过不代替用户完整美术验收。

18张最终PNG归入approved对应原Resources路径，来源归入`inputs/fixed-terrains/stage00..02/`，既有46项批准PNG及元数据全部保持。当前AllChapters64静态纹理：46单图（其中第零～第四幕30固定地形）、14头像图集/54表情、3UI、1道具；动态资源0。`offlineApprovedTextureCount`仍46，专指此前逐张用户美术确认范围；另18张已授权直接归档接入并待用户确认，不混淆。代码仅扩展构建器/解析器精确18路径及显式Development地形检查器0..4范围，原第零幕FLOOR大写名保留，排除探针更新为尚未准入幕；不改变正常游戏/原资源/逻辑路径。

当前实时报告`Builds/HD4X/static-mvp/evidence/chapter0002-floor-integration-validation.json`：Development构建Succeeded、0错误/9警告，64项Bundle哈希与包内一致；85探针、28排除、3几何回退，30项正常/旧资源/缺包坏包检查通过。第零～第四幕各3真实攻击/技能入口×6帧，共90次UI检查通过（新增54、旧36），原版单位Sprite/HP/MP/位置不变；第三幕70原版进场帧检查也通过。生成18张1280×800真实渲染截图，助手核看新增三幕9张攻击/技能画面，地形高清和尺寸正常。检查fixture暂停自动战斗，不是用户完整剧情验收。

四场景Missing Script=0；测试台36幕入口/隔离存档及包内36合成战斗起点回归通过，原索引34缺失Camp34导致普通营地读取限制保留。第一幕旧资源检查首次内容标记通过后进程退出-11；未改代码，单独重跑内容通过且退出0，原日志及重跑日志均保留。异常根因未定位，不声称是资源/并发原因；其余当前检查正常退出。196723保护文件路径/数量/哈希保持，75项本批原地形/Prefab等源文件独立保护通过；原版人物/动作/特效、业务/数值/存档、场景/Prefab/Shader、工程配置、签名、普通/旧预览应用均未改。缺包坏包临时克隆、Unity临时导入/StreamingAssets已清理，无新依赖或运行网络能力。

执行prepare.py、prompts.py、assemble.py、verify.py、archive-and-extend.py、run-checks.py、capture-runtime.py、protect.py before/after、BuildDevelopmentPlayerChapterTestsAllChapters、ValidateScenes、测试台显式检查、verify-chapter-tests.py --package --stages 0-35 --jobs 3及git检查。本轮新增18批准PNG/元数据、来源归档、离线对照与实时检查证据；修改3个HD接入/显式检查C#及5份文档（MVP、唯一清单、current-status、verification、chapter-test-desk）。Git无本轮新增异常路径，未提交/推送。下一步交正常AllChapters供用户第零～第二幕固定地形实机验收，未擅自进入第五、六幕。


### 2026-11-02 第零～第二幕固定地形对照确认与本批收尾

用户在原版/高清对照评审后明确“确认没问题，收尾吧”。本次按对照美术确认与本批归档/接入收尾记录，18张`humanArtGo=true`，3个固定地形工作包完成；不将该确认扩展为新一轮完整剧情或全部分支实机通过。此前第一/二幕原范围及第三/四幕本批实机确认继续保留。当前64项静态纹理美术均有确认，包含第零～第四幕30张固定地形；64项AllChapters及资源PNG均未改变。下一幕盘点/制作等待用户另行授权，本次不启动第五、六幕。

完整收尾记录`Builds/HD4X/static-mvp/evidence/chapter0002-floor-closeout.json`。核对64组当前PNG/元数据哈希与唯一清单一致、145重点保护文件未变（64批准PNG、46旧元数据、30原版地形PNG、3份HD代码、当前AllChapters可执行文件与Bundle），并核对本批及统一对照页所有链接和原尺寸图。仅新18项元数据追加本轮美术确认与收尾范围，旧46项元数据保持。第零～第二幕完整候选/原尺寸对照、五幕30张统一原版/高清评审页、实际运行截图及首轮退出异常/重跑日志均保留。

已清理80个文件、75,348,519字节（约71.86 MiB），仅本批work中与原资源/inputs哈希相同的工作副本、已结束的一次性过程脚本/调度副本；精确清单见收尾报告引用的cleanup-manifest。完整原生生成图、原始输入、提示词/来源归于inputs，旧对照页原生图链接已转向长期归档，当前预览仍直接来自未变的最终PNG；不删除原版资源、approved成果、身份母版、应用或已有存档。为来源记录补充归档输入路径/哈希，原始生成路径保留历史含义。

新增本批收尾JSON、清理清单及保护快照；修改唯一清单、MVP、current-status、verification、chapter-test-desk五文档及相关新18元数据/评审索引。代码、原版资源、场景/Prefab/Shader、工程配置、玩法、人物动作、存档和运行包未修改，无新生成调用。运行当前哈希/逐通道对照检查、JSON/完成数检查、链接检查及git diff --check / git status；Debug构建未运行，因为本次仅确认归档和删除已验证重复过程文件。上轮实际构建0错误/9警告属于该轮历史证据，不当作本次新构建。第一幕自动检查一次内容通过后退出-11、独立重跑退出0的根因未定位，及原Camp34普通读取限制，均保留既有记录。未提交/推送。


### 2026-11-02 第五至第十幕统一交付，等待用户验证

当前包为 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，最终执行 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，日志 `Logs/chapter0510-development-build.log`：Succeeded、0错误/9旧警告；四场景 `SwordsmanSceneValidation.ValidateScenes` Missing Script=0。ProjectSettings构建后与原始备份逐字节相同，1280×800实际截图验证通过。Bundle130纹理与包内SHA相同，精确静态白名单191探针/25排除/3几何回退通过。

执行本批 `Builds/HD4X/static-mvp/work/chapter-05-35-production/integration/run-checks.py`：59项正常/缺包/坏包回退均退出0；第五至第十幕地图/标题/实际对话映射和固定覆盖片入口通过，第零至第十幕3真实攻击/技能入口×6帧共198次UI检查，本批108次，原版单位Sprite/HP/MP/位置不变。`capture-runtime.py`生成27张1280×800受控截图；助手看6张战场视口、18张特写中心640×400原尺寸裁图、3张RPG视口，未宣称整图每像素或正常剧情验收。首轮覆盖片fixture在pause时跳过绘制、RPG截图被选幕菜单/过场黑幕挡住，均只修正显式检查器并重建重测；原GameAnimation和正常流程不变。

运行 `python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter0510-package-regression-final`：36幕战斗起点及隔离数据回读通过；索引34原Camp34缺失，普通营地读取不算通过。`verify-delivery.py`核对130当前归档PNG/元数据、173项对照右侧逐RGBA和当前图集切片、231份成功原生输入/提示词/输出来源哈希。`protect.py after`核对196726文件路径/数量/SHA256不变，`git diff --check`通过；无新依赖或网络能力，未修改原始资源、其他代码、场景/Prefab/Shader、工程/签名/版本、普通及旧预览应用和既有存档。

代码变更仅现有 `Assets/Editor/SwordsmanHd4xBuild.cs`、`Assets/Scripts/HD4X/SwordsmanHd4xSpriteResolver.cs`、`Assets/Scripts/HD4X/SwordsmanHd4xRpgSceneCheck.cs`，新增本地生成/对照/归档和构建证据，文档更新MVP、唯一清单、current-status、verification、chapter-test-desk。未提交/推送，开始前已有工作区修改保留。证据汇总 `Builds/HD4X/static-mvp/evidence/chapter0510-integration-validation.json`；用户美术和正常剧情GO保持false，当前阶段完成后停在第十幕等待用户验证。


### 2026-11-03 第九幕地图返工后的当前包验证

本次执行 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，日志 `Logs/chapter09-map-redraw-development-build.log`：Succeeded，0错误/9旧警告。原ProjectSettings逐字节保持。`SwordsmanSceneValidation.ValidateScenes` 四场景Missing Script=0，日志 `Logs/chapter09-map-redraw-scene-validation.log`。当前Bundle与本次构建Bundle SHA一致，130项白名单仍为191探针/25排除/3几何回退。

运行 `work/chapter-09-map-redraw/run-checks.py` 共9项：正常资源smoke、stage08/09/10实际地图/标题/头像/固定剧情覆盖片检查、第九幕3入口×6帧地形回归，以及缺包/坏包的smoke和stage09回退，均退出0。第九幕1地图、1标题、21对话映射、2单帧覆盖片实际替换和几何通过，31个原单位Sprite/HP/MP/位置保持；地形18次真实UI选帧通过，检查saveWrites=0。`capture-runtime.py` 使用显式Development参数 `-swordsman-stage09-map-tour <目录>` 拍10张1280×800重叠实际视口，覆盖地图有效区；助手已逐张核看8人、柱体、岩木和地面，未再见旧人物方块或此前明显宫格色差。这些暂停检查不代表正常剧情通关。

`verify-all-delivery.py` 重核130项当前批准PNG/源哈希/元数据、173项完整对照右侧RGBA及当前图集切片；23份本轮成功ImageGen原生/引用/提示词/项目副本与原生默认路径哈希另归档。主图17份高清细节最小密度2.29倍；统一新底稿仅作构图/低频光照参考，不把底稿放大称作高清。两覆盖片原生密度3.128与2.659倍，源Alpha/未选图集RGBA/原切片世界几何保持；无可见旧人物/旧地面/旧符纹RGB回贴。其他127项批准PNG和元数据逐字节不变。

执行 `python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter09-map-redraw-package`：首轮35幕退出正常，第23幕内容标记/报告通过后进程SIGSEGV退出（系统诊断pid89878）；独立 `--stages 23 --jobs 1` 重跑报告 `chapter09-map-redraw-package-stage23-retry` 通过、退出0。有效36幕战斗起点回归通过，首轮失败报告与重跑报告均保留，根因未定位，不声称已修复退出异常。原Camp34缺失，普通营地读取不计通过。

首次新包启动在Unity日志生成前被系统终止；磁盘codesign验证有效，内核记录load code signature error2。用字节完全相同的可执行文件副本更换文件inode后可正常运行，SHA与签名内容不变，最终codesign验证有效；可能为同路径覆盖后签名缓存陈旧，仅为推测，未改签名配置。具体记录 `work/chapter-09-map-redraw/launch-repair.json`。

历史保护快照196726项路径/哈希均未改变或缺失；快照外另有18个测试存档，15个最后修改早于本轮美术盘点，3个发生在初期美术阶段，全部早于本轮构建/自动检查。这18项保留并记录当前哈希，不虚称有本轮前后哈希对照，也不因旧基线清单出现added而删除。旧保护脚本原始结果保留，当前范围说明见本轮protected-after。无原版资源、业务/存档逻辑、场景/Prefab/Shader、工程/签名、其他批准图或普通/旧预览应用改动；不新增依赖或运行网络。

代码本轮仅修改 `SwordsmanHd4xBuild.cs` 三个批准SHA和 `SwordsmanHd4xRpgSceneCheck.cs` 的显式Development地图截图巡视；新增本地新绘/对照/来源/核验，修改唯一清单、MVP、current-status、verification、chapter-test-desk及第九幕当前评审索引。当前汇总 `Builds/HD4X/static-mvp/evidence/chapter09-map-redraw-validation.json`；旧chapter0510报告不作为当前包验证。Git已有修改保留，未提交/推送。第九幕美术及正常剧情复验待用户确认，不建议在确认前进入第十一幕。


### 2026-11-03 原配色/动作8人版本（最新包）

执行 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，日志 `Logs/chapter09-people-original-build.log`：Succeeded、0错误/0警告；ProjectSettings逐字节保持。包内Bundle SHA等于本次构建Bundle，codesign验证有效。仅现有构建器2个批准SHA变化，运行解析器/Development fixture源码不变；128项其他批准PNG及元数据SHA不变。

运行本轮work中的 `run-checks.py`：smoke，stage08/09/10实际Sprite/标题/对话/覆盖片，第九幕3入口×6地形，以及缺包/坏包smoke和stage09，9项均退出0。`capture-runtime.py` 拍10张1280×800实际地图巡视，助手核看左3人、右4人和后方1人的实际视口，未见此前睁眼、多余手和棕鞋残影；不是用户美术或正常剧情通过。完整原尺寸对照右侧逐RGBA等于当前最终PNG，MAP09源Alpha保持，人物/必要邻域/旧鞋修补区外RGBA冻结；EANM_01源Alpha及未选图集区保持，EANM_02未变。`verify-all-delivery.py` 130项PNG/元数据/源哈希和173项当前对照/图集切片回读通过。13份成功内置ImageGen原生结果及来源哈希另归档。

历史保护196726项无改变/缺失；原快照外测试存档沿用上一报告的限定，不虚称完整目录无新增。未重复四场景验证和36幕起点回归：本次仅2张静态纹理及构建器哈希，没有场景或运行代码变化；此前stage23退出崩溃重跑通过、根因未知，及Camp34普通营地缺失仍是历史已知限制。临时Staging/StreamingAssets及回退克隆已清理，无新依赖或运行网络能力；Git已有修改保留，未提交/推送。当前汇总 `Builds/HD4X/static-mvp/evidence/chapter09-people-original-validation.json`；建议先由用户复验，不推进后续幕。


### 2026-11-03 第十一至第十五幕统一交付

按用户明确授权，第十一「故人」、十二「密道」、十三「真相」、十四「萤火」、十五「魔骸」在同一任务连续完成，中间不等待确认。制作、逐幕完整对照、归档、AllChapters预览接入及机器检查完成；用户逐幕美术和最后统一正常剧情实机验收待确认，相关done及用户GO仍false，不扩到16..35。

第十二幕首次实际入口检查发现原Sprite名map12与PNG文件名Map12大小写不同；已仅在现有高清解析器加载后保留原名，并为全部资源探针增加精确名称校验。第十三幕两张后续地图原本在剧情第二层，初始查看入口未载入；仅显式fixture补充原showLayer(2)/unShowLayer(2)核验与恢复，正常剧情逻辑不变。前述首次检查日志保留在本批integration/first-pass-failure，随后重建并串行重跑。最终Development构建Succeeded、0错误/9警告，以 `Logs/chapter1115-final-fixture-development-build.log` 为准；四场景Missing Script=0。77项正常资源/实际入口/缺包坏包回退检查通过；264资源探针、20排除、3错误几何回退通过；第零至十五幕288次攻击/技能UI选帧（本批90次）通过。36幕包内战斗起点/隔离数据回读通过；索引34原Camp34缺失的普通营地读取限制仍保留。图形截图未完成：系统锁屏（已读取CGSSessionScreenIsLocked=Yes），播放器初始化后停止渲染；一次超时后激活窗口仍未恢复。未修改游戏代码规避锁屏，不宣称实际GPU画面通过，运行画面留给用户末尾统一实机验收。77项检查为无界面资源/几何/真实入口调用与回退检查，不能代替正常剧情及图形验收。196744保护文件路径/数量/SHA256保持。证据汇总 `Builds/HD4X/static-mvp/evidence/chapter1115-integration-validation.json`。

执行本批 `collect-origins.py`、`assemble-maps.py`、`assemble-floors.py`、`assemble-portraits.py`、`review-stage.py`、`archive-stage.py`、`integration/verify-delivery.py`、`integration/run-checks.py`、`integration/capture-runtime.py`、`protect.py after`，Unity `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`、`SwordsmanSceneValidation.ValidateScenes`，`python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter1115-package` 与 `git diff --check`。只修改现有3个HD构建/解析/显式检查C#及本批5文档，新增本地来源/成果/对照/验证产物；开始前已有工作区修改保留，不提交或推送。


### 2026-11-03 静态背景人形准则与第十五幕8处补绘

本轮统一AllChapters Development构建Succeeded、0错误/0警告，日志 `Logs/chapter1115-background-people-development-build.log`；四场景Missing Script=0。`run-validation.py`七项定向检查通过（正常资源探针、第十五幕实际入口、固定地形、缺包/坏包各资源与场景回退），264探针/20排除/3几何回退通过；应用codesign深度严格校验通过。成功采集1张1280×800受控GPU帧，助手观察仅为初始墙面/符纸及对话头像视口，不覆盖8处棺内人物，不能当成这些人物或正常剧情的实机验收。本轮没有重复36幕战斗起点回归；上一批36幕结果仅保留其历史范围。索引34原Camp34缺失限制不变。

`verify.py`通过183当前批准纹理/来源/元数据哈希、190项对照高清侧回读、201份原生来源哈希、8局部回读和专项链接检查；`protect.py after`确认196744保护文件路径/数量/SHA不变；`git diff --check`通过。仅修改Map15高清PNG及元数据、现有高清构建器一个批准SHA，更新6份规范/清单/状态/验证/测试台文档；解析器和显式fixture代码、原资源、独立人物动作、业务/存档、场景/Prefab/Shader、工程配置/签名/版本及其他应用保持。开始前已有工作区修改保留，未提交/推送。当前证据 `Builds/HD4X/static-mvp/evidence/chapter1115-background-people-validation.json`。


### 2026-11-03 第九幕8人周围地面统一（当前版本）

当前地面统一版本执行SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters，Logs/chapter09-ground-unify-build.log记录Succeeded、0错误/0警告。只修改MAP09 PNG及元数据和构建器1个批准哈希；源Alpha和手工人物保护轮廓RGBA逐通道保持，完整原尺寸对照高清侧逐通道回读相同。work/chapter-09-ground-unify/run-checks.py的9项smoke/stage08、09、10入口、第九幕地形、缺包坏包回退均退出0；capture-runtime.py生成10张1280×800巡视，助手核看左右和后方覆盖8人的实际画面。包内Bundle SHA等于本次构建Bundle。未重复36幕起点/全场景与后续幕渲染回归；不把既有报告当本轮实时结果。原Camp34与历史退出异常不在本次修复范围。原版资源、运行逻辑、存档、场景/Prefab/Shader及工程签名未修改，无新依赖或运行网络；用户美术/正常剧情GO仍false。当前证据Builds/HD4X/static-mvp/evidence/chapter09-ground-unify-validation.json。




### 2026-11-03 第十六至第二十幕统一交付

第十六「战阵」、十七「祭天塔」、十八「千魔啸夜」、十九「飞天」、二十「埋伏」按本次授权连续完成，中间不等待确认。本批Development构建Succeeded、0错误/9项既有警告，日志 `Logs/chapter1620-fixture-development-build.log`。其后第九幕同期任务重建兼容233纹理统一包，0错误/0警告，日志 `Logs/chapter09-joint-paint-build.log`；现将该包冻结为 `Swordsman-ChapterTests-16-20.app` 并重新运行本批105项、36幕及图形检查，不覆盖共享AllChapters。首次构建发现第十七幕原PPU=1.8348485与旧构建器统一PPU4假设不符；仅现有构建器改为按原Sprite PPU×2配置/验证，高清PPU3.669697，原世界尺寸及原版.meta保持。第十九幕Floor_19_t4原图1024×256，显式地形检查器改为从原图推导高清尺寸/PPU，保持2048×512纹理与512×128 UI，不改运行逻辑。首轮失败与关闭时Mono异常日志保留，最终结果以重建日志为准。四场景Missing Script=0；105项资源/实际入口/旧幕回归/缺包坏包检查、321资源探针/18排除/3几何回退通过；378次攻击/技能UI选帧（本批90）通过；36幕包内战斗起点/隔离数据回读通过。索引34原Camp34普通营地读取缺失限制保留。保护快照196744文件路径/数量/哈希保持；codesign深度严格校验通过。图形截图共22张，具体覆盖和限制见 `Builds/HD4X/static-mvp/work/chapter-16-20-production/integration/rendered-runtime-checks.json`，不等于正常剧情或全部背景人物实机确认。总报告 `Builds/HD4X/static-mvp/evidence/chapter1620-integration-validation.json`。

执行离线盘点/生成/装配/逐幕评审/归档、`integration/verify-delivery.py`、`integration/run-checks.py`、`integration/capture-runtime.py`、`protect.py before/after`，Unity `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`、`SwordsmanSceneValidation.ValidateScenes`，`python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter1620-package`，`git diff --check`及`git status --short --branch`。原版资源/独立人物/动作、玩法/数值/存档、场景/Prefab/Shader、ProjectSettings/Packages、签名/版本设置、普通与旧预览应用未改；无新增依赖或运行网络功能。


### 2026-11-03 第九幕8人与地图一体绘景（最新）

执行SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters，Logs/chapter09-joint-paint-build.log记录Succeeded、0错误/0警告。包内Bundle SHA等于本次构建Bundle，codesign验证有效。现有构建器仅2个批准SHA变化，无运行代码修改。4份内置ImageGen完整绘景原生与默认保存路径哈希相同，来源/提示词见work/chapter-09-joint-paint/provenance.json。地图组最小原生密度3.879倍，每组整框一次下采样，仅场景外缘拼接；未做人物抠图/旧边缘恢复/独立阴影，整组掩码外RGBA逐像素相同，源Alpha/世界几何、固定剧情帧未选图集RGBA保持。233项运行白名单通过321探针/18排除/3几何回退；归档时231项其他批准图/元数据未变，收尾工作区含其他并行任务新增候选，242项批准PNG/源哈希与173项第五～第十幕当前完整对照高清侧回读通过，不把242项等同本包233项。run-checks.py的9项smoke、stage08/09/10真实地图/标题/头像/固定覆盖片、第九幕地形及缺包坏包回退均退出0。capture-runtime.py生成10张1280×800受控巡视，助手核看左右/后方覆盖8人的实际画面及所有3份原生绘景/完整剧情帧。196744历史保护文件无改变或缺失。未重复36幕起点/全场景及后续幕渲染回归；原Camp34和此前退出异常不在本次修复范围。临时本轮回退克隆已清理，无新依赖或运行网络能力，用户美术与正常剧情GO仍false。当前汇总Builds/HD4X/static-mvp/evidence/chapter09-joint-paint-validation.json。


### 2026-11-03 第二十一至第二十五幕统一接入验证

本次实时执行Unity `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，日志 `Logs/chapter2125-development-build.log`：Succeeded，0错误/9警告；同名AllChapters更新至288纹理。`SwordsmanSceneValidation.ValidateScenes`验证4场景Missing Script=0，日志 `Logs/chapter2125-scene-validation.log`。

`work/chapter-21-25-production/integration/verify-delivery.py`通过288当前归档、155项完整对照高清侧/图集切片回读、242原生输出/提示词/输入哈希；`integration/run-checks.py`的45项资源/6幕实际引用/6幕地形/旧入口/缺包坏包检查均退出0。378资源探针、18排除、3错误几何回退及本批108次真实攻击/技能/地图技能UI选帧通过，人物保持原Sprite、状态保持，检查入口saveWrites=0。`integration/capture-runtime.py`的12项图形检查通过并产生24张1280×800截图。助手核看24帧布局和stage21场景、stage25技能两帧100%细节；这些固定镜头不覆盖整地图，stage21起始视口含原黑区，不等于正常剧情或用户美术验收。

`protect.py after`原始严格路径集合检查为false，原因仅为ChapterTests/v2内新增20个测试快照文件；196744已有文件SHA保持、无缺失。分类结果 `protected-classified.json`与原始结果同时保留，不将新增测试数据说成已有存档变化。第九幕两个同期批准素材外部变化保留。执行codesign深度严格校验、`git diff --check`及`git status --short --branch`；临时导入目录和回退克隆已清理。未重复36幕战斗起点完整回归；此前记录只代表其历史范围，索引34原Camp34缺失和历史退出异常仍保留。总报告 `Builds/HD4X/static-mvp/evidence/chapter2125-integration-validation.json`。用户美术/正常剧情实机待确认。


### 2026-11-03 第十幕离线地图待确认

岩壁接缝重绘完整候选与原尺寸对照已输出；详情见[静态美术记录](static-art-mvp.md#2026-11-03-第十幕岩壁接缝离线候选未接入)。入口 `Builds/HD4X/static-mvp/work/chapter-10-cliff-offline/index.html`。本轮未替换、未调用Unity；Debug构建未运行，遵照用户先看图要求。用户美术/几何及实机验收待确认。


### 2026-11-03 第十幕岩壁重绘测试包

已按用户整图确认替换MAP10并重新构建 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，Succeeded，0错误/0警告。日志 `Logs/chapter10-cliff-approved-build.log`。按用户要求未启动游戏或执行额外运行验证，用户自行选择第十幕实机检查。变更范围及交付证据见[静态美术记录](static-art-mvp.md#2026-11-03-第十幕整图确认后接入与打包)。


### 2026-11-03 索引27至30实时验证

本轮执行 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`：Succeeded，0错误/9警告，日志 `Logs/chapter2730-development-build.log`；执行 `SwordsmanSceneValidation.ValidateScenes`：4场景、Missing Script=0，日志 `Logs/chapter2730-scene-validation.log`。当前AllChapters包含326静态纹理，普通/旧预览应用未由本轮构建。

`work/chapter-27-30-production/integration/verify-delivery.py`验证326当前源/归档哈希、123完整对照高清侧/图集切片、116原生图/提示词/输入来源及生成重叠区校正记录；286既有PNG/元数据不变，2旧图集仅新增切片区域变化。`run-checks.py`共11项通过：静态探针、本批4幕Floor、4幕实际地图/标题/头像/覆盖片入口、缺包及坏包回退；418静态资源探针、17排除探针。本批受控检查仅扩展Development显式入口，28/29后续背景层按原序列化ID显式显示后恢复；不改正常剧情。`capture-runtime.py`的8项图形检查通过，16张实际1280×800截图归档于 `Builds/HD4X/static-mvp/evidence/chapter2730-runtime/`。受控入口和固定视口不能证明全部地图位置、剧情分支或用户美术通过。

原始 `protect.py after` 与分类 `protected-classified.json`同时保留：196764已有保护文件SHA不变、无缺失，新增隔离ChapterTests快照0个单列。执行codesign深度严格校验及git差异检查；构建/图片/日志不入Git。用户逐幕美术与最后正常剧情实机GO仍待确认，统一报告 `Builds/HD4X/static-mvp/evidence/chapter2730-integration-validation.json`。


### 2026-11-03 RPG-Map02修正版测试包

已按用户确认替换RPG-map02并重新编译AllChapters应用，Succeeded，0错误/0警告。日志 `Logs/rpg-map02-continuous-fix-build.log`。按用户要求未启动游戏、未执行额外运行验证，实机由用户稍后统一检查。固定OLD/重绘对比图路径不变；范围与交付记录见[静态美术记录](static-art-mvp.md#2026-11-03-rpg-map02确认后替换与打包)。


### 2026-11-04 索引31至35实时验证

执行本批`collect-origins.py`、装配/逐幕对照/归档及`integration/verify-delivery.py`：376当前源与归档哈希、148完整对照高清侧/当前图集切片、166原生来源/实际提示词/输入哈希通过；321份既有PNG/元数据不变，3旧图集仅新增切片区域变化，RPG-map26与Map08同期外部更新保留并单列。特别幕原印字保护掩码与10个人形矩形交集为0，人形/地面有专项原尺寸对照。

Unity `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`：Succeeded，0错误/9警告，日志 `Logs/chapter3135-development-build.log`。`SwordsmanSceneValidation.ValidateScenes`：4场景Missing Script=0，日志 `Logs/chapter3135-scene-validation.log`。当前AllChapters共376静态纹理，216张固定地形均准入；源码已配置472资源探针/15排除探针，本轮未启动Player运行它们，未作运行截图或正常剧情验证，最后实机由用户统一执行。包内Bundle与本次构建输出SHA相同，codesign深度严格校验通过。

`protect.py after`：196767个已有保护文件SHA保持、无缺失；新增路径0个在原始报告单列。其他普通/旧预览应用957文件SHA保持。执行`git diff --check`及最终`git status --short --branch`；本轮制作/打包完成，用户美术/正常剧情GO与相关done仍false。完整证据 `Builds/HD4X/static-mvp/evidence/chapter3135-integration-validation.json`。


### 2026-11-04 FACE02_t0愠色确认与统一包替换

用户确认“没问题，go吧，归档并替换吧”。本轮确认范围是FACE02_t0当前归档愠色表情，原图愠色、当前收眉且收平嘴角，采用已归档版本，不新增生成或重绘其他表情。同次把此前已确认的RPG-map11「鐵／華佗藥鋪」修字版同步进包。Map11-1仍保留最初第一版高清，柔和云层候选不采用；现役云PNG及元数据SHA不变。

沿用最新已扩展到376纹理的白名单和AllChapters，不回退其他批次成果。本次376组PNG/元数据与构建前锁定值一致，FACE02_t0切片与用户确认PNG逐RGBA一致，RPG-map11与确认修字图SHA一致；190项11..15评审对照回读通过。

当前 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app` 已更新；本轮实时Development构建Succeeded、0错误/0警告，日志 `Logs/face02-t0-approved-development-build.log`，四场景Missing Script=0。9项资源/实际场景/回退检查通过，覆盖472资源探针、15排除、3错误几何回退、第十一至十五幕真实入口中的头像、共享map11 RPG入口、缺包及坏包回退；codesign深度严格校验通过。196767个原版资源/配置/其他代码/已有存档及普通与旧预览应用保护文件未变。本轮未做用户正常剧情或目标表情的GPU逐帧视觉验收，最终实机待用户；索引34原Camp34缺失限制不变。

仅补充FACE02图集元数据中的本轮确认记录，更新5份相关文档/清单与评审状态，并替换统一包；本轮图集PNG、其他图像、业务/解析/显式检查代码、工程配置、签名版本均未改。运行protect.py before/after、run-checks.py、归档哈希/切片/对照回读检查、SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters、SwordsmanSceneValidation.ValidateScenes、codesign --verify --deep --strict、git diff --check。已有工作区修改保留，无新增异常Git路径，未提交/推送。证据 `Builds/HD4X/static-mvp/evidence/face02-t0-approved-integration-20261104.json`。

### 2026-11-09 静态 MVP 完整实机测试包

构建入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`。Debug 成功，0 错误、9 警告。包内资源 Bundle SHA256 与构建输出一致，`codesign --verify --deep --strict` 通过。静态自检通过 472 个既有探针及 256 个图集切片替换探针，覆盖本批108项补充切片。标题页 `Title_t1` 实际 Image 绑定、第一幕与索引13受控场景、临时克隆缺包回退均通过。`SwordsmanSceneValidation.ValidateScenes` 四场景 Missing Script 为 0。`ProjectSettings` 与 `Packages` 构建前后哈希一致，临时导入已清理。

测试包默认窗口 `1280 × 800`，内含36个选幕预设。受控检查与用户实机验收分别记录。日志 `Logs/mvp-full-device-build-20261109.log`，完整证据 `Builds/HD4X/static-mvp/evidence/mvp-full-device-build-20261109.json`。

### 2026-11-09 Map16 战损版归档与 Debug 接入

用户确认采用战损版。Map16 已归档并进入全章节 Debug 测试包。确认范围包含 21 名细化像素站兵、7 具带衣甲战损和脸部或帽沿血迹的倒地士兵，以及倒地 06 的腿部比例调整。归档、Unity 暂存和候选 PNG 哈希一致，尺寸、透明度及 Sprite 几何保持。仅更新高清 Map16、元数据、构建器对应哈希和交付记录。

执行 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，成功，0 错误、0 警告。包内 Bundle 与构建输出哈希一致，`codesign --verify --deep --strict` 通过。386 项静态纹理自检与第十六幕资源查看入口通过，实际场景加载高清 Map16，受控检查写入存档 0 次。用户正常剧情与实机美术验收待确认。证据 `Builds/HD4X/static-mvp/work/chapter-16-environment/map16-runtime-guard-full-20261109/battle-wear/archive-integration.json`。

2026-11-09 用户确认沿用当前 Debug 包内 Map16 战损版，Map16 实机美术记为 GO。倒地 06 单独重绘候选不采用。核对归档 PNG、包内 Bundle 及可执行文件哈希，未重建 Debug。整幕剧情验收独立记录。

2026-11-09 Map16 战损版的 OLD／当前高清对照已同步至第十六幕评审与统一评审门户。完整对照、7 处倒地人物、队列与兵器对照的两侧均逐像素核对，18 处高清局部与归档 PNG 一致。原版、归档 PNG 和 Debug 包哈希保持。证据 `Builds/HD4X/static-mvp/work/chapter-16-environment/map16-runtime-guard-full-20261109/battle-wear/comparison-refresh.json`。


### 2026-11-11 Map11美术GO归档与统一包替换

Map11当前完整修订71b44d485093已获用户美术GO，更新approved及构建器对应批准SHA。AllChapters Development构建Succeeded，0错误/0警告；四场景Missing Script=0；452资源自检及第十一幕实际加载检查通过（2地图、1标题、15头像、41原版单位，saveWrites=0）。包内Bundle与构建输出SHA一致，codesign --verify --deep --strict通过；198237保护文件无变化。日志为Logs/map11-approved-development-build-20261111.log、map11-approved-scene-validation-20261111.log、map11-approved-static-smoke-20261111.log、map11-approved-stage11-check-20261111.log。完整证据见Builds/HD4X/static-mvp/work/map11-two-junctions-20261111/archive-integration.json。用户需退出旧测试进程后打开更新应用，正常剧情与实机美术待确认。


### 2026-11-11 Map11用户实机验证GO

用户确认“真机验证了，没问题”。记录当前Map11完整修订`71b44d485093`在已交付全章节测试包中的实机美术与资源替换验证GO；仅限本次Map11，不扩展为整幕/全剧情验收。已同步归档元数据、唯一资源清单、统一评审及原交付证据。此轮只收尾记录，PNG及应用可执行文件哈希与交付值一致，未重新构建或重跑游戏；此前构建/机器检查结果仍为历史证据，本次实机GO来自用户明确确认。证据：`Builds/HD4X/static-mvp/work/map11-two-junctions-20261111/archive-integration.json`。

## 2026-11-11 测试辅助、技能分页与宝物闪光接入

已更新AllChapters测试包：三列选幕、七个默认关闭的分组Mock开关及12张已获GO的宝物闪光切片。正式代码同时支持技能数组扩展、战斗技能菜单分页（Tab/按钮）及普通/战斗档可选技能扩展；旧10槽核心与文件长度不变，旧版本覆盖新档会丢弃额外技能，详见[架构](architecture.md)与[测试台](chapter-test-desk.md)。旧档此前已丢失且未记录的技能不会自动补发。

实时Development构建Succeeded、0错误/9警告，日志 `Logs/test-desk-mocks-treasure-20261111-final-package-build.log`。执行构建入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`、`-swordsman-static-mvp-check`、`-swordsman-chapter-tests-check`、`-swordsman-chapter-tests-mock-check`、`python3 scripts/verify-chapter-tests.py --package --stages 0-35 --jobs 3`、`SwordsmanSceneValidation.ValidateScenes`、`codesign --verify --deep --strict`及保护文件哈希检查：12项Mock、36幕包内快照、旧档/损坏快照回归、472资源/650图集探针、四场景Missing Script=0均通过。Bundle464纹理，与包内SHA一致。证据 `Builds/HD4X/static-mvp/work/test-desk-mocks-20261111/delivery-verification.json`。

新增Mock实现/专项检查与技能扩展保存辅助；修改测试台、会话隔离、战斗技能UI/单位保护、宝物显示及高清构建解析。原图集/Sprite/Prefab/Shader、场景/预设、ProjectSettings/Packages、签名版本和正式原存档目录未改，未新增依赖/网络功能，未提交或推送。窗口三列及分组已观察；鼠标操作、最终美术及完整剧情待用户验收，原camp34缺失问题保留。


### 2026-11-11 透明底全系列GO、漏接入彻查与补包

用户确认之前透明底修订全系列美术GO，要求先核查再入包。已核对882评审项、464构建资产及实际包基线，查出25帧透明底实际漏接入、12项宝物闪光状态落后、27项离线说明未更新，另修复当前运行包汇总停留386输入的过期记录。只将缺失25帧合入最新MSG_BOX图集，保留已接入14帧箭头及底板。全章节Development新包成功，0错误/0警告；包内SHA、464资源自检、第一幕资源场景、四场景及198449保护文件检查通过，存档写入0；透明纸面合成检查通过，用户最终实机视觉待确认。明确仅离线的174项及后续制作/延期资源未扩围入包。详细范围、逐项证据及日志见[修订与运行包复核](revision-runtime-audit-20261111.md)。


### 2026-11-11 透明底全系列用户实机GO

用户确认“可以，实机验证GO”。MSG_BOX_t1～t39透明底修订（本次补入25帧、保留14帧）完成美术、归档、接入及用户实机验收。验收范围仅限本次透明底系列，不扩展其他候选或全剧情；本轮仅同步状态和证据，PNG与应用未改、未构建、未重跑游戏。本次实机结果来自用户明确确认。详细证据见[修订与运行包复核](revision-runtime-audit-20261111.md)。


### 2026-11-12 标题补边GO与满屏适配

统一AllChapters包已更新。仅切换已获GO的Title图集，并在GameTitleUI扩展背景运行时采样及高度；字标纹素显示比例和位置保持，中央菜单未改，原版Sprite/Scene/Prefab及工程配置不编辑。完整状态与归档维护于[静态美术MVP](static-art-mvp.md)及唯一资源清单，证据为 `Logs/title-viewport-integration-20261112/delivery.json`。

运行了四场景检查、AllChapters Development构建、包内静态资源自检、codesign深度严格校验、包内外Bundle SHA256、原始标题/Sprite/四场景/ProjectSettings/Packages保护哈希、实际主菜单截图配准及git差异检查。构建0错误/9个既有警告，四场景Missing Script=0，464资源与650图集探针通过；实际截图已铺满，上下无整条黑边，三处配准位移0。用户美术GO已登记，当前运行版本的用户实机验收仍待确认。

### 2026-11-12 移除云存档并更新统一测试包

产品明确不做云同步。本阶段完成删除标题/营地云存档入口、左右切换、云面板与确认流程、云槽位缓存/回调/读写分支、`SWORD_CLOUD` 和云文案。只保留本地存读档，路径、槽位、3000字节普通档字段顺序、技能扩展及战斗临时档格式不变。其他历史调试上传/下载、激活、日志与支付未扩展或启用，不宣称整个项目已完全离线。

修改文件：`GameTitleInput.cs`、`GameCampInput.cs`、`GameSLUI.cs`、`GameUserData.cs`、`GamePHP.cs`、`GameStringData.cs`、`SwordsmanChapterTestDesk.cs`、`SwordsmanChapterTestChecks.cs`、`SwordsmanChapterMockChecks.cs`；删除 `GameSLCloudUI.cs` 及其 `.meta`。`level0.unity`、`level2.unity` 各移除59个云UI对象（289个序列化对象块）。编辑器额外重序列化已还原，其他对象保持原内容；标题场景另移除云缓存字段，并清理 `GameStringData` 中实际运行的简繁文案表。该文案表不会自动调用源码的 `load()` 更新，必须同时校验序列化数据，不能只搜索源码或未解码的YAML。未新增永久源码、依赖或工程配置；玩法、资源图、签名配置和旧档未改。架构、现状及安全文档同步更新。

初次构建0错误/9个既有警告；实际GUI发现序列化旧文案后修正，最终Development构建0错误/0警告，仍更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。最终构建沿用现有入口及 `-swordsman-reuse-hd-bundle-sha256`，锁定刚验证的Bundle `123e7cb28423b2eb241ccd47f3031301854712418931be6d889fa822c08f69c5`，包内哈希一致。原交付包备份于 `Builds/macOS/archive-20261112-before-cloud-removal/`，不是新验收入口。

执行并通过：源码/文档差异检查；保留场景对象逐块比较；包含Unicode转义解码的源码/场景/Prefab云引用清零检查；新旧包程序集符号对比；`SwordsmanSceneValidation.ValidateScenes`（4场景、Missing Script=0）；最终包 `-swordsman-chapter-tests-check`（36关入口、本地普通档往返、战斗快照重载、坏档/错误预设拒绝、真实旧档只读副本加载）；首轮包第一幕 `scripts/verify-chapter-tests.py --package --stages 1 --jobs 1`；同Bundle静态资源自检（464资源、650图集探针）；最终 `codesign --verify --deep --strict`。94个原有存档前后校验值相同。最终GUI已确认标题为“前历再续”，按向右键仍停留本地列表。场景Git文本差异中标出的部分尾随空格来自未变对象的重复文本匹配，未为消除提示改动无关序列化内容；逐块比较为准。

证据统一在 `Logs/cloud-save-removal-20261112/`，入口为 `delivery.json`，最终日志为 `build-final.log`、`final-scenes.log`、`final-local-save-regression.json`，GUI为 `final-local-load.jpg`。当前实现与打包完成，用户实机验收待确认；未提交或推送，保留原有工作区修改。

### 2026-11-12 本地读档退出与标题菜单转场修复

用户报告读档标题在退出后残留，以及返回标题时文字先于菜单背景出现。实际场景确认：读档标题与4个菜单文字均带 `Shadow`，且 `useGraphicAlpha=false`；只修改 `Text.color.a` 不能使这些独立投影淡出。首轮组件透明度检查虽然通过，实际渲染截图仍能看到投影，因此不能把组件属性归零当作视觉验收。

本次修改 `Assets/Scripts/UI/GameUIFade.cs`：文字及其投影通过同一 `CanvasRenderer` 淡入淡出，缓存并恢复原有渲染透明度，保留原始选中/未选中颜色；描边分支避免对不继承Graphic透明度的文字描边重复叠乘。位移和透明度使用同一钳制进度，结束帧先写入真正终态，再回调；覆盖零时长和终点状态恢复。修改 `Assets/Scripts/Title/GameTitleInput.cs`：返回菜单与黑幕淡出同步启动，不先暂停菜单；入场期间阻止重复输入中断动画，存读档关闭标志在启动淡出前设定。

新增 `Assets/Scripts/ChapterTests/SwordsmanTitleTransitionChecks.cs` 及Unity生成的 `.meta`，只在Development章节测试包且显式带 `-swordsman-title-transition-check` 时运行。测试走实际标题/读档回调，连续3次进入退出，逐帧检查菜单文字有效透明度与背景一致、退出等待阶段文字渲染透明度为0、菜单位置无漂移；同时覆盖5处独立投影、提前取消、零时长终点回调及原渲染透明度恢复。可带 `-swordsman-title-transition-capture` 输出真实GPU转场截图。检查不写游戏存档，正常启动不执行。

统一验收包仍为 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。本任务先执行Development构建；之后另一个任务更新同包资源并复查构建，本任务等待其完成，对最后统一包重新核验程序集、运行转场/本地存读档检查及深度严格签名检查，未用较早包结果替代最终包。最终构建日志为 `Logs/omissions-allchapters-recheck-build-20261112.log`，Succeeded、0错误/9个既有警告。最终转场报告及截图位于 `Logs/title-load-transition-20261112/renderer-check/`，`errors=[]`，退出截图无残字，返回截图文字与菜单背景同时淡入。普通存档往返、战斗快照重载、真实旧档只读副本加载、36关资源入口检查通过；94个已有存档校验值不变。签名检查和源码差异检查通过，正常启动主菜单已打开供用户复验。

最终另行执行 `SwordsmanSceneValidation.ValidateScenes`，4个场景Missing Script=0，日志为 `Logs/title-load-transition-20261112/scenes.log`。此前被其他构建占用而中止的尝试不计为通过。

本任务不修改美术、场景、布局、存档格式、玩法、工程或签名配置；保留并行任务及已有工作区修改。构建前包备份于 `Builds/macOS/archive-20261112-before-title-transition/`。交付证据入口为 `Logs/title-load-transition-20261112/delivery.json`，最终包程序集校验及检查结果为 `verified-package.json`；用户实机验收待确认，未提交或推送。

## 2026-11-12 启动 Logo 隔离原型

新增可选启动计时与文字Logo承接层，保留原开场动画；实际Editor Play Mode覆盖视频准备、首内容帧揭示、提前跳过和缺片回调。默认启动不启用。独立应用冷启动/黑屏削减量和Unity Logo零闪现尚未验证，未执行现有许可标记后处理，未构建或覆盖验收包。详情与证据口径见[启动原型](startup-logo-prototype.md)。

## 2026-11-12 人物巨剑启动效果包

用户确认的抠像人物巨剑动画已接入独立 `Builds/macOS/Swordsman-StartupPreview.app`，1280×800。副标题保持文字，加载点态每0.4秒循环且固定起点；先展示约6.4秒，再完整播放原开场。Development构建0错误/9警告，实际Player检查通过人物循环、三种点态、原开场首内容帧和结束回调，签名校验通过。现有AllChapters、原视频、场景和工程配置未改；使用官方构建，不调用许可标记改写后处理，故可能显示Unity自带启动标识。已知抠像白晕保留，待用户视觉验收。当前权威说明及文件范围见[启动原型](startup-logo-prototype.md)，本轮证据 `Logs/startup-artwork-integration-20261112/delivery.json`。


## 2026-11-14 设置页与虚拟键盘专项验证

唯一构建入口仍为`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，不生成其他应用。证据目录：`Logs/settings-virtual-keyboard-20261114/`。

新增`SwordsmanVirtualKeyboardChecks`仅在Development章节测试包显式传入`-swordsman-virtual-keyboard-report <绝对JSON路径>`时启用。检查通过真实EasyTouch指针处理器触发四方向及A/B，检查长按/松开、显示/隐藏、缩放上下限、设置重载；结束恢复所有触及偏好（包含原来不存在的键），不保存游戏进度。窗口实测另覆盖鼠标点击与实体键盘共用、跨场景和重启；自动指针处理器检查不能代替操作系统鼠标实际事件检查。

Windows、iOS与Android默认值仅能从平台判断确认；本轮不以Mac测试替代这些平台的构建或触摸实机验收。构建、专项报告、截图、存档及配置保护结果以该目录最终delivery.json为准；未有最终报告前不视为验收通过。


最终结果：唯一AllChapters Development包构建Succeeded、0错误/9个既有警告，四场景Missing Script=0，codesign深度严格校验通过。最终包18项专项检查全部通过，涵盖旧Input鼠标模块启用、方向键按下/长按/松开、A确认/B取消、开关重载和缩放上下限。333个受保护存档、其他场景及配置校验值保持；标题场景74个退役序列化块删除、23个新增块，其他变化均限定设置子树和语言表，2处无关重序列化已还原。

Mac实际鼠标操作来自用户明确确认“可以正常操作”，不能标成助手自动鼠标检查通过：CUA坐标点击工具持续报noWindowsAvailable，助手已区分此工具限制与游戏组件检查。用户已用最终包的虚拟方向键和A/B验证菜单移动、确认、返回；虚拟键盘保持打开供继续调试。Windows/iOS/Android未做本轮构建或实机验证。未保存或覆盖游戏存档，未提交/推送。


### 2026-11-14 虚拟键盘单项切换验证

最终唯一AllChapters包构建Succeeded，0错误/9个既有警告，签名深度严格校验通过。`SwordsmanVirtualKeyboardChecks`增加单项结构、虚拟A关闭、关闭文案、Confirm重新打开检查，最终22项全部通过。专项通过真实ETCButton的指针处理器触发A，再经过GameTouchManager/GameInputManager与标题输入回调，不将它描述为操作系统鼠标点击实测。另在实际Player窗口通过Return完成打开→关闭→打开，截图确认只有一个当前值、焦点不丢失、键盘显示同步；原生自动鼠标点击A未观察到切换，不作为通过证据。其他输入链路仍以专项检查为准。

本轮未改难度规则、存档、其他界面和工程配置；原有存档及所有场景/工程配置的334个保护文件校验值不变。更新了专项检查，未新增源码文件；原有工作区状态保留，未提交。证据：`Logs/keyboard-single-toggle-20261114/delivery.json`、`check.json`、`return-off.png`、`return-on.png`；开关保持打开供调试。
## 2026-11-14 虚拟键盘皮肤：已接入、Editor验证，暂不打包

本阶段完成；新增`GameVirtualKeyboardSkin.cs`、`SwordsmanVirtualKeyboardSkinValidation.cs`及Unity生成的.meta，新增`Assets/Resources/VirtualKeyboard/Bronze/`三张PNG及.meta；修改`GameTouchLeftUI.cs`/`GameTouchRightUI.cs`接入初始化，并更新架构、本文及静态资源清单。设计输入、准确提示词、Alpha处理记录与深浅背景对照保留在`Builds/HD4X/static-mvp/work/virtual-controls-skins-20261114/`。使用内置ImageGen制作透明素材，再执行用户授权的切图及Alpha分层，没有把游戏截图背景烘焙进素材。

执行：`SwordsmanVirtualKeyboardSkinValidation.ConfigureAndValidate`（batchmode/nographics/quit）与`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode`（图形Editor、不带quit），分别对应`Logs/virtual-keyboard-bronze-20261114/import-scenes-final.log`与`editor-playmode-final.log`。前者完成三图RGBA32/FullRect/无mipmap/NPOT不缩放导入及四场景验证，Missing Script=0，最终无脚本编译错误；最初验证器使用了不适用于此Unity版本的importer属性，已改用TextureImporterSettings并重新通过。后者47项通过，实际EasyTouch指针处理器覆盖四方向、中心、松开、A/B确认/取消、皮肤加载与按下图、原点击尺寸、A/B位置、150%边界及开关/禁用尺寸控件。此处不是操作系统鼠标自动点击实测。

`editor-settings.png`、`editor-settings-150.png`均为1280×800的Editor实际GPU截图，已查看：底面透出背景，金属与字形可辨认，A左下/B右上。验证临时Game View尺寸预设已恢复并删除；测试直接调整内存字段，无设置保存或游戏存档写入。最初固定分辨率断言误用了Editor回调上下文中的Screen尺寸，已改为读取Game View并重新运行通过，最终PNG尺寸另行核验。145份场景、插件、工程配置及现有应用关键文件哈希前后相同。运行中的用户旧Player未关闭。

Debug/Development Player构建：**未运行**，用户已要求后续优化完成后统一打包；唯一应用未更新。旧档格式、玩法、难度、剧情、C/D及原始Sprite资源未改。当前未解决项仅为统一构建后新皮肤的Player原生鼠标和实机美术复验，不以Editor结果替代。Git保留原有修改，无额外应用、提交或推送；Resources及Builds继续按原策略本地保存。建议等用户下一批修改或构建指令，届时仍用唯一构建入口。
## 2026-11-14 虚拟键盘皮肤与关闭禁用尺寸项已入唯一包

用户确认退出并授权统一构建，覆盖前文暂不打包状态。通过唯一`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`直接更新`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`；复用锁定已验证高清Bundle SHA256 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`。构建Succeeded，0错误/9个既有警告，包内Bundle哈希相同；codesign深度严格检查通过，1819资源/472探针/2005图集探针通过。

本阶段仅扩展`SwordsmanVirtualKeyboardChecks.cs`显式测试入口：检查包内三张RGBA32皮肤、DPad及A/B实际引用、原点击尺寸、A左下/B右上并向右下靠齐，增加可选`-swordsman-virtual-keyboard-capture <目录>`截图参数；普通启动不自动运行。最终Player以`-swordsman-virtual-keyboard-report <报告>`执行33项检查，errors=[]，含虚拟A单项切换、A确认/B取消、四方向、按下皮肤、100%/150%、关闭后缩放按钮禁用/回调不改尺寸/导航跳过/重开恢复及偏好恢复。未改玩法、存档、场景、插件或工程配置，未新增源码文件或额外应用。

已查看最终Player的打开/150%/关闭截图，确认底面透出实际游戏背景、中央金属印章与A/B位置正确，关闭时尺寸一行置灰；不能把指针处理器测试称为操作系统鼠标自动点击或用户验收。603个受保护存档、皮肤/插件、场景及配置文件哈希保持，未写游戏进度，既有Git路径状态保持，无提交/推送。用户实机视觉与鼠标手感仍待确认，建议现在直接验收唯一应用。证据入口`Logs/virtual-keyboard-bronze-20261114/player/delivery.json`，构建日志`build.log`、专项`check.json`、实际截图`captures/keyboard-on.png`、`keyboard-150.png`、`keyboard-off.png`。
## 2026-11-14 A/B显示缩小20%，未更新应用

用户反馈A/B过大。本阶段完成：仅修改本地`Assets/Resources/VirtualKeyboard/Bronze/A.png`、`B.png`，为原617×617切图各侧增加77px透明留边，得到771×771，整体显示直径变为原来的617/771≈80%；原中心区域RGBA逐字节不变，没有重绘、插值或改变原半透明度。100×100点击Rect、A/B中心位置、回调、方向盘、中央印章、玩法和存档均保持，Sprite .meta/GUID保持，FullRect及NPOT None沿用。新增修前PNG及修订记录在原工作目录的`ab-smaller/`，便于回滚。

修改`SwordsmanVirtualKeyboardSkinValidation.cs`，仅增加可选`-swordsman-keyboard-skin-output <目录>`，防止后续验证覆盖前次证据。执行图形Editor的`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode`并输出至`Logs/virtual-keyboard-bronze-20261114/ab-smaller/`；脚本编译及四场景Missing Script=0，47项运行检查通过，已查看实际1280×800的150%画面确认A/B与字母同步缩小、材质不变。148项场景、插件、配置、DPad、Sprite.meta及现有应用程序集保护文件无变化；Git路径状态保持。Debug/Development Player构建未运行，本次没有更新唯一测试包或关闭运行中的游戏，等待用户后续构建指令。当前Player仍显示上一版较大的A/B。证据入口`ab-smaller/delivery.json`；无需新增第三方依赖、业务代码、场景、Prefab或工程配置。
## A/B缩小版已更新唯一测试包

用户确认退出并授权更新，覆盖前文“缩小20%未更新应用”状态。本轮未新增或修改产品代码，使用既有唯一入口`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`直接更新`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，包含771×771透明留边的A/B切图（显示直径约80%）；原100×100点击Rect、中心位置、半透明度、输入回调、方向盘与开关/禁用缩放规则保持。同步修改本文和资源清单及本地交付元数据；无新应用或源码文件。

本次Development构建Succeeded，0错误/0警告；`codesign --verify --deep --strict`通过。锁定复用Bundle SHA256 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`，包内哈希完全一致，未重跑未变Bundle的完整静态资源运行检查。最终Player执行`-swordsman-virtual-keyboard-report`及截图参数，33项全部通过，含实际包内皮肤、原点击尺寸/位置、四方向、A/B确认/取消、开关、缩放及关闭后的禁用。已查看最终Player实际150%截图确认A/B与字母缩小、背景可透出。603份存档、场景、资源、插件及工程配置保护文件哈希保持，Git路径状态保持，无提交/推送。

证据入口`Logs/virtual-keyboard-bronze-20261114/ab-smaller/player/delivery.json`，构建`build.log`、专项`check.json`、最终截图`captures/keyboard-150.png`。指针处理器检查不替代用户原生鼠标和手感验收；建议现在在唯一应用复验本次缩小效果。
## 2026-11-15 紧凑A/B：缩小、收紧间距并右下留边，尚未打包

本阶段完成。用户明确选择将点击区从100×100缩至80×80，以支持明显收紧间距；确认/返回语义不变。修改`GameVirtualKeyboardSkin.cs`的A/B尺寸与位置，`SwordsmanVirtualKeyboardSkinValidation.cs`和`SwordsmanVirtualKeyboardChecks.cs`对应断言，及本地A/B透明PNG（保留.meta/GUID），同步架构、本文及唯一资源清单。未新增源码文件；原始617×617核心RGBA保持，705×705透明画布配合80×80显示使可见直径约为最初70%、上一版的87.5%。A/B中心为(-111,45)/(-45,111)，右下可见padding约10，紧凑但不进入另一键可见区域。修前PNG、候选与逐像素检查结果在原工作目录`ab-compact/`。

执行`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode -swordsman-keyboard-skin-output Logs/virtual-keyboard-bronze-20261115/ab-compact`，脚本编译通过，四场景Missing Script=0、47项Editor运行检查通过；已查看实际1280×800的150%截图，确认尺寸、间距、右下布局及半透明效果。逐像素检测：两键非零Alpha像素进入另一80×80点击区的数量均为0；点击矩形仅在空白角区14×14交叠，不声称矩形完全不重叠。中心距约93.34，150%全部角点仍在Canvas内。178项受保护场景、插件、DPad、Sprite.meta、配置及现有应用程序集哈希保持，Git路径状态保持。

Debug/Development Player构建未运行，未更新或关闭当前游戏；方向盘、C/D、输入回调、开关、缩放倍率、存档、玩法及工程配置未改。建议用户确认本版画面后按后续指令更新唯一测试包；Player实机效果和鼠标复验待构建后进行。证据入口`Logs/virtual-keyboard-bronze-20261115/ab-compact/delivery.json`，实际截图`editor-settings-150.png`，原始检查日志`editor.log`。
## 2026-11-15 紧凑A/B版已更新唯一测试包

用户退出并明确授权更新，覆盖前文紧凑版“尚未打包”状态。本阶段完成，既有唯一入口`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`直接更新`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，包含70%显示直径、80×80点击区、A(-111,45)/B(-45,111)、更短间距和右下留白。未新增或修改产品代码、切图；仅同步本文、架构和唯一资源清单及本地交付记录，无额外应用、提交或推送。

Development构建Succeeded，0错误/9个既有警告；`codesign --verify --deep --strict`通过。复用锁定HD Bundle SHA256 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`，包内哈希一致；没有重跑未变Bundle的完整静态运行检查。最终Player执行现有`-swordsman-virtual-keyboard-report`与截图参数，33项全部通过，覆盖实际皮肤引用、紧凑位置/80×80点击区、四方向、A确认/B返回、开关、缩放、关闭后尺寸项禁用及重开恢复。已查看100%/150%实际Player截图确认大小、间距、右下位置和半透明效果；不将指针处理器检查描述为操作系统鼠标自动点击或用户验收。603个受保护存档、场景、资源、插件及配置哈希保持，Git路径状态保持；玩法、存档格式、场景和工程配置未改。

证据入口`Logs/virtual-keyboard-bronze-20261115/ab-compact/player/delivery.json`，构建`build.log`、专项`check.json`、实际截图`captures/keyboard-on.png`和`keyboard-150.png`。当前未解决项为用户实机视觉与鼠标手感复验；建议现在在唯一应用验收。
## 2026-11-15 左侧方向盘显示缩小10%，尚未打包

本阶段完成，仅修改本地`Assets/Resources/VirtualKeyboard/Bronze/DPad.png`：1254×1254原稿四边各增加70px全透明留白，得到1394×1394，显示比例约89.96%，四方向与中央圆环印章同步缩小；原1254×1254核心RGBA逐字节保持，透明度不变，无重绘、插值或重新编码图案。DPad点击Rect仍230×230，buttonSizeCoef=3、中心位置及所有方向回调不变；A/B两张PNG、80×80点击区、紧凑位置、插件、场景、配置、玩法和存档保持。没有新增或修改源码文件。新增修前PNG和修订记录在原工作目录`dpad-smaller/`；同步本文及唯一资源清单的待入包状态。

执行`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode -swordsman-keyboard-skin-output Logs/virtual-keyboard-bronze-20261115/dpad-smaller`，Unity无脚本编译错误，四场景Missing Script=0，47项Editor运行检查通过；已查看1280×800的150%实际渲染，方向盘与印章缩小、Alpha及A/B保持。受保护文件哈希与Git路径状态保持，详情以`Logs/virtual-keyboard-bronze-20261115/dpad-smaller/delivery.json`为准。Debug/Development Player构建未运行，当前唯一应用仍为上一版方向盘，未关闭运行中的游戏。待用户后续构建指令后统一更新，再做Player与用户实机复验。
## 2026-11-15 左侧方向盘缩小版已更新唯一测试包

本阶段完成，用户授权更新后，发现旧进程尚在，先暂停自身构建且未覆盖Player；用户完全退出并核实进程不存在后，重新采集保护基线并继续。通过既有唯一入口`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`直接更新`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。包含1394×1394透明画布的DPad，方向盘和印章显示缩小约10%，230×230点击区、中心位置及方向判定保持，紧凑A/B未改。未新增或修改源码/切图，仅同步验证与资源状态记录；无其他应用、提交或推送。

Development构建Succeeded，0错误/0警告；`codesign --verify --deep --strict`通过。锁定复用HD Bundle SHA256 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`，包内哈希一致，未重跑未变Bundle的完整静态运行检查。最终Player显式运行`-swordsman-virtual-keyboard-report`及截图参数，33项全部通过，覆盖包内RGBA皮肤、原DPad几何/四方向/松开、A/B位置和确认/取消、开关、缩放及关闭后禁用尺寸项。已查看实际Player的150%截图，方向盘与中央印章缩小、Alpha正确，右侧A/B保持。603个受保护存档、资源、插件、场景及配置哈希保持，Git路径状态保持。

证据入口`Logs/virtual-keyboard-bronze-20261115/dpad-smaller/player/delivery.json`，构建`build.log`、专项`check.json`、截图`captures/keyboard-150.png`及进程交接`process-handoff.json`。原生鼠标与用户视觉验收仍待确认，不以实际指针处理器检查代替用户实机。前文方向盘“未打包”仅对应接入阶段；建议现在在唯一应用复验。
## 2026-11-15 左方向盘再缩小并匹配A/B留边，尚未打包

本阶段完成。修改本地DPad.png为1550×1550透明画布，原核心RGBA保持，不重绘、不改变Alpha；显示比上一版再缩约10%。修改`GameVirtualKeyboardSkin.cs`左方向盘位置为(101.44,99.39)，原230×230点击区、buttonSizeCoef=3及输入语义保持；A/B两图、80×80尺寸及原紧凑位置不改。修改`SwordsmanVirtualKeyboardSkinValidation.cs`新增实际Alpha边界/Canvas留边测量，修改`SwordsmanVirtualKeyboardChecks.cs`新增下次Player的位置断言，更新架构、本文及唯一资源清单。新增本地修前PNG与修订元数据在原工作目录`dpad-compact-padding/`；无新源码文件或依赖。

执行`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode -swordsman-keyboard-skin-output Logs/virtual-keyboard-bronze-20261115/dpad-compact-padding`：无脚本编译错误，四场景Missing Script=0，54项Editor检查通过。在100%与150%分别按实际PNG非零Alpha像素中心转换到Canvas，方向盘左留边与B右留边、方向盘底留边与A底留边差值均小于0.05 Canvas单位，所有可见图案在画面内；已查看两档实际1280×800截图。原点击Rect因整体移向角落而可能有透明边缘位于Canvas之外，不将可见图案未越界表述为全部点击Rect未越界；四方向及中心/松开检查通过。受保护文件哈希和Git路径状态保持，详情`Logs/virtual-keyboard-bronze-20261115/dpad-compact-padding/delivery.json`。

Debug/Development Player构建未运行，现有唯一包仍为上版方向盘；未关闭运行中的游戏。场景、Prefab、插件、工程配置、存档、玩法、右侧A/B和透明度保持，待用户后续构建指令后统一更新及Player实机复验。
## 2026-11-15 工作室三行文案与官网入口，尚未打包

本阶段完成。修改`GameHelpUI.cs`增加固定官网URL、简繁三行文案、原`onClickB`回调及展示/切换时更新；修改`level0.unity`的工作室Text、点击矩形、文字矩形及原工作室简繁条目。新增`SwordsmanStudioHomepageValidation.cs`和Unity生成的.meta。官网地址从相邻官网仓库当前`COMPANY_INFO.md`读取，HTTPS HEAD返回200；官网仓库未修改。用户括号中的说明未进入界面。其他设置、虚拟键盘、玩法、存档、插件和工程配置未改。

先用Unity Editor的`SwordsmanStudioHomepageValidation.Apply`设置，仅保留序列化对象429/485/672/645四块变化，恢复2块无关重序列化；GameStringData的68个既有条目仅原工作室条目64改变，其他条目和枚举位置保持。修改前场景、完整Unity保存结果及范围报告保留于`Logs/studio-homepage-20261115/`。

执行`SwordsmanStudioHomepageValidation.RunPlayMode -swordsman-open-studio-homepage`：脚本编译无错误、四场景Missing Script=0，14项检查通过，含回调绑定、固定HTTPS首页、简繁三行准确内容/无额外换行/未混入括号说明、第一行点击范围和第二三行不触发。已查看1280×800的`simplified.png`、`traditional.png`。通过实际场景Button.onClick回调调起系统URL处理，浏览器状态确认新Chrome标签为官网首页且页面标题为“灵感盒子科技”；这是Editor回调的系统浏览器实测，不是独立Player原生鼠标点击验收。

另运行现有`SwordsmanVirtualKeyboardSkinValidation.RunPlayMode`，输出`keyboard-regression/`，54项键盘回归检查通过。180项其他场景、资源、插件、配置及旧应用程序集哈希保持；新Git路径仅验证器及.meta，其他既有修改保留。Debug/Development Player构建未运行，未关闭或覆盖现有游戏；Windows/iOS/Android未做本次构建及设备验证。建议等待用户后续指令，将本次文案/链接与前面尚未入包的方向盘留边修改一并更新唯一测试包。证据入口`Logs/studio-homepage-20261115/delivery.json`，`scene-scope.json`与`editor-check.json`。
## 2026-11-15 左方向盘留边与工作室官网入口已统一入包

本阶段完成，已核实用户游戏进程退出后，通过唯一`SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`直接更新`Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。包含1550×1550 DPad透明画布、位置(101.44,99.39)、左/底可见padding对应B右/A底，以及工作室简繁三行新文案与第一行官网跳转；A/B及其他业务保持。前文两项“尚未打包”状态已由本次覆盖。

本阶段仅扩展`SwordsmanVirtualKeyboardChecks.cs`显式入口，新增最终包的三行简繁内容/无说明括号/无溢出/第一行点击区域/官网绑定检查，及可选`-swordsman-virtual-keyboard-open-homepage`：只在专项检查完成后通过真实Button指针点击处理器打开固定官网一次，正常启动不执行。同步本文、架构、安全记录、资源清单和本地交付元数据；无新源码文件、依赖、额外应用、提交或推送。

Development构建Succeeded，0错误/9个既有警告；`codesign --verify --deep --strict`通过。复用锁定HD Bundle SHA256 `7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b`，包内哈希一致，未重跑未变Bundle的完整静态运行检查。最终包带report/capture/open-homepage专项参数运行，49项全部通过，含原键盘方向/确认/取消/开关/缩放和新增文案、位置及链接。已查看实际`keyboard-150.png`和`studio-traditional.png`；可见方向盘留边、文字及Alpha正确。真实Player Button指针点击处理器调起系统URL处理后，Chrome页面核对为官网首页（标题“灵感盒子科技”）；不将此描述为操作系统鼠标自动点击或用户验收。

603个受保护存档、场景、资源、插件及配置哈希保持，Git路径状态保持。当前唯一应用已包含本次合并修改，用户实机视觉及鼠标手感待确认；Windows/iOS/Android未进行本次构建或设备测试。证据入口`Logs/settings-combined-20261115/delivery.json`，构建`build.log`、专项`check.json`与`captures/`。
## 2026-11-15 去掉工作室“主页”与三档难度方案研究

本阶段完成：仅将`GameHelpUI.StudioInformation`的简体“主页”及繁体“主頁”后缀删除，官网URL和点击区域不变。同步标题场景Text及工作室简繁条目，仅保留序列化对象645/672两块变化，其他场景块保持；没有修改难度名称、枚举、数值或存档。修改`SwordsmanStudioHomepageValidation.cs`使迁移支持现有工作室条目、增加可选输出目录与后缀检查。新增`docs/difficulty-mode-proposal.md`，记录基于代码的三档映射、经验/减伤/恢复/濒死保护/重试熟练度方案及兼容、阶段验收，状态为待用户确认。

执行Unity的`SwordsmanStudioHomepageValidation.Apply`和`RunPlayMode -swordsman-studio-output Logs/difficulty-design-20261115/copy-check`，无脚本编译错误，四场景Missing Script=0，14项简繁文案/布局/点击区检查通过；本次未重开未变的官网链接。受保护脚本（除本次文案所属GameHelpUI）、其他场景、配置、键盘资源及旧应用程序集哈希全部保持，证明没有实现新难度逻辑；数量与范围见`Logs/difficulty-design-20261115/delivery.json`。新增Git路径仅方案文档，其他原有修改保留，无异常文件、提交或推送。

Debug/Development Player构建未运行，当前运行包仍含“主页”字样及原两档难度。需要用户先确认难度方案，之后再分阶段实现；本轮文字变化可在下次统一构建入包。建议确认初版的经验×2、伤害/恢复参数、一次保护、重试熟练度宽免及成长切回不回退规则后进入下一阶段。

## 2026-11-17 LZFSE 正式接入唯一应用

唯一Development构建入口 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters` 成功，0错误、9条既有警告；脚本编译通过，签名严格验证通过。包内高清Bundle为 `SWLZF001`，3,300,624,150 → 2,936,657,162字节；全量解码SHA与实验一致。arm64 Player执行 `-swordsman-static-mvp-check` 通过1819项资源及472项探针检查；`-swordsman-chapter-tests-check -swordsman-chapter-tests-report Logs/lzfse-production-20261117/chapter-check.json` 扫过36幕，存档/检查点测试通过。Rosetta x86_64 Player运行同一高清检查通过，真Intel设备及人工画质仍待用户验收。唯一应用磁盘占用 `du -sk` 为4,233,812 KiB，构建入口计时287.57秒。隔离验证构建产物约12 GiB已删除，证据见 `Logs/lzfse-production-20261117/` 与 [方案记录](lzfse-validation.md)。
