using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameTouchRightUI : GameUI<GameTouchRightUI>
{
    Transform pageUp, pageDown;
    public override void initSingleton()
    {
        GameVirtualKeyboardSkin.ApplyRight( transform );
        pageUp = transform.Find( "ButtonC" );
        pageDown = transform.Find( "ButtonD" );
        RefreshPageButtons();
    }
    public void showUI()
    {
        RefreshPageButtons();

        show();
        showFade();

        transform.localScale = new Vector3( GameSetting.instance.touchScale , GameSetting.instance.touchScale , GameSetting.instance.touchScale );
    }

    public static bool NeedsPageButtons()
    {
        if ( !GameSetting.instance.virtualKeyboardEnabled || GameSceneManager.instance == null ) return false;
        var scene = GameSceneManager.instance.SceneType;
        if ( scene != GameSceneType.Camp && scene != GameSceneType.Rpg ) return false;
        if ( GameBlackUI.instance != null && GameBlackUI.instance.IsShow ||
             GameMsgBoxUI.instance != null && GameMsgBoxUI.instance.IsShow ||
             GameMsgBoxChooseUI.instance != null && GameMsgBoxChooseUI.instance.IsShow ||
             GameHelpUI.instance != null && GameHelpUI.instance.IsShow ) return false;
        if ( scene == GameSceneType.Camp && GameCampInput.instance != null && GameCampInput.instance.pause ||
             scene == GameSceneType.Rpg && GameRPGInput.instance != null && GameRPGInput.instance.pause ) return false;
        if ( scene == GameSceneType.Rpg && GameShopUI.instance != null && GameShopUI.instance.IsShow )
            return GameShopUI.instance.IsPagingListVisible && !GameShopUI.instance.IsFadeIn && !GameShopUI.instance.isShowAskUI();
        if ( scene == GameSceneType.Rpg && GameRPGEventManager.instance != null && GameRPGEventManager.instance.IsShow ) return false;
        if ( GameItemUI.instance != null && GameItemUI.instance.IsShow )
            return !GameItemUI.instance.IsFadeIn && !GameItemUI.instance.isShowItemAskUI() && !GameItemUI.instance.isShowAskUI();
        return scene == GameSceneType.Camp && GameAlchemyUI.instance != null && GameAlchemyUI.instance.IsShow &&
            !GameAlchemyUI.instance.IsFadeIn && !GameAlchemyUI.instance.isShowAskUI();
    }

    public void RefreshPageButtons()
    {
        bool visible = NeedsPageButtons();
        if ( pageUp == null || pageDown == null ) return;
        if ( pageUp.gameObject.activeSelf != visible || pageDown.gameObject.activeSelf != visible )
        {
            // Release only paging, so hiding a held X/Y cannot leave a queued page operation.
            if ( !visible && GameTouchManager.instance != null ) GameTouchManager.instance.ReleasePaging();
            pageUp.gameObject.SetActive( visible );
            pageDown.gameObject.SetActive( visible );
        }
        GameVirtualKeyboardSkin.LayoutRight( transform, visible );
    }

    void LateUpdate() { RefreshPageButtons(); }

}

