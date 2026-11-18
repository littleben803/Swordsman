using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#if SWORDSMAN_CHAPTER_TESTS && (DEVELOPMENT_BUILD || UNITY_EDITOR)
[Serializable]
public sealed class SwordsmanChapterTestFlag
{
    public int id;
    public int value;
}

[Serializable]
public sealed class SwordsmanChapterTestPreset
{
    public int stage;
    public string title;
    public int suggestedLevel;
    public int[] startingParty;
    public short[] items;
    public SwordsmanChapterTestFlag[] flags;
    public int revision;
    public string validation;
    public string notes;
}

[Serializable]
public sealed class SwordsmanChapterTestCatalog
{
    public int schemaVersion;
    public SwordsmanChapterTestPreset[] entries;
}

[Serializable]
public sealed class SwordsmanChapterTestCheckpoint
{
    public int schemaVersion = 2;
    public int stage;
    public int level;
    public string presetHash;
    public string binaryHash;
    public string provenance = "synthetic-opening-played-to-first-player-turn";
}

public static class SwordsmanChapterTestSession
{
    public static int Stage { get; private set; }
    public static int Level { get; private set; } = 1;
    public static string RootDirectory
    {
        get
        {
            string root = Path.Combine( Application.persistentDataPath, "ChapterTests", "v2" );
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-easy-check" ) >= 0 )
                return Path.Combine( root, "EasyValidation" );
            if ( SwordsmanChapterTestMock.Active != SwordsmanChapterMockOption.None )
                root = Path.Combine( root, "Mock", SwordsmanChapterTestMock.Profile );
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-package-check" ) >= 0 )
                return Path.Combine( root, "PackageValidation" );
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-stage-check" ) >= 0 )
                return Path.Combine( root, "Population" );
            return Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-check" ) >= 0
                ? Path.Combine( root, "Automation" ) : root;
        }
    }
    public static string SaveDirectory { get { string path = DirectoryFor( Stage, Level ); Directory.CreateDirectory( path ); return path; } }

    public static string DirectoryFor( int stage, int level )
    {
        if ( stage < 0 || stage >= GameDefine.MAX_STAGE || level < 1 || level > GameDefine.MAX_LEVEL )
            throw new ArgumentOutOfRangeException( "Invalid chapter test profile." );
        string path = Path.Combine( RootDirectory, "stage" + stage.ToString( "D2" ), "level" + level.ToString( "D2" ) );
        return path;
    }

    public static SwordsmanChapterTestCatalog LoadCatalog()
    {
        string path = Path.Combine( Application.streamingAssetsPath, "SwordsmanChapterTests", "presets.json" );
        SwordsmanChapterTestCatalog catalog = JsonUtility.FromJson<SwordsmanChapterTestCatalog>( File.ReadAllText( path ) );
        if ( catalog == null || catalog.schemaVersion != 2 || catalog.entries == null || catalog.entries.Length != GameDefine.MAX_STAGE )
            throw new InvalidDataException( "Chapter test catalog is incomplete." );
        bool[] seen = new bool[GameDefine.MAX_STAGE];
        foreach ( SwordsmanChapterTestPreset preset in catalog.entries )
        {
            if ( preset.stage < 0 || preset.stage >= seen.Length || seen[preset.stage] || string.IsNullOrEmpty( preset.title ) ||
                preset.suggestedLevel < 1 || preset.suggestedLevel > GameDefine.MAX_LEVEL || preset.startingParty == null || preset.items == null || preset.flags == null )
                throw new InvalidDataException( "Invalid chapter test preset." );
            seen[preset.stage] = true;
            foreach ( short item in preset.items ) if ( item < 0 ) throw new InvalidDataException( "Invalid test inventory item." );
            foreach ( int unit in preset.startingParty ) if ( unit < 0 || unit >= GameDefine.MAX_USER ) throw new InvalidDataException( "Invalid test party member." );
            foreach ( SwordsmanChapterTestFlag flag in preset.flags ) if ( flag.id < 0 || flag.id >= GameDefine.MAX_GAMEDATA ) throw new InvalidDataException( "Invalid test story flag." );
        }
        for ( int i = 0; i < catalog.entries.Length; i++ ) if ( catalog.entries[i].stage != i ) throw new InvalidDataException( "Chapter test entries must follow source indices." );
        return catalog;
    }

    public static void Select( SwordsmanChapterTestPreset preset, int level )
    {
        string path = DirectoryFor( preset.stage, level );
        Directory.CreateDirectory( path );
        Stage = preset.stage;
        Level = level;
        GameSetting.PersistentDataPath = path;
        GameSetting.AddonPath = path;
    }

    public static void Apply( SwordsmanChapterTestPreset preset, int level )
    {
        Select( preset, level );
        GameUserData.instance.init();
        GameUserData.instance.setStage( preset.stage );
        GameUserData.instance.setNextStage( Mathf.Min( preset.stage + 1, GameDefine.MAX_STAGE - 1 ) );
        for ( int i = 0; i < GameDefine.MAX_USER; i++ )
        {
            GameUnitBase unit = GameUserData.instance.getUnitBase( i );
            unit.InTeam = (short)( Array.IndexOf( preset.startingParty, i ) >= 0 ? 1 : 0 );
            // A deterministic synthetic growth profile, not a historical playthrough.
            GameUnit growthUnit = GameUnitData.instance.getData( i );
            GameUnitLevelUp growth = GameUnitLevelUpData.instance.getData( i );
            if ( growth != null && growthUnit != null )
            {
                while ( unit.LV < level )
                {
                    unit.HP += growthUnit.HPGrow + (int)( unit.Vit / 35.0f + ( growth.HP + 1 ) * 0.5f );
                    unit.MP += growthUnit.MPGrow + (int)( unit.Int / 35.0f + ( growth.MP + 1 ) * 0.5f );
                    unit.Str += (short)( growth.StrBase + ( 1 + Math.Max( 1, (int)growth.StrRand ) ) / 2 );
                    unit.Int += (short)( growth.IntBase + ( 1 + Math.Max( 1, (int)growth.IntRand ) ) / 2 );
                    unit.Avg += (short)( growth.AvgBase + ( 1 + Math.Max( 1, (int)growth.AvgRand ) ) / 2 );
                    unit.Vit += (short)( growth.VitBase + ( 1 + Math.Max( 1, (int)growth.VitRand ) ) / 2 );
                    unit.Luk += (short)( growth.LukBase + ( 1 + Math.Max( 1, (int)growth.LukRand ) ) / 2 );
                    unit.BaseSpiritPower += 2;
                    unit.LV++;
                }
            }
            unit.Exp = 0;
        }
        foreach ( short item in preset.items )
        {
            if ( GameItemData.instance.getData( item ) == null ) throw new InvalidDataException( "Unknown test inventory item." );
            GameUserData.instance.addItem( item );
        }
        foreach ( SwordsmanChapterTestFlag flag in preset.flags ) GameUserData.instance.setGameData( flag.id, flag.value );
        SwordsmanChapterTestMock.PrepareParty();
    }

    static string PresetHash( SwordsmanChapterTestPreset preset, int level )
    {
        return Hash( Encoding.UTF8.GetBytes( "chapter-tests-v2|" + level + "|" + JsonUtility.ToJson( preset ) ) );
    }

    public static bool HasCheckpoint( SwordsmanChapterTestPreset preset, int level )
    {
        try
        {
            string path = DirectoryFor( preset.stage, level );
            string bundled = Path.Combine( Application.streamingAssetsPath, "SwordsmanChapterTests", "Checkpoints", "stage" + preset.stage.ToString( "D2" ), "level" + level.ToString( "D2" ) );
            string[] args = Environment.GetCommandLineArgs();
            bool installBundled = Array.IndexOf( args, "-swordsman-chapter-tests-stage-check" ) < 0 || Array.IndexOf( args, "-swordsman-chapter-tests-reuse" ) >= 0;
            if ( installBundled && !File.Exists( Path.Combine( path, "BattleStart.dat" ) ) && !File.Exists( Path.Combine( path, "BattleStart.json" ) ) && ValidCheckpoint( bundled, preset, level ) )
            {
                Directory.CreateDirectory( path );
                File.Copy( Path.Combine( bundled, "BattleStart.dat" ), Path.Combine( path, "BattleStart.dat" ), false );
                File.Copy( Path.Combine( bundled, "BattleStart.json" ), Path.Combine( path, "BattleStart.json" ), false );
            }
            return ValidCheckpoint( path, preset, level );
        }
        catch ( IOException ) { return false; }
        catch ( ArgumentException ) { return false; }
    }

    static bool ValidCheckpoint( string path, SwordsmanChapterTestPreset preset, int level )
    {
        try
        {
            SwordsmanChapterTestCheckpoint info = JsonUtility.FromJson<SwordsmanChapterTestCheckpoint>( File.ReadAllText( Path.Combine( path, "BattleStart.json" ) ) );
            byte[] bytes = File.ReadAllBytes( Path.Combine( path, "BattleStart.dat" ) );
            return info != null && info.schemaVersion == 2 && info.stage == preset.stage && info.level == level &&
                info.presetHash == PresetHash( preset, level ) && bytes.Length == 102400 && BitConverter.ToInt32( bytes, 0 ) == preset.stage && info.binaryHash == Hash( bytes );
        }
        catch ( IOException ) { return false; }
        catch ( ArgumentException ) { return false; }
    }

    public static void CaptureCheckpoint( SwordsmanChapterTestPreset preset, int level )
    {
        if ( GameUserData.instance.Stage != preset.stage || GameSetting.PersistentDataPath != DirectoryFor( preset.stage, level ) )
            throw new InvalidOperationException( "Test checkpoint context mismatch." );
        GameUserData.instance.saveBattle();
        byte[] bytes = File.ReadAllBytes( Path.Combine( SaveDirectory, "SaveBattle.dat" ) );
        if ( bytes.Length != 102400 || BitConverter.ToInt32( bytes, 0 ) != preset.stage ) throw new InvalidDataException( "Invalid test battle snapshot." );
        File.WriteAllBytes( Path.Combine( SaveDirectory, "BattleStart.dat" ), bytes );
        SwordsmanChapterTestCheckpoint info = new SwordsmanChapterTestCheckpoint {
            stage = preset.stage, level = level, presetHash = PresetHash( preset, level ), binaryHash = Hash( bytes ) };
        File.WriteAllText( Path.Combine( SaveDirectory, "BattleStart.json" ), JsonUtility.ToJson( info, true ) );
    }

    public static bool RestoreCheckpoint( SwordsmanChapterTestPreset preset, int level )
    {
        if ( !HasCheckpoint( preset, level ) ) return false;
        Select( preset, level );
        File.Copy( Path.Combine( SaveDirectory, "BattleStart.dat" ), Path.Combine( SaveDirectory, "SaveBattle.dat" ), true );
        return true;
    }

    public static string Hash( byte[] bytes )
    {
        using ( SHA256 hash = SHA256.Create() ) return BitConverter.ToString( hash.ComputeHash( bytes ) ).Replace( "-", "" ).ToLowerInvariant();
    }
}
#endif
