using UnityEngine;
using UnityEngine.UI;

// Presentation only. EasyTouch retains the input callbacks and direction-pad geometry.
public static class GameVirtualKeyboardSkin
{
    static Sprite pad, buttonA, buttonB;

    public static void ApplyLeft( Transform root )
    {
        if ( pad == null ) pad = Resources.Load<Sprite>( "VirtualKeyboard/Bronze/DPad" );
        var control = root.GetComponentInChildren<ETCDPad>( true );
        if ( control == null || pad == null ) return;
        control.normalSprite = control.pressedSprite = pad;
        control.GetComponent<Image>().sprite = pad;
        // Opacity is carried by the PNG: translucent faces and more solid metal details.
        control.GetComponent<Image>().color = Color.white;
        // Match visible left/bottom padding to the existing B-right/A-bottom padding.
        // Keep the 230x230 input rect and its center dead zone; only move it with the artwork.
        control.GetComponent<RectTransform>().anchoredPosition = new Vector2( 101.44f, 99.39f );
    }

    public static void ApplyRight( Transform root )
    {
        if ( buttonA == null ) buttonA = Resources.Load<Sprite>( "VirtualKeyboard/Bronze/A" );
        if ( buttonB == null ) buttonB = Resources.Load<Sprite>( "VirtualKeyboard/Bronze/B" );
        ApplyButton( root.Find( "ButtonA" ), buttonA, new Vector2( -111, 45 ) );
        ApplyButton( root.Find( "ButtonB" ), buttonB, new Vector2( -45, 111 ) );
        ApplyLabeledButton( root.Find( "ButtonA" ), "Ent", 0 );
        ApplyLabeledButton( root.Find( "ButtonB" ), "Esc", 0 );
        ApplyButton( root.Find( "ButtonC" ), buttonA, Vector2.zero );
        ApplyButton( root.Find( "ButtonD" ), buttonA, Vector2.zero );
        ApplyLabeledButton( root.Find( "ButtonC" ), "", 1 );
        ApplyLabeledButton( root.Find( "ButtonD" ), "", -1 );
    }

    public static void LayoutRight( Transform root, bool paging )
    {
        ((RectTransform)root.Find( "ButtonA" )).anchoredPosition = paging ? new Vector2( -45, 45 ) : new Vector2( -111, 45 );
        ((RectTransform)root.Find( "ButtonB" )).anchoredPosition = paging ? new Vector2( -45, 129 ) : new Vector2( -45, 111 );
        ((RectTransform)root.Find( "ButtonC" )).anchoredPosition = new Vector2( -45, 297 );
        ((RectTransform)root.Find( "ButtonD" )).anchoredPosition = new Vector2( -45, 213 );
    }

    static void ApplyLabeledButton( Transform target, string letter, int pageDirection )
    {
        var image = target.GetComponent<Image>();
        image.enabled = false; // ETCButton still owns this Image and its original input callbacks.
        var go = new GameObject( "BronzeFace", typeof(RectTransform), typeof(CanvasRenderer), typeof(GameVirtualKeyboardPageFace) );
        go.layer = target.gameObject.layer;
        go.transform.SetParent( target, false );
        var rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.sizeDelta = Vector2.zero;
        var face = go.GetComponent<GameVirtualKeyboardPageFace>();
        face.Source = image; face.PageDirection = pageDirection; face.raycastTarget = true;
        var label = target.GetComponentInChildren<Text>( true );
        label.gameObject.SetActive( true ); label.transform.SetAsLastSibling();
        label.text = letter; label.font = Resources.GetBuiltinResource<Font>( "Arial.ttf" );
        foreach ( var effect in label.GetComponents<BaseMeshEffect>() ) effect.enabled = false;
        label.fontSize = 28; label.resizeTextForBestFit = false; label.alignment = TextAnchor.MiddleCenter;
        label.color = new Color32( 247, 225, 168, 255 ); label.raycastTarget = false;
        label.rectTransform.anchorMin = Vector2.zero; label.rectTransform.anchorMax = Vector2.one;
        label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
        label.gameObject.SetActive( pageDirection == 0 );
    }

    static void ApplyButton( Transform target, Sprite sprite, Vector2 position )
    {
        if ( target == null || sprite == null ) return;
        var control = target.GetComponent<ETCButton>();
        if ( control == null ) return;
        control.normalSprite = control.pressedSprite = sprite;
        control.normalColor = Color.white;
        control.pressedColor = new Color( 1, 0.87f, 0.66f, 1 );
        var image = target.GetComponent<Image>();
        image.sprite = sprite;
        image.color = Color.white;
        // Letters are part of the approved skin. Keep legacy labels as inactive fallback objects.
        foreach ( Text label in target.GetComponentsInChildren<Text>( true ) ) label.gameObject.SetActive( false );
        var rect = (RectTransform)target;
        // User-approved smaller targets leave room for a closer pair without covering either face.
        rect.sizeDelta = new Vector2( 80, 80 );
        rect.anchoredPosition = position;
    }
}
