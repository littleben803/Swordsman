using System;
using System.Collections.Generic;

// No serialized state: the original two modes return before any new calculation.
public static class GameEasyRules
{
    public static bool Enabled { get { return GameSetting.instance.mode == GameSetting.GameMode.Easy; } }
    public static bool CanChangeDifficulty
    {
        get { return GameSceneManager.instance == null || GameSceneManager.instance.SceneType != GameSceneType.Battle; }
    }

    public static bool PartyMember( GameBattleUnit unit )
    {
        return unit != null && unit.IsUser && !unit.IsSummon && unit.UnitID < GameDefine.MAX_USER;
    }

    public static bool ProtectedTarget( GameBattleUnit unit )
    {
        if ( PartyMember( unit ) ) return true;
        if ( unit == null || !unit.IsNpc || unit.IsSummon || GameBattleManager.instance == null ) return false;
        var stage = GameBattleManager.instance.ActiveBattleStage;
        if ( stage == null || stage.Judge == null ) return false;
        // Only named allied NPCs whose death explicitly fails this stage receive escort assistance.
        foreach ( var lose in stage.Judge.JudgeLose )
            if ( lose.Judge == GameBattleJudgeJudgeType.KillOne && lose.ID == unit.BattleManID ) return true;
        return false;
    }

    public static int Damage( GameBattleUnit source, GameBattleUnit target, int damage )
    {
        if ( !Enabled || damage <= 0 ) return damage;
        if ( ProtectedTarget( target ) ) return Math.Max( 1, (int)( (long)damage * 9 / 10 ) );
        if ( PartyMember( source ) && target != null && target.IsEnemy ) return Math.Max( 1, (int)( (long)damage * 11 / 10 ) );
        return damage;
    }

    public static void Physical( GameBattleAttackResultPhysical result, GameBattleUnit source, GameBattleUnit target )
    {
        if ( !Enabled ) return;
        result.HP = Damage( source, target, result.HP );
        result.HP2 = Damage( source, target, result.HP2 );
    }

    public static void Skills( List<GameBattleAttackResultSkill> results, GameBattleUnit source )
    {
        if ( !Enabled ) return;
        foreach ( var result in results )
        {
            bool damage = result.type == GameSkillResutlType.Damage;
            if ( result.type == GameSkillResutlType.None || result.type == GameSkillResutlType.Special )
            {
                switch ( result.other )
                {
                    case GameSkillOtherEffect.AttackX2:
                    case GameSkillOtherEffect.AttackX3:
                    case GameSkillOtherEffect.AttackX4:
                    case GameSkillOtherEffect.AttackPalsy:
                    case GameSkillOtherEffect.AttackPoison:
                    case GameSkillOtherEffect.AttackHP:
                    case GameSkillOtherEffect.AttackFetter: damage = true; break;
                }
            }
            // Cure, mana, HealAll and KillSelf retain their original meaning.
            if ( damage ) result.HP = Damage( source, result.unit, result.HP );
        }
    }

    public static void Recover( GameBattleUnit unit )
    {
        if ( !Enabled || !PartyMember( unit ) || !unit.IsAlive ) return;
        unit.addHP( Math.Max( 0, Math.Min( unit.HPMax - unit.HP, Math.Max( 1, unit.HPMax / 10 ) ) ) );
        unit.addMP( Math.Max( 0, Math.Min( unit.MPMax - unit.MP, unit.MPMax / 20 ) ) );
    }
}
