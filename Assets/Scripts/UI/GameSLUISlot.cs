using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;


class GameSLUISlot : MonoBehaviour
{
    Image imageBackground;
    Image image;
    Text text;
    Text lvText;

    Text Proficiency0;
    Text Proficiency1;
    Text Turn0;
    Text Turn1;
    Text time;

    [ SerializeField]
    bool selection;

    [SerializeField]
    float color = 1.0f;
    [SerializeField]
    bool fade = false;

    private void Awake()
    {
        image = transform.Find( "image" ).GetComponent<Image>();
        text = transform.Find( "text" ).GetComponent<Text>();
        lvText = transform.Find( "lv" ).GetComponent<Text>();

        Proficiency0 = transform.Find( "Proficiency0" ).GetComponent<Text>();
        Proficiency1 = transform.Find( "Proficiency1" ).GetComponent<Text>();
        Turn0 = transform.Find( "Turn0" ).GetComponent<Text>();
        Turn1 = transform.Find( "Turn1" ).GetComponent<Text>();
        time = transform.Find( "time" ).GetComponent<Text>();

        Proficiency0.gameObject.SetActive( false );
        Proficiency1.gameObject.SetActive( false );
        Turn0.gameObject.SetActive( false );

        // Render the label and value together so the whole row shares the time's right edge.
        RectTransform turnRect = Turn1.rectTransform;
        turnRect.anchoredPosition = new Vector2( 112.0f , -8.0f );
        turnRect.sizeDelta = new Vector2( 140.0f , 32.0f );
        Turn1.alignment = TextAnchor.MiddleRight;

        // Keep both time lines inside the slot, above the separate turn-count row.
        RectTransform timeRect = time.rectTransform;
        timeRect.anchoredPosition = new Vector2( 112.0f , 5.0f );
        timeRect.sizeDelta = new Vector2( 140.0f , 32.0f );
        time.fontSize = 10;
        time.alignment = TextAnchor.UpperRight;
        time.horizontalOverflow = HorizontalWrapMode.Wrap;
        time.verticalOverflow = VerticalWrapMode.Truncate;

        imageBackground = GetComponent<Image>();
    }


    public void clear()
    {
        imageBackground.color = Color.white;
        image.color = Color.white;

        selection = false;
        fade = true;
        color = 1.0f;
    }

    public void select()
    {
        selection = true;
        fade = true;
        color = 1.0f;
    }

    public void setData( GameSaveDataInfo info )
    {
        if ( info.Stage == 0 )
        {
            image.gameObject.SetActive( false );
            text.text = "";
            lvText.text = "";
            Proficiency0.text = "";
            Proficiency1.text = "";
            Turn0.text = "";
            Turn1.text = "";
            time.text = "";
            return;
        }

        selection = false;

        image.gameObject.SetActive( true );

        GameBattleStage stage = GameBattleData.instance.getStage( info.Stage );
        text.text = stage.SDES.Title;
        lvText.text = GameDefine.getBigInt( info.LV.ToString() );

        Turn1.text = GameStringData.instance.getString( GameStringType.SL4 ) +
            GameDefine.getBigInt( info.TurnCount.ToString() );

        DateTime dt = DateTime.Parse( "1970-01-01 00:00:00" ).AddSeconds( info.TimeData );
        time.text = dt.ToLocalTime().ToString( "yyyy-MM-dd HH:mm" , System.Globalization.CultureInfo.InvariantCulture );

        // Play time is a duration: keep total hours even after the first 24 hours.
        int totalMinutes = Math.Max( 0 , info.Time ) / 60;
        time.text += "\n" + GameStringData.instance.getString( GameStringType.Time0 ) +
            string.Format( System.Globalization.CultureInfo.InvariantCulture , "{0:D2}:{1:D2}" , totalMinutes / 60 , totalMinutes % 60 );
    }

    void Update()
    {
        if ( !selection )
        {
            return;
        }

        float f = 0.3f;

        if ( fade )
        {
            color -= Time.deltaTime * f;

            if ( color < 0.7f )
            {
                color = 0.7f;
                fade = false;
            }
        }
        else
        {
            color += Time.deltaTime * f;

            if ( color > 0.9f )
            {
                color = 0.9f;
                fade = true;
            }
        }

        Color c = new Color( color , 1.0f , 1.0f );
        imageBackground.color = c;
        image.color = c;
    }

}

