using UnityEngine;
using UnityEngine.UI;

public class GameTitleUI : GameUI<GameTitleUI>
{
    Text[] text = new Text[ 4 ];
    Text agreement;
    Text version;
    Sprite viewportBackground;

    int selection = 0;

    Color color;
    
    public int Selection { get { return selection; } }



    public override void initSingleton()
    {
        Transform background = transform.Find( "background" );
        Image backgroundImage = background == null ? null : background.GetComponent<Image>();
        if ( backgroundImage != null )
        {
            backgroundImage.sprite = SwordsmanHd4xSpriteResolver.Resolve( backgroundImage.sprite );
            ExpandTitleBackgroundViewport( backgroundImage );
        }

        text[ 0 ] = transform.Find( "title/new" ).GetComponent<Text>();
        text[ 1 ] = transform.Find( "title/load" ).GetComponent<Text>();
        text[ 2 ] = transform.Find( "title/loadBattle" ).GetComponent<Text>();
        text[ 3 ] = transform.Find( "title/help" ).GetComponent<Text>();

        agreement = transform.Find( "agreement" ).GetComponent<Text>();
        version = transform.Find( "version" ).GetComponent<Text>();

        color = text[ 0 ].color;
    }

    // Extend the approved backdrop downward without scaling or moving its existing artwork.
    void ExpandTitleBackgroundViewport( Image image )
    {
        Sprite source = image.sprite;
        if ( source == null || source.name != "Title_t0" || !Mathf.Approximately( source.pixelsPerUnit, 4 ) || source.texture.width != 2048 ) return;
        RectTransform rect = image.rectTransform;
        Vector2 size = rect.sizeDelta;
        if ( !Mathf.Approximately( size.x, 321 ) || !Mathf.Approximately( size.y, 191 ) ) return;
        float height = source.rect.height * 200.0f / size.y;
        float added = height - source.rect.height;
        Rect expanded = new Rect( source.rect.x, source.rect.y - added, source.rect.width, height );
        if ( expanded.yMin < 0 || expanded.yMax > source.texture.height )
            throw new System.InvalidOperationException( "Title viewport texture bounds are invalid." );
        Vector2 pivot = new Vector2( source.pivot.x / source.rect.width, source.pivot.y / source.rect.height );
        viewportBackground = Sprite.Create( source.texture, expanded, pivot, source.pixelsPerUnit, 0, SpriteMeshType.FullRect, source.border );
        viewportBackground.name = source.name;
        image.sprite = viewportBackground;
        rect.anchoredPosition += new Vector2( 0, ( size.y - 200.0f ) * 0.5f );
        rect.sizeDelta = new Vector2( size.x, 200 );
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.LogFormat( "SWORDSMAN_TITLE_VIEWPORT rect={0} size={1} position={2} texelScaleBefore={3} texelScaleAfter={4}",
            expanded, rect.sizeDelta, rect.anchoredPosition, size.y / source.rect.height, 200.0f / expanded.height );
#endif
    }

    void OnDestroy()
    {
        if ( viewportBackground != null ) Destroy( viewportBackground );
    }

    public override void onShow()
    {
        version.text = GameSetting.instance.getVersion();
        agreement.text = GameStringData.instance.getString( GameStringType.Agreement0 ) + "\n" + SystemInfo.deviceUniqueIdentifier;
    }

    public void select( int i )
    {
        if ( i < 0 )
        {
            i = 3;
        }

        if ( i > 3 )
        {
            i = 0;
        }

        selection = i;

        updateText();
    }

    public void updateText()
    {
        for ( int i = 0 ; i < 4 ; i++ )
        {
            Color c = color;

            if ( selection == i )
            {
                c.a = 1.0f;
                text[ i ].color = c;
            }
            else
            {
                c.a = 0.3f;
                text[ i ].color = c;
            }
        }
    }

}
