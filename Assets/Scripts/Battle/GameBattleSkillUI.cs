using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameBattleSkillUI : GameUI<GameBattleSkillUI>
{
    GameBattleSkillUISlot[] slots = new GameBattleSkillUISlot[ GameDefine.MAX_SLOT ];
    Text description;
    int selection = 0;
    GameBattleUnit skillUnit;
    int skillPage;
    GUIStyle skillPageStyle;
    public int SkillPage { get { return skillPage; } }
    public void NextSkillPage()
    {
        if ( skillUnit == null || !IsShow ) return;
        skillPage = ( skillPage + 1 ) % Mathf.CeilToInt( skillUnit.Skill.Length / (float)GameDefine.MAX_SLOT );
        setData( skillUnit );
    }
    protected override void onUpdate()
    {
        base.onUpdate();
        if ( IsShow && skillUnit != null && skillUnit.Skill.Length > GameDefine.MAX_SLOT &&
            Input.GetKeyDown( KeyCode.Tab ) ) NextSkillPage();
    }
    void OnGUI()
    {
        if ( !IsShow || skillUnit == null || skillUnit.Skill.Length <= GameDefine.MAX_SLOT ) return;
        if ( skillPageStyle == null )
        {
            skillPageStyle = new GUIStyle { fontSize = 17, alignment = TextAnchor.MiddleCenter };
            foreach ( GUIStyleState state in new[] { skillPageStyle.normal, skillPageStyle.hover, skillPageStyle.active, skillPageStyle.focused } )
            { state.background = Texture2D.whiteTexture; state.textColor = Color.white; }
        }
        Matrix4x4 old = GUI.matrix; Color color = GUI.backgroundColor;
        GUI.matrix = Matrix4x4.Scale( new Vector3( Screen.width / 1280f, Screen.height / 800f, 1 ) );
        Rect rect = new Rect( 330, 16, 240, 36 );
        GUI.backgroundColor = rect.Contains( Event.current.mousePosition ) ? new Color( 0.25f, 0.55f, 0.9f ) : new Color( 0.05f, 0.35f, 0.7f );
        if ( GUI.Button( rect, "技能页 " + ( skillPage + 1 ) + "/" + Mathf.CeilToInt( skillUnit.Skill.Length / 10f ) + " [Tab]", skillPageStyle ) ) NextSkillPage();
        GUI.backgroundColor = color; GUI.matrix = old;
    }


    public int Selection { get { return selection; } }

    public override void initSingleton()
    {
        description = transform.Find( "description/text" ).GetComponent<Text>();

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ] = transform.Find( "slot" + i ).GetComponent< GameBattleSkillUISlot >();
        }
    }

    
    public GameSkill getSelectionSkill()
    {
        if ( selection == GameDefine.INVALID_ID )
        {
            return null;
        }

        if ( !slots[ selection ].Enabled )
        {
            return null;
        }

        return slots[ selection ].Skill;
    }

    public void select( int s )
    {
        if ( s < 0 || s >= GameDefine.MAX_SLOT )
        {
            return;
        }

        if ( slots[ s ].Skill == null )
        {
            return;
        }

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].selection( false );
        }

        selection = s;
        slots[ selection ].selection( true );

        description.text = slots[ s ].Skill.Description;
    }

    public void setData( GameBattleUnit unit )
    {
        short[] skills = unit.Skill;
        int offset = 0;
        if ( skillUnit != unit || !IsShow ) skillPage = 0;
        skillUnit = unit;
        offset = skillPage * GameDefine.MAX_SLOT;
        description.text = "";

        selection = GameDefine.INVALID_ID;

        for ( int i = 0 ; i < GameDefine.MAX_SLOT ; i++ )
        {
            slots[ i ].clear();

            if ( i + offset >= skills.Length || skills[ i + offset ] == GameDefine.INVALID_ID )
            {
                continue;
            }

            GameSkill m = GameSkillData.instance.getData( skills[ i + offset ] );

            slots[ i ].setData( m );
            slots[ i ].enable( unit.canUseSkill( m ) );

            if ( i == 0 )
            {
                select( 0 );
            }
        }
    }


}

