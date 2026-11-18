using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameCampUI : GameUI<GameCampUI>
{
    GameAnimation gameAnimation;
    RectTransform transBackground;

    GameCampScript script;

    public override void initSingleton()
    {
        transBackground = transform.Find( "Background" ).GetComponent<RectTransform>();
    }

    void clear()
    {
        GameDefine.DestroyAll( transBackground );
    }
    
    public override void onUnShow()
    {
        clear();
    }

    public void show( GameCampScript s )
    {
        script = s;

        show();

        string path = "Prefab/Camp/Image/camp" + GameDefine.getString2( script.Camp );

        GameObject gameOjbect = SwordsmanHd4xResources.LoadPrefab( path );
        GameObject obj = Instantiate( gameOjbect , transBackground );
        Transform trans = obj.transform;
        trans.localPosition = new Vector3( 0.0f , 0.0f , 0.0f );
        trans.localScale = new Vector3( 1.0f , 1.0f , 1.0f );

        gameAnimation = obj.GetComponent<GameAnimation>();
        gameAnimation.UI = true;
        gameAnimation.offsetX = -(int)( GameCameraManager.instance.sceneWidthHalf - GameCameraManager.instance.xOffset );
        gameAnimation.offsetY = (int)( GameCameraManager.instance.sceneHeightHalf );
        if ( script.Camp == 0 )
        {
            restoreCamp00Background();
        }
        gameAnimation.playAnimation();

        GameCampSelectUI.instance.show( 0 );
        GameCampSelectUI.instance.showFade();
    }

    void restoreCamp00Background()
    {
        var room = SwordsmanHd4xResources.LoadPrefab( "Prefab/Camp/Image/Camp10" );
        if ( room == null )
        {
            Debug.LogWarning( "Camp00 room background is unavailable." );
            return;
        }

        var source = room.GetComponent<GameAnimation>();
        var roomLayer = source.saf1[ 0 ].saf11[ 0 ];
        int index = gameAnimation.sprites.Length;
        Array.Resize( ref gameAnimation.sprites, index + 1 );
        Array.Resize( ref gameAnimation.saf2, index + 1 );
        gameAnimation.sprites[ index ] = source.sprites[ roomLayer.textureID ];
        gameAnimation.saf2[ index ] = source.saf2[ roomLayer.textureID ];
        gameAnimation.safHead.count2 = (short)gameAnimation.sprites.Length;

        // Camp10 shares the foreground placement and timing with Camp00.
        var background = new GameAnimation.SAF11
        {
            textureID = (ushort)index,
            textureX = roomLayer.textureX,
            textureY = roomLayer.textureY,
            textureType = roomLayer.textureType,
            unknow8 = roomLayer.unknow8,
            unknow10 = roomLayer.unknow10,
            unknow12 = roomLayer.unknow12
        };
        foreach ( var frame in gameAnimation.saf1 )
        {
            var layers = new GameAnimation.SAF11[ frame.saf11.Length + 1 ];
            layers[ 0 ] = background;
            Array.Copy( frame.saf11, 0, layers, 1, frame.saf11.Length );
            frame.saf11 = layers;
            frame.count = (short)layers.Length;
        }
    }

    void onLeave()
    {
        if ( script.Town != GameDefine.INVALID_ID )
        {
            GameUserData.instance.setTown( script.Town );
            GameSceneManager.instance.loadScene( GameSceneType.Rpg , GameSceneLoadMode.CampBack );
        }
        else
        {
            GameUserData.instance.setStage( GameUserData.instance.NextStage );
            GameSceneManager.instance.loadScene( GameSceneType.Battle , GameSceneLoadMode.StartBattle );
        }
    }

    public void leave()
    {
        GameBlackUI.instance.showBlack( 1.0f , onLeave );
    }


}


