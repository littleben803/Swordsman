using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class SwordsmanHd4xBuild
{
    [Serializable]
    sealed class HdCacheValidationReport
    {
        public bool passed;
        public string[] checks;
        public string bundleHash;
        public int assets;
    }

    // Explicit Editor-only checks; never edits approved art, source assets or runtime loading.
    public static void ValidateApprovedBundleCache()
    {
        var checks = new List<string>();
        string recipe = GetHdCacheRecipeHash();
        List<HdCacheInput> inputs = CreateHdCacheInputs( recipe );
        HdCacheState state = ReadHdCacheState();
        RequireHdCacheCheck( state != null, "Build the approved cache before validating it." );
        RequireHdCacheCheck( PlanHdCacheChanges( inputs, state ).changed.Count == 0, "Warm cache is not clean." );
        checks.Add( "unchanged-inputs-import-zero" );

        HdCacheInput selected = inputs[0];
        foreach ( HdCacheInput input in inputs )
            if ( new FileInfo( GetHdCacheFilePath( input.asset.stagingPath ) ).Length <
                 new FileInfo( GetHdCacheFilePath( selected.asset.stagingPath ) ).Length ) selected = input;
        List<HdCacheInput> fullInputs = inputs;
        HdCacheState fullState = state;
        HdCacheInput sentinel = inputs[0] == selected ? inputs[1] : inputs[0];
        inputs = new List<HdCacheInput> { selected, sentinel };
        state = JsonUtility.FromJson<HdCacheState>( JsonUtility.ToJson( state ) );
        state.entries = Array.FindAll( state.entries, entry =>
            entry.path == selected.asset.stagingPath || entry.path == sentinel.asset.stagingPath );
        state.inputsHash = GetHdCacheInputsHash( inputs );
        int index = inputs.IndexOf( selected );
        var fileHashes = new Dictionary<string, string>( StringComparer.Ordinal );
        ApprovedAsset modified = selected.asset;
        modified.maxTextureSize = modified.maxTextureSize == 256 ? 512 : 256;
        AtlasSpec? atlas = selected.atlas;
        if ( atlas.HasValue ) { AtlasSpec value = atlas.Value; value.asset = modified; atlas = value; }
        var changed = new List<HdCacheInput>( inputs );
        changed[index] = CreateHdCacheInput( modified, atlas, recipe, fileHashes );
        HdCachePlan plan = PlanHdCacheChanges( changed, state );
        RequireHdCacheCheck( plan.changed.Count == 1 && plan.changed[0].asset.stagingPath == modified.stagingPath,
            "Importer change did not isolate one asset." );
        RequireHdCacheCheck( GetHdCacheInputsHash( changed ) != state.inputsHash, "Importer change kept the bundle fingerprint." );
        checks.Add( "one-importer-change-selects-one-image" );

        var added = new List<HdCacheInput>( inputs );
        ApprovedAsset extra = selected.asset;
        extra.stagingPath = StagingFolder + "/CacheValidation/new-image.png";
        atlas = selected.atlas;
        if ( atlas.HasValue ) { AtlasSpec value = atlas.Value; value.asset = extra; atlas = value; }
        added.Add( CreateHdCacheInput( extra, atlas, recipe, fileHashes ) );
        plan = PlanHdCacheChanges( added, state );
        RequireHdCacheCheck( plan.changed.Count == 1 && plan.changed[0].asset.stagingPath == extra.stagingPath,
            "New image invalidated unrelated cached imports." );
        checks.Add( "one-added-image-selects-one-import" );

        var removed = new List<HdCacheInput>( inputs ); removed.RemoveAt( index );
        plan = PlanHdCacheChanges( removed, state );
        RequireHdCacheCheck( plan.changed.Count == 0 && plan.removed.Count == 1 &&
            plan.removed[0] == selected.asset.stagingPath && GetHdCacheInputsHash( removed ) != state.inputsHash,
            "Removed image did not invalidate bundle membership." );
        checks.Add( "removed-image-invalidates-bundle-membership" );

        string probe = "Builds/HD4X/static-mvp/cache-validation/changed-image.png";
        Directory.CreateDirectory( Path.GetDirectoryName( GetAbsoluteProjectPath( probe ) ) );
        File.Copy( GetAbsoluteProjectPath( selected.asset.approvedRelativePath ), GetAbsoluteProjectPath( probe ), true );
        ApprovedAsset probeAsset = selected.asset;
        probeAsset.approvedRelativePath = probe;
        atlas = selected.atlas;
        if ( atlas.HasValue ) { AtlasSpec value = atlas.Value; value.asset = probeAsset; atlas = value; }
        HdCacheInput first = CreateHdCacheInput( probeAsset, atlas, recipe, new Dictionary<string, string>() );
        using ( var stream = new FileStream( GetAbsoluteProjectPath( probe ), FileMode.Append ) ) stream.WriteByte( 0 );
        probeAsset.approvedSha256 = GetSha256( GetAbsoluteProjectPath( probe ) );
        if ( atlas.HasValue ) { AtlasSpec value = atlas.Value; value.asset = probeAsset; atlas = value; }
        HdCacheInput second = CreateHdCacheInput( probeAsset, atlas, recipe, new Dictionary<string, string>() );
        RequireHdCacheCheck( first.hash != second.hash, "Changed image bytes were not detected." );
        changed = new List<HdCacheInput>( inputs ); changed[index] = second;
        RequireHdCacheCheck( PlanHdCacheChanges( changed, state ).changed.Count == 1, "Changed image invalidated unrelated imports." );
        checks.Add( "changed-image-bytes-select-one-import" );
        probeAsset.approvedSha256 = new string( '0', 64 );
        bool rejected = false;
        try { CreateHdCacheInput( probeAsset, atlas, recipe, new Dictionary<string, string>() ); }
        catch ( InvalidOperationException ) { rejected = true; }
        RequireHdCacheCheck( rejected, "Unapproved image hash was accepted." );
        checks.Add( "unapproved-input-rejected" );
        File.Delete( GetAbsoluteProjectPath( probe ) );

        HdCacheState damaged = JsonUtility.FromJson<HdCacheState>( JsonUtility.ToJson( state ) );
        foreach ( HdCacheEntry entry in damaged.entries )
            if ( entry.path == selected.asset.stagingPath ) entry.metaHash = new string( '0', 64 );
        plan = PlanHdCacheChanges( inputs, damaged );
        RequireHdCacheCheck( plan.changed.Count == 1 && plan.resetImporter.Contains( selected.asset.stagingPath ),
            "Damaged importer was not isolated for repair." );
        checks.Add( "damaged-importer-selects-one-repair" );

        string bundlePath = GetAbsoluteProjectPath( BundleOutputRelativePath + "/" + BundleName );
        damaged.bundleHash = new string( '0', 64 );
        RequireHdCacheCheck( !CanReuseHdCacheBundle( damaged, recipe, state.inputsHash, bundlePath ), "Wrong bundle hash was reused." );
        RequireHdCacheCheck( !CanReuseHdCacheBundle( state, recipe + "changed", state.inputsHash, bundlePath ), "Changed importer recipe was reused." );
        RequireHdCacheCheck( !CanReuseHdCacheBundle( state, recipe, "changed", bundlePath ), "Changed asset membership was reused." );
        checks.Add( "bundle-hash-recipe-and-membership-invalidation" );

        string manifest = GetAbsoluteProjectPath( HdCacheManifest );
        byte[] savedManifest = File.ReadAllBytes( manifest );
        try
        {
            File.WriteAllText( manifest, "{ invalid json" );
            RequireHdCacheCheck( ReadHdCacheState() == null, "Truncated cache manifest was trusted." );
        }
        finally { File.WriteAllBytes( manifest, savedManifest ); }
        checks.Add( "truncated-manifest-rejected" );

        inputs = fullInputs;
        state = fullState;
        string cachedImage = GetHdCacheFilePath( selected.asset.stagingPath );
        byte[] savedImage = File.ReadAllBytes( cachedImage );
        try
        {
            using ( var stream = new FileStream( cachedImage, FileMode.Append ) ) stream.WriteByte( 0 );
            plan = PlanHdCacheChanges( inputs, state );
            RequireHdCacheCheck( plan.changed.Count == 1, "Damaged cached image was not detected." );
            BuildAndValidateCachedBundle();
            RequireHdCacheCheck( GetSha256( cachedImage ) == selected.asset.approvedSha256, "Cached image was not repaired." );
            RequireHdCacheCheck( GetSha256( bundlePath ) == state.bundleHash, "Repair unexpectedly changed the approved bundle." );
            RequireHdCacheCheck( PlanHdCacheChanges( inputs, ReadHdCacheState() ).changed.Count == 0, "Repaired cache is not clean." );
        }
        finally
        {
            if ( File.Exists( cachedImage ) && GetSha256( cachedImage ) != selected.asset.approvedSha256 )
                File.WriteAllBytes( cachedImage, savedImage );
        }
        RequireHdCacheCheck( !Directory.Exists( GetAbsoluteProjectPath( StagingFolder ) ), "HD Resources staging remained in Assets." );
        ValidateHdCacheDirectory( GetAbsoluteProjectPath( HdCacheAssets ) );
        checks.Add( "real-single-image-repair-keeps-bundle-and-stable-cache" );

        var report = new HdCacheValidationReport { passed = true, checks = checks.ToArray(), bundleHash = state.bundleHash, assets = inputs.Count };
        string[] args = Environment.GetCommandLineArgs();
        int reportIndex = Array.IndexOf( args, "-swordsman-hd-cache-report" );
        string output = reportIndex >= 0 && reportIndex + 1 < args.Length ? args[reportIndex + 1]
            : GetAbsoluteProjectPath( "Logs/hd-cache-validation.json" );
        Directory.CreateDirectory( Path.GetDirectoryName( Path.GetFullPath( output ) ) );
        File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        Debug.LogFormat( "SWORDSMAN_HD_CACHE_VALIDATION passed checks={0} assets={1}", checks.Count, inputs.Count );
    }

    static void RequireHdCacheCheck( bool condition, string message )
    {
        if ( !condition ) throw new InvalidOperationException( message );
    }
}
