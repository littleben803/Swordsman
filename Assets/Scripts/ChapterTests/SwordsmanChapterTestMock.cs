using System;
using System.Collections.Generic;
using UnityEngine;

#if SWORDSMAN_CHAPTER_TESTS && (DEVELOPMENT_BUILD || UNITY_EDITOR)
[Flags]
public enum SwordsmanChapterMockOption
{
    None = 0, InfiniteHP = 1, InfiniteMP = 2, AllSkills = 4,
    BestWeapon = 8, BestArmor = 16, BestAccessory = 32, HiddenTreasure = 64
}

public static class SwordsmanChapterTestMock
{
    public static SwordsmanChapterMockOption Pending;
    public static SwordsmanChapterMockOption Active { get; private set; }
    static GameUnitBase[] baseline;
    static readonly HashSet<GameUnitBase> applied = new HashSet<GameUnitBase>();
    public static readonly short[] Weapons = { 13, 32, 55, 44, 65, 76, 84, 35, 92, 94 };
    public static readonly short[] Armors = { 111, 135, 123, 134, 123, 146, 112, 138, 152, 154 };
    // Explicit test presets: melee characters use 鬼神金冠; eligible casters use 七色璎珞.
    public static readonly short[] Accessories = { 195, 216, 195, 216, 195, 195, 216, 216, 195, 195 };
    public static bool Enabled( SwordsmanChapterMockOption option ) { return ( Active & option ) != 0; }
    public static bool Protects( GameBattleUnit unit, SwordsmanChapterMockOption option )
    { return Enabled( option ) && unit != null && unit.IsUser; }
    public static string Profile { get { return ((int)Active).ToString( "X2" ); } }

    public static void BeginEntry()
    {
        Active = Pending;
        baseline = null;
        applied.Clear();
    }

    static GameUnitBase Clone( GameUnitBase unit )
    { return JsonUtility.FromJson<GameUnitBase>( JsonUtility.ToJson( unit ) ); }

    public static void PrepareParty()
    {
        if ( Active == SwordsmanChapterMockOption.None || baseline != null ) return;
        baseline = new GameUnitBase[GameDefine.MAX_USER];
        for ( int i = 0; i < baseline.Length; i++ ) baseline[i] = Clone( GameUserData.instance.getUnitBase( i ) );
        for ( int i = 0; i < baseline.Length; i++ ) ApplyUnit( GameUserData.instance.getUnitBase( i ) );
    }

    public static void AfterBattleEntry()
    {
        if ( Active == SwordsmanChapterMockOption.None ) return;
        PrepareParty();
        // Loading a legacy snapshot restores per-unit equipment after initUnits.
        // Apply once after the complete load, without repeatedly overriding later story events.
        applied.Clear();
        for ( int i = 0; i < GameDefine.MAX_USER; i++ ) ApplyUnit( GameUserData.instance.getUnitBase( i ) );
        foreach ( GameBattleUnit unit in GameBattleUnitManager.instance.User )
        {
            ApplyUnit( unit.getUnitBase() );
            unit.updateUnitData();
        }
    }

    public static GameUnitBase ForBattle( GameBattleUnit unit, GameUnitBase original )
    {
        if ( Active == SwordsmanChapterMockOption.None ) return original;
        PrepareParty();
        if ( !unit.IsUser ) return Clone( baseline[unit.UnitID] );
        ApplyUnit( original );
        return original;
    }

    static void Equip( GameUnitBase unit, short id, GameItemType type )
    {
        GameItem item = GameItemData.instance.getData( id );
        if ( item == null || item.ItemType != type || !item.canEquip( unit.UnitID ) )
            throw new InvalidOperationException( "Invalid Mock equipment preset for unit " + unit.UnitID );
        short old = type == GameItemType.Weapon ? unit.Weapon : type == GameItemType.Armor ? unit.Armor : unit.Accessory;
        int slot = Array.IndexOf( unit.Items, id );
        if ( slot < 0 )
        {
            slot = Array.IndexOf( unit.Items, old );
            if ( slot < 0 ) slot = Array.IndexOf( unit.Items, GameDefine.INVALID_ID );
            if ( slot < 0 ) throw new InvalidOperationException( "Mock equipment needs a free inventory slot." );
            unit.Items[slot] = id;
        }
        if ( type == GameItemType.Weapon ) unit.Weapon = id;
        else if ( type == GameItemType.Armor ) unit.Armor = id;
        else unit.Accessory = id;
    }

    static void ApplyUnit( GameUnitBase unit )
    {
        int id = unit.UnitID;
        if ( id < 0 || id >= GameDefine.MAX_USER || !applied.Add( unit ) ) return;
        if ( Enabled( SwordsmanChapterMockOption.AllSkills ) )
        {
            for ( int i = 0; i < unit.SpiritPower.Length; i++ ) unit.SpiritPower[i] = GameDefine.MAX_POWER;
            short[] skills = FullSkills( unit );
            unit.Skills = skills;
        }
        if ( Enabled( SwordsmanChapterMockOption.BestWeapon ) ) Equip( unit, Weapons[id], GameItemType.Weapon );
        if ( Enabled( SwordsmanChapterMockOption.BestArmor ) ) Equip( unit, Armors[id], GameItemType.Armor );
        if ( Enabled( SwordsmanChapterMockOption.BestAccessory ) ) Equip( unit, Accessories[id], GameItemType.Accessory );
    }

    public static short[] FullSkills( GameUnitBase unit )
    {
        var list = new List<short>();
        GameUnitLevelUp learn = GameUnitLevelUpData.instance.getData( unit.UnitID );
        if ( learn != null ) foreach ( GameUnitLevelUpSkillLearn entry in learn.Skill )
            if ( entry != null && entry.SkillID >= 0 && !list.Contains( entry.SkillID ) ) list.Add( entry.SkillID );
        GameUnitBase initial = GameUnitInitData.instance.getData( unit.UnitID );
        if ( initial != null ) foreach ( short id in initial.Skills )
            if ( id >= 0 && !list.Contains( id ) ) list.Add( id );
        foreach ( short id in unit.Skills ) if ( id >= 0 && !list.Contains( id ) ) list.Add( id );
        while ( list.Count < GameDefine.MAX_SLOT ) list.Add( GameDefine.INVALID_ID );
        return list.ToArray();
    }

    public static string Summary
    {
        get
        {
            if ( Active == SwordsmanChapterMockOption.None ) return "辅助关闭";
            var parts = new List<string>();
            if ( Enabled( SwordsmanChapterMockOption.InfiniteHP ) ) parts.Add( "无限血" );
            if ( Enabled( SwordsmanChapterMockOption.InfiniteMP ) ) parts.Add( "无限气" );
            if ( Enabled( SwordsmanChapterMockOption.AllSkills ) ) parts.Add( "全技能" );
            if ( Enabled( SwordsmanChapterMockOption.BestWeapon ) ) parts.Add( "武器" );
            if ( Enabled( SwordsmanChapterMockOption.BestArmor ) ) parts.Add( "防具" );
            if ( Enabled( SwordsmanChapterMockOption.BestAccessory ) ) parts.Add( "饰品" );
            if ( Enabled( SwordsmanChapterMockOption.HiddenTreasure ) ) parts.Add( "隐藏宝物" );
            return "Mock：" + string.Join( " / ", parts );
        }
    }
}
#endif
