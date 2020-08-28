using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Vision : MonoBehaviour
{

    public int viewRange;
    Vector3Int[] Results;
    Grid grid;
    Camera m_MainCamera;
    Camera_Zoomer zoomer;
    private Tilemap overlayTilemap;
    //fog
    private Tilemap fogTilemap;
    public Tile fogTile;
    public Tile highlight;


    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        m_MainCamera = Camera.main;
        zoomer = m_MainCamera.GetComponent<Camera_Zoomer>();
        overlayTilemap = GameObject.Find("Overlays").GetComponent<Tilemap>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
        viewRange = 2;
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
        if (PieceManager.state == PieceManager.State.Day)
        {
            viewRange = 20;
            zoomer.ResetZoomCamera();
        }
        if (PieceManager.state == PieceManager.State.Dawn | PieceManager.state == PieceManager.State.Dusk)
        {
            viewRange = 3;
            zoomer.ZoomCamera(viewRange, transform.position);
        }
        if (PieceManager.state == PieceManager.State.Night)
        {
            viewRange = 2;
            zoomer.ZoomCamera(viewRange, transform.position);
        }
        Vector3Int[] viewHexes = HexCoordinates.GetFieldOfView(center, viewRange);
        foreach (Vector3Int coord in viewHexes)
        {
            fogTilemap.SetTile(HexCoordinates.CubeToOffset(coord), null);
           
        }


    }

}
