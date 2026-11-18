using UnityEngine;
using UnityEngine.UI;

public class GameHelpUI : GameUI<GameHelpUI>
{
    public const string StudioHomepageUrl = "https://www.ideaboxapps.com/";
    public static string StudioInformation( bool traditional )
    {
        return traditional
            ? "點擊訪問「Biyou Village Studio」\n馬村長歡迎您加入「碧遊村」\n微信：27771640"
            : "点击访问「Biyou Village Studio」\n马村长欢迎您加入「碧游村」\n微信：27771640";
    }
    public const int DialogueSelection = 1;
    public const int LanguageSelection = 2;
    public const int KeyboardSelection = 3;
    public const int ShrinkSelection = 4;
    public const int GrowSelection = 5;
    public const int CloseSelection = 6;
    readonly Text[] text = new Text[7];
    int selection;
    Color color;
    Text setting00;
    Text setting10;
    Text keyboardValue;
    Text dialogueValue;
    Text sizeLabel;
    Text studioInformation;
    readonly Button[] sizeButtons = new Button[2];
    readonly Button[] difficultyButtons = new Button[2];

    public int Selection { get { return selection; } }

    public override void initSingleton()
    {
        Image background = transform.Find( "back" ).GetComponent<Image>();
        background.sprite = SwordsmanHd4xSpriteResolver.Resolve( background.sprite );
        CreateDialogueRow();
        text[0] = transform.Find( "setting0/text" ).GetComponent<Text>();
        text[DialogueSelection] = transform.Find( "dialogue/text" ).GetComponent<Text>();
        text[LanguageSelection] = transform.Find( "setting1/text" ).GetComponent<Text>();
        text[KeyboardSelection] = transform.Find( "keyboard/text" ).GetComponent<Text>();
        text[ShrinkSelection] = transform.Find( "setting20" ).GetComponent<Text>();
        text[GrowSelection] = transform.Find( "setting21" ).GetComponent<Text>();
        text[CloseSelection] = transform.Find( "button7/text" ).GetComponent<Text>();
        setting00 = transform.Find( "setting00" ).GetComponent<Text>();
        setting10 = transform.Find( "setting10" ).GetComponent<Text>();
        keyboardValue = transform.Find( "keyboardValue" ).GetComponent<Text>();
        dialogueValue = transform.Find( "dialogueValue" ).GetComponent<Text>();
        sizeLabel = transform.Find( "setting2/text" ).GetComponent<Text>();
        studioInformation = transform.Find( "buttonB/text" ).GetComponent<Text>();
        sizeButtons[0] = transform.Find( "setting20" ).GetComponent<Button>();
        sizeButtons[1] = transform.Find( "setting21" ).GetComponent<Button>();
        difficultyButtons[0] = transform.Find( "setting0" ).GetComponent<Button>();
        difficultyButtons[1] = transform.Find( "setting00" ).GetComponent<Button>();
        color = text[0].color;
    }

    void CreateDialogueRow()
    {
        // Reuse the authored row styling at runtime; scene/Prefab GUIDs and saved UI bindings stay intact.
        var row = Instantiate( transform.Find( "setting1" ).gameObject, transform, false );
        row.name = "dialogue";
        var value = Instantiate( transform.Find( "setting10" ).gameObject, transform, false );
        value.name = "dialogueValue";
        foreach ( var node in new[] { row, value } )
        {
            foreach ( var label in node.GetComponentsInChildren<GameLocalization>( true ) ) Destroy( label );
            var button = node.GetComponent<Button>();
            if ( button == null ) button = node.AddComponent<Button>();
            button.transition = Selectable.Transition.None;
            var graphic = node.GetComponent<Graphic>();
            if ( graphic != null ) { graphic.raycastTarget = true; button.targetGraphic = graphic; }
            button.onClick = new Button.ButtonClickedEvent(); button.onClick.AddListener( onDialogueSpeed );
        }
        MoveY( "setting0", 52 ); MoveY( "setting00", 52 );
        MoveY( "dialogue", 35 ); MoveY( "dialogueValue", 35 );
        MoveY( "setting1", 18 ); MoveY( "setting10", 18 );
        MoveY( "keyboard", 1 ); MoveY( "keyboardValue", 1 );
    }

    void MoveY( string name, float y )
    {
        var rect = (RectTransform)transform.Find( name );
        rect.anchoredPosition = new Vector2( rect.anchoredPosition.x, y );
    }

    public override void onShow()
    {
        updateSettingText();
    }

    public void select( int index )
    {
        int target = ( index % text.Length + text.Length ) % text.Length;
        if ( !GameEasyRules.CanChangeDifficulty && target == 0 ) target = index < selection ? CloseSelection : DialogueSelection;
        if ( !GameSetting.instance.virtualKeyboardEnabled && ( target == ShrinkSelection || target == GrowSelection ) )
            target = index < selection ? KeyboardSelection : CloseSelection;
        selection = target;
        updateText();
    }

    public void activateSelection()
    {
        switch ( selection )
        {
            case 0: onSetting0(); break;
            case DialogueSelection: onDialogueSpeed(); break;
            case LanguageSelection: onSetting1(); break;
            case KeyboardSelection: onKeyboardToggle(); break;
            case ShrinkSelection: onSetting20(); break;
            case GrowSelection: onSetting21(); break;
            case CloseSelection: onClick7(); break;
        }
    }

    public void onSetting0()
    {
        if ( !GameEasyRules.CanChangeDifficulty ) return;
        select( 0 );
        GameSetting.instance.mode = GameSetting.instance.mode == GameSetting.GameMode.Normal
            ? GameSetting.GameMode.Hard : GameSetting.instance.mode == GameSetting.GameMode.Hard
                ? GameSetting.GameMode.Easy : GameSetting.GameMode.Normal;
        GameSetting.instance.save();
        updateSettingText();
    }

    public void onSetting1()
    {
        select( LanguageSelection );
        GameSetting.instance.location = GameSetting.instance.location == GameSetting.GameLocation.SimplifiedChinese
            ? GameSetting.GameLocation.TraditionalChinese : GameSetting.GameLocation.SimplifiedChinese;
        foreach ( GameLocalization label in GameObject.Find( "Canvas" ).GetComponentsInChildren<GameLocalization>( true ) )
            label.updateText();
        GameSetting.instance.save();
        updateSettingText();
    }

    public void onKeyboardToggle()
    {
        select( KeyboardSelection );
        GameSetting.instance.virtualKeyboardEnabled = !GameSetting.instance.virtualKeyboardEnabled;
        GameSetting.instance.save();
        GameTouchManager.instance.onTouchUp();
        GameTouchCenterUI.instance.showUI();
        updateSettingText();
    }

    public void onDialogueSpeed()
    {
        select( DialogueSelection );
        GameSetting.instance.fastDialogue = !GameSetting.instance.fastDialogue;
        GameSetting.instance.save(); updateSettingText();
    }

    public void onSetting20() { resizeKeyboard( -0.1f , ShrinkSelection ); }
    public void onSetting21() { resizeKeyboard( 0.1f , GrowSelection ); }

    void resizeKeyboard( float delta , int index )
    {
        if ( !GameSetting.instance.virtualKeyboardEnabled ) return;
        select( index );
        float scale = Mathf.Clamp( GameSetting.instance.touchScale + delta , 1.0f , 1.5f );
        GameSetting.instance.touchScale = scale;
        GameTouchLeftUI.instance.transform.localScale = Vector3.one * scale;
        GameTouchRightUI.instance.transform.localScale = Vector3.one * scale;
        GameSetting.instance.save();
    }

    public void updateSettingText()
    {
        bool traditional = GameSetting.instance.location == GameSetting.GameLocation.TraditionalChinese;
        studioInformation.text = StudioInformation( traditional );
        setting00.text = GameSetting.instance.mode == GameSetting.GameMode.Easy
            ? ( traditional ? "簡單" : "简单" ) : GameSetting.instance.mode == GameSetting.GameMode.Normal
                ? "中等" : ( traditional ? "困難" : "困难" );
        setting10.text = traditional ? "繁體" : "简体";
        text[DialogueSelection].text = traditional ? "對話速度" : "对话速度";
        dialogueValue.text = GameSetting.instance.fastDialogue ? "快速" : "正常";
        text[KeyboardSelection].text = traditional ? "虛擬鍵盤" : "虚拟键盘";
        keyboardValue.text = GameSetting.instance.virtualKeyboardEnabled
            ? ( traditional ? "打開" : "打开" ) : ( traditional ? "關閉" : "关闭" );
        sizeLabel.text = traditional ? "鍵盤大小" : "键盘大小";
        updateText();
    }

    Color opacity( float alpha )
    {
        Color value = color;
        value.a = alpha;
        return value;
    }

    public void updateText()
    {
        bool enabled = GameSetting.instance.virtualKeyboardEnabled;
        if ( !GameEasyRules.CanChangeDifficulty && selection == 0 ) selection = DialogueSelection;
        if ( !enabled && ( selection == ShrinkSelection || selection == GrowSelection ) ) selection = KeyboardSelection;
        for ( int i = 0; i < text.Length; i++ ) text[i].color = opacity( selection == i ? 1.0f : 0.7f );
        setting00.color = text[0].color;
        dialogueValue.color = text[DialogueSelection].color;
        setting10.color = text[LanguageSelection].color;
        keyboardValue.color = text[KeyboardSelection].color;
        sizeLabel.color = opacity( selection == ShrinkSelection || selection == GrowSelection ? 1.0f : 0.7f );
        foreach ( Button button in sizeButtons ) button.interactable = enabled;
        foreach ( Button button in difficultyButtons ) if ( button != null ) button.interactable = GameEasyRules.CanChangeDifficulty;
        if ( !GameEasyRules.CanChangeDifficulty ) text[0].color = setting00.color = new Color( 0.5f, 0.5f, 0.5f, 0.45f );
        if ( !enabled )
        {
            Color disabled = new Color( 0.5f , 0.5f , 0.5f , 0.45f );
            sizeLabel.color = disabled;
            text[ShrinkSelection].color = disabled;
            text[GrowSelection].color = disabled;
        }
    }

    public void onClick7() { unShow(); }
    // Existing scene button binding. A fixed HTTPS homepage, opened only on an explicit click.
    public void onClickB() { Application.OpenURL( StudioHomepageUrl ); }
}
