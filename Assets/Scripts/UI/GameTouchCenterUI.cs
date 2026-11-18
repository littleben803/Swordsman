using UnityEngine;
using UnityEngine.EventSystems;

public class GameTouchCenterUI : GameUI<GameTouchCenterUI>
{
    public void showUI()
    {
        ConfigurePointerInput();
        if ( !GameSetting.instance.virtualKeyboardEnabled )
        {
            GameTouchManager.instance.ResetControls();
            GameTouchLeftUI.instance.unShow();
            GameTouchRightUI.instance.unShow();
            GameTouchManager.instance.SetVisible( false );
            unShow();
            return;
        }

        if ( !GameTouchLeftUI.instance.IsShow || GameTouchLeftUI.instance.IsFadeOut )
            GameTouchLeftUI.instance.showUI();
        if ( !GameTouchRightUI.instance.IsShow || GameTouchRightUI.instance.IsFadeOut )
            GameTouchRightUI.instance.showUI();
        GameTouchManager.instance.SetVisible( true );
        unShow();
    }

    static void ConfigurePointerInput()
    {
#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
        // The project uses legacy Input; its historical Input System UI module cannot read pointers.
        EventSystem system = EventSystem.current;
        if ( system == null ) return;
        foreach ( BaseInputModule module in system.GetComponents<BaseInputModule>() )
            if ( module.GetType().FullName == "UnityEngine.InputSystem.UI.InputSystemUIInputModule" ) module.enabled = false;
        if ( system.GetComponent<StandaloneInputModule>() == null ) system.gameObject.AddComponent<StandaloneInputModule>();
        // Navigation/confirm already run through GameInputManager; avoid submitting twice.
        system.sendNavigationEvents = false;
#endif
    }

    public void unShowUI()
    {
        GameTouchManager.instance.ResetControls();
        GameTouchManager.instance.SetVisible( false );
        if ( !GameSetting.instance.virtualKeyboardEnabled )
        {
            GameTouchLeftUI.instance.unShow();
            GameTouchRightUI.instance.unShow();
            unShow();
            return;
        }

        if ( GameTouchLeftUI.instance.IsShow ) GameTouchLeftUI.instance.unShowFade();
        if ( GameTouchRightUI.instance.IsShow ) GameTouchRightUI.instance.unShowFade();
        show();
    }
}
