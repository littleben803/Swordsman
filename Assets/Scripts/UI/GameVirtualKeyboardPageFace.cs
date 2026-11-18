using UnityEngine;
using UnityEngine.UI;

// Native UI mesh reuses A's original bronze rim and translucent brown pixels, without its baked letter.
// No texture copies, importer changes or extra raster assets are needed for labels/page icons.
public sealed class GameVirtualKeyboardPageFace : MaskableGraphic
{
    public Image Source;
    public int PageDirection;
    public override Texture mainTexture { get { return Source != null && Source.sprite != null ? Source.sprite.texture : Texture2D.whiteTexture; } }

    protected override void OnEnable()
    {
        base.OnEnable();
        if ( Source != null ) Source.enabled = false;
    }

    void LateUpdate()
    {
        // EasyTouch re-enables its original Image on every OnEnable. Keep only the letter-free mesh visible.
        if ( Source != null && Source.enabled ) Source.enabled = false;
        if ( Source != null && color != Source.color ) color = Source.color;
    }

    protected override void OnPopulateMesh( VertexHelper mesh )
    {
        mesh.Clear();
        Rect rect = GetPixelAdjustedRect();
        Vector2 center = rect.center;
        float size = Mathf.Min( rect.width, rect.height );
        const int segments = 96;
        // Original A is a 705px canvas with its outer rim at ~308px radius.
        // The annulus samples original UVs; the disk samples only the letter-free brown material.
        for ( int i = 0; i <= segments; i++ )
        {
            float angle = i * Mathf.PI * 2 / segments;
            var direction = new Vector2( Mathf.Cos( angle ), Mathf.Sin( angle ) );
            mesh.AddVert( center + direction * size * 0.438f, color, Vector2.one * 0.5f + direction * 0.438f );
            mesh.AddVert( center + direction * size * 0.375f, color, Vector2.one * 0.5f + direction * 0.375f );
            mesh.AddVert( center + direction * size * 0.375f, color, new Vector2( 0.265f, 0.5f ) + direction * 0.015f );
            if ( i == 0 ) continue;
            int a = ( i - 1 ) * 3, b = i * 3;
            mesh.AddTriangle( a, b, a + 1 ); mesh.AddTriangle( a + 1, b, b + 1 );
        }
        int middle = mesh.currentVertCount;
        mesh.AddVert( center, color, new Vector2( 0.265f, 0.5f ) );
        for ( int i = 0; i < segments; i++ ) mesh.AddTriangle( middle, i * 3 + 2, ( i + 1 ) * 3 + 2 );
        if ( PageDirection != 0 )
        {
            float direction = PageDirection > 0 ? 1 : -1;
            Stroke( mesh, center, size, direction, new Vector2( -0.17f, 0.22f ), new Vector2( 0.17f, 0.22f ) );
            Stroke( mesh, center, size, direction, new Vector2( 0, -0.20f ), new Vector2( 0, 0.12f ) );
            Stroke( mesh, center, size, direction, new Vector2( -0.135f, -0.015f ), new Vector2( 0, 0.12f ) );
            Stroke( mesh, center, size, direction, new Vector2( 0, 0.12f ), new Vector2( 0.135f, -0.015f ) );
        }
    }

    void Stroke( VertexHelper mesh, Vector2 center, float size, float direction, Vector2 from, Vector2 to )
    {
        from.y *= direction; to.y *= direction;
        Vector2 axis = ( to - from ).normalized;
        Vector2 normal = new Vector2( -axis.y, axis.x ) * 0.019f;
        // Sample an opaque ivory-gold pixel from the original A glyph, retaining the existing material palette.
        Vector2 uv = new Vector2( 0.5f, 0.432f );
        int first = mesh.currentVertCount;
        mesh.AddVert( center + ( from + normal ) * size, color, uv );
        mesh.AddVert( center + ( to + normal ) * size, color, uv );
        mesh.AddVert( center + ( to - normal ) * size, color, uv );
        mesh.AddVert( center + ( from - normal ) * size, color, uv );
        mesh.AddTriangle( first, first + 1, first + 2 ); mesh.AddTriangle( first, first + 2, first + 3 );
    }
}
