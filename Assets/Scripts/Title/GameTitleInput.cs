using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;




public class GameTitleInput : Singleton<GameTitleInput>
{
    float time = 0.0f;

    bool confirm = false;
    bool cancel = false;

    


    void Update()
    {
        confirm = false;
        cancel = false;

        if ( GameInputManager.getKeyDown( GameInputCode.Confirm ) )
        {
            confirm = true;
        }
        if ( GameInputManager.getKeyDown( GameInputCode.Cancel ) )
        {
            cancel = true;
        }

        updateMovie();

        updateSLUI();


        updateHelpUI();

        updateTitleUI();
    }

    void moveSLUI()
    {
        bool bMove = false;

        if ( GameSLUI.instance.IsShowAskUI )
        {
            if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
            {
                bMove = true;
            }
            if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
            {
                bMove = true;
            }

            if ( !bMove )
            {
                return;
            }

            GameSLUI.instance.showAskUI( !GameSLUI.instance.IsOKAskUI );

            return;
        }


        int selection = GameSLUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            return;
        }



        if ( !bMove )
        {
            return;
        }

        GameSLUI.instance.select( selection );
    }





    void moveTitleUI()
    {
        bool bMove = false;

        int selection = GameTitleUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameTitleUI.instance.select( selection );
    }

    void moveHelpUI()
    {
        bool bMove = false;

        int selection = GameHelpUI.instance.Selection;

        if ( GameInputManager.getKeyDown( GameInputCode.Up ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Down ) )
        {
            selection++;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Left ) )
        {
            selection--;
            bMove = true;
        }

        if ( GameInputManager.getKeyDown( GameInputCode.Right ) )
        {
            selection++;
            bMove = true;
        }

        if ( !bMove )
        {
            return;
        }

        GameHelpUI.instance.select( selection );
    }


    void updateMovie()
    {
        if ( !GameMovieManager.instance.IsPlaying )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameMovieManager.instance.onClick();

            time = 0.0f;

            return;
        }

        if ( cancel )
        {
            cancel = false;

            GameMovieManager.instance.onClick();

            time = 0.0f;

            return;
        }
    }

    void updateSLUI()
    {
        if ( !GameSLUI.instance.IsShow || GameSLUI.instance.IsFadeIn ||
            GameBlackUI.instance.IsShow )
        {
            return;
        }

        if ( confirm )
        {
            confirm = false;

            GameSLUI.instance.onClick();

            return;
        }

        if ( cancel )
        {
            cancel = false;

            if ( GameSLUI.instance.IsShowAskUI )
            {
                GameSLUI.instance.unShowAskUI();
            }
            else
            {
                GameSLUI.instance.clear();
                GameSLUI.instance.AutoUnshow = false;
                GameSLUI.instance.unShowFade();

                GameBlackUI.instance.showBlack( 1.0f , onShowBlackOverUnShowSLUI );
            }

            return;
        }

        moveSLUI();
    }



    void onShowBlackOverUnShowSLUI()
    {
        GameSLUI.instance.unShow();
        GameSLUI.instance.updateUIStageFadeOver();

        GameTitleUI.instance.Pause = false;
        GameTitleUI.instance.show();
        GameTitleUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void onShowBlackOverLoadGame()
    {
        GameTitleUI.instance.unShowFade();

        GameSLUI.instance.show( GameSLType.Load );
        GameSLUI.instance.showFade();

        GameBlackUI.instance.unShowBlack( 0.5f , null );
    }

    void onNewGame()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Battle ,
                            GameSceneLoadMode.StartBattle );
    }

    void onLoadBattle()
    {
        GameSceneManager.instance.loadScene( GameSceneType.Battle ,
                           GameSceneLoadMode.LoadBattle );
    }

    void updateTitleUI()
    {
        if ( !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn || GameTitleUI.instance.Pause )
        {
            return;
        }

        if ( GameSLUI.instance.IsShow || 
            GameBlackUI.instance.IsShow ||
            !GameTitleUI.instance.IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( confirm )
        {
            confirm = false;

            switch ( GameTitleUI.instance.Selection )
            {
                case 0:
                    {
                        if ( time < 1.0f )
                        {
                            return;
                        }

                        GameUserData.instance.setStage( 0 );

                        GameBlackUI.instance.showBlack( 0.5f , onNewGame );
                    }
                    break;
                case 1:
                    {
                        GameBlackUI.instance.showBlack( 1 , onShowBlackOverLoadGame );
                    }
                    break;
                case 2:
                    {
                        string path = GameSetting.PersistentDataPath + "/SaveBattle.dat";

                        if ( !File.Exists( path ) )
                        {
                            return;
                        }

                        GameBlackUI.instance.showBlack( 0.5f , onLoadBattle );
                    }
                    break;
                case 3:
                    {
                        GameHelpUI.instance.show();
                        GameHelpUI.instance.select( 0 );
                    }
                    break;
            }


            return;
        }

        if ( cancel )
        {
            cancel = false;

            return;
        }

        moveTitleUI();
    }


    void updateHelpUI()
    {
        if ( !GameHelpUI.instance.IsShow )
        {
            return;
        }

        if ( GameSLUI.instance.IsShow ||
            GameBlackUI.instance.IsShow ||
            !GameHelpUI.instance.IsShow )
        {
            return;
        }

        time += Time.deltaTime;

        if ( confirm )
        {
            confirm = false;

            GameHelpUI.instance.activateSelection();

            return;
        }

        if ( cancel )
        {
            cancel = false;
            GameHelpUI.instance.unShow();
            return;
        }

        moveHelpUI();
    }

}


