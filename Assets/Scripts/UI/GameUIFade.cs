using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


public enum GameUIFadeType
{
    North = 0,
    South,
    West,
    East,

    ZoomIn,
    ZoomOut,

    Count
}


public class GameUIFade : MonoBehaviour
{
    float uiTime = 0.0f;
    bool uiStart = false;
    float uiAlpha = 0.0f;
    float uiDis = 0.0f;
    bool uiFadeIn = false;
    float uiD = 20.0f;
    float uiT = 0.3f;
    GameUIFadeType uiFadeType;
    Vector2 uiPosition;
    Vector3 uiScale;
    RectTransform uiRectTrans;

    [SerializeField]
    Image[] uiImageList = null;
    [SerializeField]
    Text[] uiTextList = null;
    [SerializeField]
    Outline[] uiOutlieList = null;
    [SerializeField]
    GameAnimation[] uiGameAnimationList = null;

    List<Color> uiImageColorList = new List<Color>();
    List<Color> uiTextColorList = new List<Color>();
    List<float> uiTextRendererAlphaList = new List<float>();
    List<Color> uiOutlineColorList = new List<Color>();
    List<Color> uiGameAnimationColorList = new List<Color>();


    public GameUIFadeType FadeInFadeType;
    public float FadeInDistance = 0;
    public float FadeInTime = 0.3f;

    public GameUIFadeType FadeOutFadeType;
    public float FadeOutDistance = 0;
    public float FadeOutTime = 0.3f;

    void Awake()
    {
        uiRectTrans = GetComponent<RectTransform>();
    }

    OnEventOver onEventOver;

    public void startUIFade( bool f , OnEventOver over )
    {
        if ( uiStart )
        {
            updateUIStageFadeOver();
        }

        onEventOver = over;

        uiFadeIn = f;
        uiFadeType = uiFadeIn ? FadeInFadeType : FadeOutFadeType;
        uiD = uiFadeIn ? FadeInDistance : FadeOutDistance;
        uiDis = uiFadeIn ? uiD : 0.0f;
        uiAlpha = uiFadeIn ? 0.0f : 1.0f;
        uiTime = 0.0f;
        uiT = uiFadeIn ? FadeInTime : FadeOutTime;

        uiStart = true;

        if ( uiRectTrans != null )
        {
            uiPosition = uiRectTrans.anchoredPosition;
            uiScale = uiRectTrans.localScale;
        }
        else
        {
            uiPosition = transform.localPosition;
            uiScale = transform.localScale;
        }

        uiImageColorList.Clear();
        uiTextColorList.Clear();
        uiTextRendererAlphaList.Clear();
        uiOutlineColorList.Clear();
        uiGameAnimationColorList.Clear();

        uiImageList = GetComponentsInChildren<Image>();
        uiTextList = GetComponentsInChildren<Text>();
        uiOutlieList = GetComponentsInChildren<Outline>();
        uiGameAnimationList = GetComponentsInChildren<GameAnimation>();

        for ( int i = 0 ; i < uiImageList.Length ; i++ )
        {
            uiImageColorList.Add( uiImageList[ i ] != null ? uiImageList[ i ].color : Color.white );
        }

        for ( int i = 0 ; i < uiTextList.Length ; i++ )
        {
            uiTextColorList.Add( uiTextList[ i ].color );
            uiTextRendererAlphaList.Add( uiTextList[ i ].canvasRenderer.GetAlpha() );
        }

        for ( int i = 0 ; i < uiOutlieList.Length ; i++ )
        {
            uiOutlineColorList.Add( uiOutlieList[ i ].effectColor );
        }

        for ( int i = 0 ; i < uiGameAnimationList.Length ; i++ )
        {
            uiGameAnimationList[ i ].pause = true;
            uiGameAnimationColorList.Add( uiGameAnimationList[ i ].getColor() );
        }

        updateUIStage();
    }

    public void updateUIStageFadeOver()
    {
        if ( uiRectTrans != null )
        {
            uiRectTrans.anchoredPosition = new Vector2( uiPosition.x , uiPosition.y );
            uiRectTrans.localScale = uiScale;
        }
        else
        {
            transform.localPosition = new Vector3( uiPosition.x , uiPosition.y , transform.localPosition.z );
            transform.localScale = uiScale;
        }


        for ( int i = 0 ; i < uiImageList.Length ; i++ )
        {
            if ( uiImageList[ i ] != null )
                uiImageList[ i ].color = uiImageColorList[ i ];
        }

        for ( int i = 0 ; i < uiTextList.Length ; i++ )
        {
            uiTextList[ i ].color = uiTextColorList[ i ];
            uiTextList[ i ].canvasRenderer.SetAlpha( uiTextRendererAlphaList[ i ] );
        }

        for ( int i = 0 ; i < uiOutlieList.Length ; i++ )
        {
            uiOutlieList[ i ].effectColor = uiOutlineColorList[ i ];
        }

        for ( int i = 0 ; i < uiGameAnimationList.Length ; i++ )
        {
            uiGameAnimationList[ i ].pause = false;
            uiGameAnimationList[ i ].setColor( uiGameAnimationColorList[ i ] );
        }


        uiImageList = null;
        uiTextList = null;
        uiOutlieList = null;
        uiGameAnimationList = null;

        uiImageColorList.Clear();
        uiTextColorList.Clear();
        uiTextRendererAlphaList.Clear();
        uiOutlineColorList.Clear();
        uiGameAnimationColorList.Clear();
    }

    void updateUIStage()
    {
        if ( !uiStart )
        {
            return;
        }

        for ( int i = 0 ; i < uiImageList.Length ; i++ )
        {
            Color c = uiImageColorList[ i ];
            c.a = uiAlpha * c.a;

            if ( uiImageList[ i ] != null )
                uiImageList[ i ].color = c;
        }

        for ( int i = 0 ; i < uiTextList.Length ; i++ )
        {
            // Fade glyphs and shadows together; Shadow can ignore the Graphic's color alpha.
            uiTextList[ i ].color = uiTextColorList[ i ];
            uiTextList[ i ].canvasRenderer.SetAlpha( uiAlpha * uiTextRendererAlphaList[ i ] );
        }

        for ( int i = 0 ; i < uiOutlieList.Length ; i++ )
        {
            Color c = uiOutlineColorList[ i ];
            // A text outline that ignores Graphic alpha already receives the renderer fade.
            if ( uiOutlieList[ i ].useGraphicAlpha || uiOutlieList[ i ].GetComponent<Text>() == null )
                c.a = uiAlpha * c.a;
            uiOutlieList[ i ].effectColor = c;
        }

        for ( int i = 0 ; i < uiGameAnimationList.Length ; i++ )
        {
            Color c = uiGameAnimationColorList[ i ];
            c.a = uiAlpha * c.a;
            uiGameAnimationList[ i ].setColor( c );
        }

        if ( uiRectTrans != null )
        {
            switch ( uiFadeType )
            {
                case GameUIFadeType.South:
                    uiRectTrans.anchoredPosition = new Vector2( uiPosition.x , uiPosition.y - uiDis );
                    break;
                case GameUIFadeType.West:
                    uiRectTrans.anchoredPosition = new Vector2( uiPosition.x + uiDis , uiPosition.y );
                    break;
                case GameUIFadeType.North:
                    uiRectTrans.anchoredPosition = new Vector2( uiPosition.x , uiPosition.y + uiDis );
                    break;
                case GameUIFadeType.East:
                    uiRectTrans.anchoredPosition = new Vector2( uiPosition.x - uiDis , uiPosition.y );
                    break;
                case GameUIFadeType.ZoomIn:
                    uiRectTrans.localScale = new Vector3( uiScale.x - uiDis , uiScale.y - uiDis , uiScale.z - uiDis );
                    break;
                case GameUIFadeType.ZoomOut:
                    uiRectTrans.localScale = new Vector3( uiScale.x + uiDis , uiScale.y + uiDis , uiScale.z + uiDis );
                    break;
            }
        }
        else
        {
            switch ( uiFadeType )
            {
                case GameUIFadeType.South:
                    transform.localPosition = new Vector3( uiPosition.x , uiPosition.y - uiDis , transform.localPosition.z );
                    break;
                case GameUIFadeType.West:
                    transform.localPosition = new Vector3( uiPosition.x + uiDis , uiPosition.y , transform.localPosition.z );
                    break;
                case GameUIFadeType.North:
                    transform.localPosition = new Vector3( uiPosition.x , uiPosition.y + uiDis , transform.localPosition.z );
                    break;
                case GameUIFadeType.East:
                    transform.localPosition = new Vector3( uiPosition.x - uiDis , uiPosition.y , transform.localPosition.z );
                    break;
                case GameUIFadeType.ZoomIn:
                    transform.localScale = new Vector3( uiScale.x - uiDis , uiScale.y - uiDis , uiScale.z - uiDis );
                    break;
                case GameUIFadeType.ZoomOut:
                    transform.localScale = new Vector3( uiScale.x + uiDis , uiScale.y + uiDis , uiScale.z + uiDis );
                    break;
            }
        }

    }

    public void updateFade()
    {
        if ( !uiStart )
        {
            return;
        }

        uiTime += Time.deltaTime;
        float progress = uiT <= 0.0f ? 1.0f : Mathf.Clamp01( uiTime / uiT );
        uiDis = uiFadeIn ? uiD * ( 1.0f - progress ) : -uiD * progress;
        uiAlpha = uiFadeIn ? progress : 1.0f - progress;

        // Apply the final position and opacity before callbacks can hide or restore the UI.
        updateUIStage();

        if ( progress >= 1.0f )
        {
            uiStart = false;
            uiTime = 0.0f;
            if ( onEventOver != null )
            {
                onEventOver();
            }
        }
    }
}

