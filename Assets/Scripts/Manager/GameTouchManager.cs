using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


class GameTouchManager : SingletonManager<GameTouchManager>
{
    bool[] isDonw = new bool[ (int)GameInputCode.Count ];
    bool[] isPress = new bool[ (int)GameInputCode.Count ];

    public bool IsShow { get; set; }
    public override void initSingleton()
    {
        IsShow = false;
    }

    public void SetVisible( bool visible )
    {
        if ( IsShow != visible ) onTouchUp();
        IsShow = visible;
    }

    void OnApplicationFocus( bool focused )
    {
        if ( !focused ) ResetControls();
    }

    public void ResetControls()
    {
        onTouchUp();
        if ( GameTouchLeftUI.instance != null ) ResetControls( GameTouchLeftUI.instance.gameObject );
        if ( GameTouchRightUI.instance != null ) ResetControls( GameTouchRightUI.instance.gameObject );
    }

    static void ResetControls( GameObject root )
    {
        foreach ( ETCBase control in root.GetComponentsInChildren<ETCBase>( true ) )
        {
            bool active = control.activated;
            control.activated = false;
            control.activated = active;
        }
    }

    public bool getKeyDown( GameInputCode c )
    {
        bool b = isDonw[ (int)c ];
        isDonw[ (int)c ] = false;

        return b;
    }

    public bool getKey( GameInputCode c )
    {
        bool b = isPress[ (int)c ];
        isPress[ (int)c ] = false;

        return b;
    }

    public void onTouchUp()
    {
        for ( int i = 0 ; i < (int)GameInputCode.Count ; i++ )
        {
            isDonw[ i ] = false;
            isPress[ i ] = false;
        }
    }

    public void OnButtonADown()
    {
        isDonw[ (int)GameInputCode.Confirm ] = true;
    }

    public void OnButtonBDown()
    {
        isDonw[ (int)GameInputCode.Cancel ] = true;
    }


    public void OnDownUp1()
    {
        isDonw[ (int)GameInputCode.Up1 ] = true;
    }
    public void ReleasePaging()
    {
        isDonw[(int)GameInputCode.Up1] = isPress[(int)GameInputCode.Up1] = false;
        isDonw[(int)GameInputCode.Down1] = isPress[(int)GameInputCode.Down1] = false;
    }
    public void OnDownDown1()
    {
        isDonw[ (int)GameInputCode.Down1 ] = true;
    }
    public void OnDownUp()
    {
        isDonw[ (int)GameInputCode.Up ] = true;
    }
    public void OnDownDown()
    {
        isDonw[ (int)GameInputCode.Down ] = true;
    }
    public void OnDownLeft()
    {
        isDonw[ (int)GameInputCode.Left ] = true;
    }
    public void OnDownRight()
    {
        isDonw[ (int)GameInputCode.Right ] = true;
    }


    public void OnPressUp1()
    {
        isPress[ (int)GameInputCode.Up1 ] = true;
    }
    public void OnPressDown1()
    {
        isPress[ (int)GameInputCode.Down1 ] = true;
    }
    public void OnPressUp()
    {
        isPress[ (int)GameInputCode.Up ] = true;
    }
    public void OnPressDown()
    {
        isPress[ (int)GameInputCode.Down ] = true;
    }
    public void OnPressLeft()
    {
        isPress[ (int)GameInputCode.Left ] = true;
    }
    public void OnPressRight()
    {
        isPress[ (int)GameInputCode.Right ] = true;
    }

}

