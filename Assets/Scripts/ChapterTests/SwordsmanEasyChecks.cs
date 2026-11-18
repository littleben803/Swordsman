using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
// Explicit, isolated test run. Does not change the user's mode preference or ordinary save slots.
public sealed class SwordsmanEasyChecks : MonoBehaviour
{
    [Serializable] sealed class Report
    {
        public List<string> passed = new List<string>();
        public string error = "";
    }
    readonly Report report = new Report();
    string output;
    GameSetting.GameMode previousMode;
    GameSetting.GameLocation previousLanguage;
    readonly Dictionary<string, object> prefs = new Dictionary<string, object>();
    readonly string[] keys = { "touchScale", "virtualKeyboardEnabled", "fastDialogue", "location", "mode", "enableMusic", "enableSound", "quality" };

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Install()
    {
        var args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf( args, "-swordsman-easy-check" );
        if ( i < 0 || i + 1 >= args.Length ) return;
        var go = new GameObject( "EasyDifficultyChecks" ); DontDestroyOnLoad( go );
        go.AddComponent<SwordsmanEasyChecks>().output = args[i + 1];
    }
    void Require( bool condition, string name )
    {
        if ( !condition ) throw new Exception( name );
        report.passed.Add( name );
    }
    static void Set( object target, string field, object value )
    {
        target.GetType().GetField( field, BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( target, value );
    }
    static void Award( GameBattleUnit unit, float value )
    {
        typeof(GameBattleUnit).GetMethod( "AccumulateExperience", BindingFlags.Instance | BindingFlags.NonPublic ).Invoke( unit, new object[] { value } );
    }
    static void ClearXP( GameBattleUnit unit )
    {
        Set( unit, "tempExp", (short)0 ); Set( unit, "easyExp", 0 ); Set( unit, "easyDeferredExp", 0 );
        unit.getUnitBase().Exp = 0;
    }
    IEnumerator Start()
    {
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            yield return null;
        }
        previousMode = GameSetting.instance.mode; previousLanguage = GameSetting.instance.location;
        foreach ( string key in keys ) prefs[key] = !PlayerPrefs.HasKey( key ) ? null : key == "touchScale" ? (object)PlayerPrefs.GetFloat( key ) : PlayerPrefs.GetInt( key );
        var run = Verify();
        while ( true )
        {
            object current;
            try { if ( !run.MoveNext() ) break; current = run.Current; }
            catch ( Exception e ) { report.error = e.ToString(); break; }
            yield return current;
        }
        GameSetting.instance.mode = previousMode; GameSetting.instance.location = previousLanguage;
        foreach ( var pair in prefs )
            if ( pair.Value == null ) PlayerPrefs.DeleteKey( pair.Key );
            else if ( pair.Key == "touchScale" ) PlayerPrefs.SetFloat( pair.Key, (float)pair.Value );
            else PlayerPrefs.SetInt( pair.Key, (int)pair.Value );
        PlayerPrefs.Save();
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        Application.Quit( report.error.Length == 0 ? 0 : 1 );
    }

    IEnumerator Verify()
    {
        Require( (int)GameSetting.GameMode.Normal == 0 && (int)GameSetting.GameMode.Hard == 1 && (int)GameSetting.GameMode.Easy == 2,
            "legacy mode values retained; Easy appended" );
        var help = GameHelpUI.instance;
        help.show();
        foreach ( bool traditional in new[] { false, true } )
        {
            GameSetting.instance.location = traditional ? GameSetting.GameLocation.TraditionalChinese : GameSetting.GameLocation.SimplifiedChinese;
            GameSetting.instance.mode = GameSetting.GameMode.Easy;
            string[] labels = traditional ? new[] { "簡單", "中等", "困難" } : new[] { "简单", "中等", "困难" };
            foreach ( string label in labels )
            {
                help.updateSettingText();
                Require( help.transform.Find( "setting00" ).GetComponent<Text>().text == label, "mode label " + label );
                help.onSetting0();
            }
            Require( GameSetting.instance.mode == GameSetting.GameMode.Easy, "three-mode confirm cycle " + traditional );
        }
        GameSetting.instance.save(); GameSetting.instance.mode = GameSetting.GameMode.Hard; GameSetting.instance.load();
        Require( GameSetting.instance.mode == GameSetting.GameMode.Easy, "Easy preference reload" );
        foreach ( int old in new[] { 0, 1 } )
        {
            PlayerPrefs.SetInt( "mode", old ); GameSetting.instance.load();
            Require( (int)GameSetting.instance.mode == old, "legacy preference " + old );
        }
        help.unShow();
        var desk = SwordsmanChapterTestDesk.Instance;
        SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.None;
        GameSetting.instance.mode = GameSetting.GameMode.Normal;
        yield return desk.Enter( desk.Catalog.entries[1], 1, "battle" );
        float deadline = Time.realtimeSinceStartup + 120;
        while ( !desk.CheckpointCaptured && desk.LaunchError == null && Time.realtimeSinceStartup < deadline ) yield return null;
        Require( desk.LaunchError == null && desk.CheckpointCaptured, "real chapter one player turn" );
        Require( GameSetting.PersistentDataPath.Contains( "EasyValidation" ), "dedicated validation save isolation" );
        var user = GameBattleUnitManager.instance.User[0];
        var enemy = GameBattleUnitManager.instance.Enemy[0];
        foreach ( int p in new[] { 0, 24, 25, 60, 200 } )
        {
            GameUserData.instance.Proficiency = p;
            GameSetting.instance.mode = GameSetting.GameMode.Normal;
            float normal = GameDefine.Difficulty;
            Require( Mathf.Approximately( normal, p / 60f + 0.8f ), "unchanged normal formula P=" + p );
            enemy.updateUnitData(); string stats = EnemyStats( enemy );
            GameSetting.instance.mode = GameSetting.GameMode.Easy; enemy.updateUnitData();
            Require( Mathf.Approximately( GameDefine.Difficulty, normal ) && stats == EnemyStats( enemy ), "Easy retains enemy base stats and identity P=" + p );
            GameSetting.instance.mode = GameSetting.GameMode.Hard;
            Require( Mathf.Approximately( GameDefine.Difficulty, Mathf.Max( normal + 0.1f, 1f ) ), "unchanged hard formula P=" + p );
        }
        foreach ( var mode in new[] { GameSetting.GameMode.Normal, GameSetting.GameMode.Hard, GameSetting.GameMode.Easy } )
        {
            GameSetting.instance.mode = mode;
            bool easy = mode == GameSetting.GameMode.Easy;
            Require( GameEasyRules.Damage( enemy, user, 100 ) == ( easy ? 90 : 100 ), "incoming " + mode );
            Require( GameEasyRules.Damage( user, enemy, 100 ) == ( easy ? 110 : 100 ), "outgoing " + mode );
            Require( GameEasyRules.Damage( user, enemy, 0 ) == 0, "zero and immunity " + mode );
            var physical = new GameBattleAttackResultPhysical { HP = 100, HP2 = 50 };
            GameEasyRules.Physical( physical, enemy, user );
            Require( physical.HP == ( easy ? 90 : 100 ) && physical.HP2 == ( easy ? 45 : 50 ), "double and counter damage " + mode );
            var results = new List<GameBattleAttackResultSkill> {
                new GameBattleAttackResultSkill { unit = enemy, type = GameSkillResutlType.Damage, HP = 100 },
                new GameBattleAttackResultSkill { unit = user, type = GameSkillResutlType.Cure, HP = 100 },
                new GameBattleAttackResultSkill { unit = user, type = GameSkillResutlType.Special, other = GameSkillOtherEffect.KillSelf, HP = 100 },
                new GameBattleAttackResultSkill { unit = enemy, type = GameSkillResutlType.Special, other = GameSkillOtherEffect.AttackPoison, HP = 100 } };
            GameEasyRules.Skills( results, user );
            Require( results[0].HP == ( easy ? 110 : 100 ) && results[3].HP == ( easy ? 110 : 100 ) && results[1].HP == 100 && results[2].HP == 100,
                "magic and special damage; heal and self-sacrifice excluded " + mode );
            ClearXP( user ); user.addExpSkill( user );
            Require( user.TempExp == ( easy ? 55 : 50 ), "actual support XP generator " + mode );
            user.addExp(); Require( user.getUnitBase().Exp == ( easy ? 55 : 50 ), "displayed XP equals payout " + mode );
        }
        GameSetting.instance.mode = GameSetting.GameMode.Easy;
        help.onSetting0(); Require( GameSetting.instance.mode == GameSetting.GameMode.Easy && !GameEasyRules.CanChangeDifficulty, "battle difficulty change rejected" );
        user.initHPMP( 1, 0 ); ClearXP( user );
        GameEasyRules.Recover( user );
        Require( user.HP == 1 + user.HPMax / 10 && user.MP == user.MPMax / 20 && user.TempExp == 0, "turn supplies exact percentages without XP" );
        user.initHPMP(); GameEasyRules.Recover( user );
        Require( user.HP == user.HPMax && user.MP == user.MPMax, "recovery caps" );
        user.initHPMP( 1, 0 ); GameBattleTurn.instance.start( 2 ); GameBattleTurn.instance.RecoverEasyTurn();
        Require( user.HP == 1 && user.MP == 0, "loading current turn does not grant duplicate supply" );
        Set( GameBattleTurn.instance, "turn", 3 ); GameBattleTurn.instance.RecoverEasyTurn(); int recovered = user.HP;
        GameBattleTurn.instance.RecoverEasyTurn(); Require( user.HP == recovered && recovered > 1, "supply once per new player turn" );
        user.initHPMP( 1, 0 ); user.addHP( -1 ); GameEasyRules.Recover( user );
        Require( user.IsKilled && user.HP == 0, "lethal damage remains lethal; recovery never revives" );

        yield return desk.Enter( desk.Catalog.entries[1], 1, "battle" );
        deadline = Time.realtimeSinceStartup + 120;
        while ( !desk.CheckpointCaptured && desk.LaunchError == null && Time.realtimeSinceStartup < deadline ) yield return null;
        user = GameBattleUnitManager.instance.User[0]; enemy = GameBattleUnitManager.instance.Enemy[0];
        GameSetting.instance.mode = GameSetting.GameMode.Easy;
        ClearXP( user ); int level = user.LV, spirit = user.getUnitBase().BaseSpiritPower;
        Award( user, 1500 ); Require( user.TempExp == 1650, "large action receives exactly 110 percent once" ); user.addExp();
        GameUnitLevelUpData.instance.levelUp( user ); bool completed = false;
        GameLevelUpUI.instance.show( () => completed = true, user, GameUnitLevelUpData.instance.LastData );
        GameLevelUpUI.instance.onClick(); GameLevelUpUI.instance.onClick(); GameLevelUpUI.instance.onClick();
        Require( completed && user.LV == level + 3 && user.getUnitBase().Exp == 150 && user.getUnitBase().BaseSpiritPower == spirit + 6,
            "real level UI drains multiple levels; five-spirit remains two per level" );
        ClearXP( user ); Award( user, 100000 ); user.addExp(); int iterations = 0;
        while ( user.IsLevelUp && iterations++ < 100 ) GameUnitLevelUpData.instance.levelUp( user );
        Require( user.LV == 99 && user.getUnitBase().Exp == 0 && iterations < 100 && !user.IsLevelUp,
            "short overflow avoided through level 99; original growth and skills used" );
        Award( user, 100 ); Require( user.TempExp == 0, "max level earns no further XP" );
        foreach ( var mode in new[] { GameSetting.GameMode.Normal, GameSetting.GameMode.Hard, GameSetting.GameMode.Easy } )
        {
            GameSetting.instance.mode = mode;
            GameUserData.instance.setStage( 34 ); GameUserData.instance.Proficiency = 24;
            GameUserData.instance.ProficiencyStage = 1; GameUserData.instance.ReloadBattleCount = 1;
            GameBattleJudgment.instance.UpdateProficiency();
            Require( GameUserData.instance.Proficiency == 24 && GameUserData.instance.ReloadBattleCount == 0,
                "earned proficiency after retry " + mode );
        }
        GameUserData.instance.setStage( 1 );
        GameUserData.instance.saveBattle(); GameUserData.instance.loadBattle();
        yield return null; yield return null;
        while ( GameSceneManager.instance.IsLoading ) yield return null;
        Require( new FileInfo( Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "SaveBattle.dat" ) ).Length == 102400,
            "battle save roundtrip retains original binary size" );
        // All authored escort loss targets are discovered from original stage data, without a new NPC stat table.
        int escorts = 0;
        GameSetting.instance.mode = GameSetting.GameMode.Easy;
        for ( int stage = 0; stage < desk.Catalog.entries.Length; stage++ )
        {
            var data = GameBattleData.instance.getStage( stage );
            bool hasEscort = false;
            foreach ( var lose in data.Judge.JudgeLose )
            {
                if ( lose.Judge != GameBattleJudgeJudgeType.KillOne || lose.ID < 0 || lose.ID >= data.Man.Length ) continue;
                var man = data.Man[lose.ID];
                if ( man.BattleAIType != GameBattleAIType.None && GameUnitData.instance.getData( man.UnitBase.UnitID ).UnitCampType == GameUnitCampType.User ) hasEscort = true;
            }
            if ( !hasEscort ) continue;
            yield return desk.Enter( desk.Catalog.entries[stage], desk.Catalog.entries[stage].suggestedLevel, "inspect" );
            Require( desk.LaunchError == null, "escort scene entry " + stage );
            foreach ( var npc in GameBattleUnitManager.instance.Npc )
            {
                bool named = false;
                foreach ( var lose in data.Judge.JudgeLose ) if ( lose.Judge == GameBattleJudgeJudgeType.KillOne && lose.ID == npc.BattleManID ) named = true;
                Require( GameEasyRules.ProtectedTarget( npc ) == named && GameEasyRules.Damage( null, npc, 100 ) == ( named ? 90 : 100 ),
                    "only authored escort NPC protected: stage=" + stage + " man=" + npc.BattleManID );
                if ( named ) escorts++;
            }
        }
        Require( escorts > 0, "real escort targets verified without changing loss rules" );
    }
    static string EnemyStats( GameBattleUnit enemy )
    {
        return enemy.HPMax + ":" + enemy.MPMax + ":" + enemy.Str + ":" + enemy.Vit + ":" + enemy.Avg + ":" + enemy.Int + ":" + enemy.MoveMax + ":" + enemy.IsBoss;
    }
}
#endif
