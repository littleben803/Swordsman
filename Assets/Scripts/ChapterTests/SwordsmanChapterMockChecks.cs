using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
public static class SwordsmanChapterMockChecks
{
    [Serializable] public sealed class Report
    {
        public List<string> passed = new List<string>();
        public string error;
        public int[] weapons;
        public int[] armors;
        public int[] accessories;
        public bool runtimeComplete;
    }
    static Report report;
    static string output;
    static bool Check( string label, Action action )
    {
        try { action(); report.passed.Add( label ); return true; }
        catch ( Exception e ) { report.error = label + ": " + e; Finish( false ); return false; }
    }
    static void Require( bool value, string message ) { if ( !value ) throw new InvalidOperationException( message ); }
    static void Finish( bool success )
    {
        report.runtimeComplete = success;
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        Application.Quit( success ? 0 : 1 );
    }
    static int VisibleTreasureCount()
    {
        Transform root = GameObject.Find( "BattleScene/Treasures" ).transform;
        int count = 0;
        foreach ( Transform child in root ) if ( child.gameObject.activeSelf ) count++;
        return count;
    }
    static GameBattleUnit Player { get { return GameBattleUnitManager.instance.User[0]; } }
    static SwordsmanChapterMockOption All { get { return (SwordsmanChapterMockOption)127; } }

    static void CheckSkillSaveCompatibility()
    {
        var unit = new GameUnitBase();
        for ( int i = 0; i < GameDefine.MAX_SLOT; i++ ) unit.Skills[i] = (short)i;
        unit.addSkill( 10 ); unit.addSkill( 10 );
        Require( unit.Skills.Length == 11 && unit.hasSkill( 10 ), "Learning the eleventh skill failed or duplicated it" );
        byte[] legacy = new byte[3000];
        for ( int i = 0; i < 2922; i++ ) legacy[i] = (byte)( i % 251 );
        byte[] extended = (byte[])legacy.Clone();
        GameSkillSaveExtension.Write( extended, 2922, new[] { unit } );
        for ( int i = 0; i < 2922; i++ ) Require( legacy[i] == extended[i], "Legacy core layout changed" );
        Require( GameSkillSaveExtension.Read( extended, 2922, new[] { unit } ) && unit.hasSkill( 10 ) && unit.Skills.Length == 11, "Extended skill save did not round-trip" );
        Require( !GameSkillSaveExtension.Read( legacy, 2922, new[] { unit } ) && unit.Skills.Length == 10, "Old save retained stale extra skills" );
        byte[] corrupt = (byte[])extended.Clone(); corrupt[corrupt.Length - 49] ^= 0x7f;
        Require( !GameSkillSaveExtension.Read( corrupt, 2922, new[] { unit } ) && unit.Skills.Length == 10, "Corrupt extension was accepted" );
        unit.addSkill( 10 ); bool refused = false;
        try { GameSkillSaveExtension.Write( new byte[3000], 2999, new[] { unit } ); }
        catch ( InvalidDataException ) { refused = true; }
        Require( refused, "Extension overlapped legacy fields" );
    }

    public static IEnumerator Run( SwordsmanChapterTestDesk desk, string path )
    {
        report = new Report(); output = path;
        var catalog = desk.Catalog;
        if ( !Check( "formal-eleventh-skill-and-legacy-save-extension", CheckSkillSaveCompatibility ) ) yield break;
        if ( !Check( "equipment-preset-compatibility", () => {
            for ( int i = 0; i < GameDefine.MAX_USER; i++ )
            {
                foreach ( short id in new[] { SwordsmanChapterTestMock.Weapons[i], SwordsmanChapterTestMock.Armors[i], SwordsmanChapterTestMock.Accessories[i] } )
                    Require( GameItemData.instance.getData( id ).canEquip( i ), "Equipment not allowed for unit " + i );
            }
        } ) ) yield break;
        SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.None;
        yield return desk.Enter( catalog.entries[1], catalog.entries[1].suggestedLevel, "battle" );
        int baseHP = 0, baseMP = 0; short baseWeapon = -1, baseArmor = -1, baseAccessory = -1;
        string normalRoot = SwordsmanChapterTestSession.RootDirectory;
        if ( !Check( "baseline-and-hidden-off", () => {
            Require( desk.LaunchError == null && desk.CheckpointCaptured, "Baseline battle snapshot unavailable" );
            baseHP = Player.HPMax; baseMP = Player.MPMax; baseWeapon = Player.WeaponID; baseArmor = Player.ArmorID; baseAccessory = Player.AccessoryID;
            Require( baseHP < 9999 && baseMP < 9999, "Unexpected baseline values" );
            GameBattleManager.instance.updateTreasures();
            Require( VisibleTreasureCount() == 1, "Hidden-off must show only the ordinary gold" );
            Require( GameBattleManager.instance.MapEvent.ContainsKey( 2 ), "Hiding the marker removed the reward" );
        } ) ) yield break;
        var enemies = new Dictionary<int, string>();
        foreach ( GameBattleUnit unit in GameBattleUnitManager.instance.Enemy )
            enemies[unit.BattleManID] = unit.HPMax + ":" + unit.MPMax + ":" + unit.WeaponID + ":" + unit.ArmorID;

        foreach ( SwordsmanChapterMockOption option in new[] { SwordsmanChapterMockOption.InfiniteHP, SwordsmanChapterMockOption.InfiniteMP } )
        {
            SwordsmanChapterTestMock.Pending = option;
            yield return desk.Enter( catalog.entries[1], catalog.entries[1].suggestedLevel, "battle" );
            if ( !Check( option.ToString(), () => {
                Require( desk.LaunchError == null && desk.CheckpointCaptured, "Mock checkpoint unavailable" );
                Require( SwordsmanChapterTestSession.RootDirectory != normalRoot && SwordsmanChapterTestSession.RootDirectory.Contains( "Mock" ), "Mock save isolation missing" );
                if ( option == SwordsmanChapterMockOption.InfiniteHP )
                {
                    Player.addHP( -1000000 ); Require( Player.HP == 9999 && Player.HPMax == 9999 && Player.IsAlive, "Lethal damage was not blocked" );
                    Require( Player.MPMax == baseMP, "HP toggle affected MP" );
                }
                else
                {
                    Player.addMP( -1000000 ); Require( Player.MP == 9999 && Player.MPMax == 9999, "MP consumption was not blocked" );
                    Require( Player.HPMax == baseHP, "MP toggle affected HP" );
                }
            } ) ) yield break;
        }
        SwordsmanChapterTestMock.Pending = All;
        yield return desk.Enter( catalog.entries[1], catalog.entries[1].suggestedLevel, "battle" );
        if ( !Check( "combined-skills-gear-and-enemy-isolation", () => {
            Require( desk.LaunchError == null, "Combined Mock entry failed" );
            for ( int i = 0; i < GameDefine.MAX_USER; i++ )
            {
                GameUnitBase unit = GameUserData.instance.getUnitBase( i );
                Require( unit.Weapon == SwordsmanChapterTestMock.Weapons[i] && unit.Armor == SwordsmanChapterTestMock.Armors[i] && unit.Accessory == SwordsmanChapterTestMock.Accessories[i], "Equipment preset mismatch" );
                for ( int k = 0; k < unit.SpiritPower.Length; k++ ) Require( unit.SpiritPower[k] == GameDefine.MAX_POWER, "Spirit stat not maxed" );
                var learn = GameUnitLevelUpData.instance.getData( i );
                if ( learn != null ) foreach ( var skill in learn.Skill )
                    if ( skill != null && skill.SkillID >= 0 ) Require( Array.IndexOf( SwordsmanChapterTestMock.FullSkills( unit ), skill.SkillID ) >= 0, "A native learnable skill is missing" );
            }
            GameBattleUnit caster = GameBattleUnitManager.instance.User.Find( unit => unit.UnitID == 1 );
            Require( caster != null && caster.Skill.Length == 11 && caster.getUnitBase().Skills.Length == 11, "Eleven-skill runtime list or extended skill data failed" );
            GameBattleSkillUI.instance.show(); GameBattleSkillUI.instance.setData( caster ); GameBattleSkillUI.instance.NextSkillPage();
            Require( GameBattleSkillUI.instance.SkillPage == 1 && GameBattleSkillUI.instance.getSelectionSkill() != null, "Second skill page is not selectable" );
            GameBattleSkillUI.instance.unShow();
            GameUserData.instance.saveBattle(); GameUserData.instance.loadBattle();
            Require( GameUserData.instance.getUnitBase( 1 ).Skills.Length == 11, "Actual battle save lost the eleventh skill" );
            Require( new FileInfo( Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "SaveBattle.dat" ) ).Length == 102400, "Battle file size changed" );

            foreach ( GameBattleUnit enemy in GameBattleUnitManager.instance.Enemy )
                Require( enemies[enemy.BattleManID] == enemy.HPMax + ":" + enemy.MPMax + ":" + enemy.WeaponID + ":" + enemy.ArmorID, "Mock changed an enemy" );
            Require( VisibleTreasureCount() == 2, "Hidden-on must show two reward locations" );
            GameBattleManager.instance.updateTreasures();
            Require( VisibleTreasureCount() == 2, "Refresh duplicated treasure markers" );
        } ) ) yield break;
        if ( !Check( "extended-ordinary-save-write", () => {
            GameUserData.instance.save( 0 );
            Require( new FileInfo( Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "Save0.dat" ) ).Length == 3000, "Ordinary file size changed" );
            GameUserData.instance.load( 0 );
        } ) ) yield break;
        float deadline = Time.realtimeSinceStartup + 25;
        while ( ( SceneManager.GetActiveScene().buildIndex != 2 || GameSceneManager.instance.IsLoading ) && Time.realtimeSinceStartup < deadline ) yield return null;
        if ( !Check( "extended-ordinary-save-load", () => {
            Require( SceneManager.GetActiveScene().buildIndex == 2, "Extended ordinary save did not reach camp" );
            Require( GameUserData.instance.getUnitBase( 1 ).Skills.Length == 11 && GameUserData.instance.getUnitBase( 3 ).Skills.Length == 11, "Ordinary save lost a caster's eleventh skill" );
        } ) ) yield break;
        SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.None;
        yield return desk.Enter( catalog.entries[1], catalog.entries[1].suggestedLevel, "battle" );
        if ( !Check( "all-off-restores-baseline", () => {
            Require( Player.HPMax == baseHP && Player.MPMax == baseMP && Player.WeaponID == baseWeapon && Player.ArmorID == baseArmor && Player.AccessoryID == baseAccessory, "Off did not restore original starting profile" );
            Require( SwordsmanChapterTestSession.RootDirectory == normalRoot, "Normal directory not restored" );
            Require( VisibleTreasureCount() == 1, "Hidden marker remained after off" );
        } ) ) yield break;
        SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.HiddenTreasure;
        yield return desk.Enter( catalog.entries[4], catalog.entries[4].suggestedLevel, "inspect" );
        if ( !Check( "shared-reward-four-cells", () => {
            GameBattleManager.instance.updateTreasures();
            Require( VisibleTreasureCount() == 8, "Fourth chapter should have eight marked cells" );
            Player.setPos( 20, 22 );
            GameBattleManager.instance.checkMapEvent( Player, null );
            Require( !GameBattleManager.instance.MapEvent.ContainsKey( 8 ), "Shared reward was not removed" );
        } ) ) yield break;
        yield return null; yield return null;
        if ( !Check( "shared-pickup-refresh-and-save", () => {
            GameBattleManager.instance.updateTreasures();
            Require( VisibleTreasureCount() == 4, "Shared marker group did not disappear" );
            GameUserData.instance.saveBattle();
            GameUserData.instance.loadBattle();
            Require( !GameBattleManager.instance.MapEvent.ContainsKey( 8 ), "Save/reload respawned shared reward" );
            Require( VisibleTreasureCount() == 4, "Reload marker count mismatch" );
        } ) ) yield break;
        yield return null;
        yield return desk.Enter( catalog.entries[19], catalog.entries[19].suggestedLevel, "inspect" );
        if ( !Check( "no-treasure-map", () => {
            GameBattleManager.instance.updateTreasures();
            Require( VisibleTreasureCount() == 0, "No-treasure map has phantom markers" );
        } ) ) yield break;
        SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.None;
        Finish( true );
    }
}
#endif
