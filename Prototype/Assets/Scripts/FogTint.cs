using UnityEngine;
using UnityEngine.Tilemaps;

// Tints a sprite according to the fog of war:
//  - in view: its normal colour
//  - fogged, but next to a visible hex: the fog's colour, so it disappears against fog
//    but leaves a shadowy silhouette where it overlaps visible hexes
//  - fogged and more than one hex from view: invisible
// Added at runtime by Piece and by opaque Scenery (brambles).
[RequireComponent(typeof(SpriteRenderer))]
public class FogTint : MonoBehaviour
{
    static Tilemap fogTilemap;
    static Color fogColor;

    public float fadeSpeed = 4f; // fog amount per second (4 = ~0.25s per step)

    SpriteRenderer spriteRenderer;
    Color baseColor;
    bool exempt; // the player is never fogged
    float fogAmount; // 0 = in view, 1 = fog-coloured silhouette, 2 = invisible
    float revealUntil; // Time.time until which the piece is shown regardless of fog

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        exempt = GetComponent<PlayerMovement>() != null;
    }

    void Start()
    {
        FindFog();
        fogAmount = TargetAmount(); // no fade on scene load
        Apply();
    }

    // Call when the sprite's underlying colour changes (e.g. Piece applying PieceColor).
    public void SetBaseColor(Color color)
    {
        baseColor = color;
        if (spriteRenderer != null) Apply();
    }

    // Show this sprite at full colour right away, even in fog, for `seconds`; it then fades back
    // to its normal fog state. Used so the player sees animals when one eats another.
    public void Reveal(float seconds)
    {
        revealUntil = Mathf.Max(revealUntil, Time.time + seconds);
        fogAmount = 0f;
        if (spriteRenderer != null) Apply();
    }

    // LateUpdate so it runs after movement and Vision.UpdateFog() in the same frame
    void LateUpdate()
    {
        FindFog();
        fogAmount = Mathf.MoveTowards(fogAmount, TargetAmount(), Time.deltaTime * fadeSpeed);
        Apply();
    }

    static void FindFog()
    {
        if (fogTilemap != null) return; // also re-finds after a scene change destroys the old one
        GameObject fogObject = GameObject.Find("Fog");
        if (fogObject == null) return;
        fogTilemap = fogObject.GetComponent<Tilemap>();
        if (fogTilemap == null) return;
        // rendered fog colour = tilemap colour * material tint (Sprites-Default multiplies by _Color)
        fogColor = fogTilemap.color;
        TilemapRenderer fogRenderer = fogObject.GetComponent<TilemapRenderer>();
        if (fogRenderer != null && fogRenderer.sharedMaterial != null && fogRenderer.sharedMaterial.HasProperty("_Color"))
            fogColor *= fogRenderer.sharedMaterial.color;
    }

    static bool IsVisible(Vector3Int cell)
    {
        return fogTilemap.cellBounds.Contains(cell) && !fogTilemap.HasTile(cell);
    }

    float TargetAmount()
    {
        if (exempt || fogTilemap == null || Time.time < revealUntil) return 0f;
        Vector3Int cell = fogTilemap.WorldToCell(transform.position);
        if (!fogTilemap.HasTile(cell)) return 0f;
        foreach (Vector3Int neighbor in HexCoordinates.GetNeighborsAtPos(cell))
        {
            if (IsVisible(neighbor)) return 1f;
        }
        return 2f;
    }

    void Apply()
    {
        Color tint;
        if (fogAmount <= 1f)
        {
            tint = Color.Lerp(baseColor, fogColor, fogAmount);
            tint.a = baseColor.a;
        }
        else
        {
            tint = fogColor;
            tint.a = baseColor.a * (2f - fogAmount);
        }
        spriteRenderer.color = tint;
    }
}
