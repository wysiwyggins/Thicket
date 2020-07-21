using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Vision : MonoBehaviour
{

    public int viewRange;
    Vector3Int[] Results;
    Grid grid;

    private Tilemap overlayTilemap;
    //fog
    private Tilemap fogTilemap;
    public Tile fogTile;
    public Tile highlight;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        overlayTilemap = GameObject.Find("Overlays").GetComponent<Tilemap>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
        viewRange = 10;
        UpdateFog();

    }



    public void UpdateFog()
    {
        Vector3Int PiecePosition = fogTilemap.WorldToCell(transform.position);
        Vector3Int center = HexCoordinates.OffsetToCube(PiecePosition);
        
        //add fog
        foreach (var position in fogTilemap.cellBounds.allPositionsWithin)
        {
            fogTilemap.SetTile(position, fogTile);
        }
        //this would remove fog tiles from field of view

        Debug.Log("Attempting field of view with a center of " + center + " and a range of " + viewRange);
        Vector3Int[] viewHexes = HexCoordinates.GetFieldOfView(center, viewRange);
        foreach (Vector3Int coord in viewHexes)
        {
            fogTilemap.SetTile(HexCoordinates.CubeToOffset(coord), null);
           
        }


    }

}
