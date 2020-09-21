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

    public float transitionSpeed;
    float visibility;
    private Tilemap overlayTilemap;
    //fog
    private Tilemap fogTilemap;
    public Tile fogTile;
    public Tile highlight;


    public AnimationCurve fogSizeOverTime;

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
        visibility = Mathf.PingPong(PieceManager.Instance.hour, 6) + 1;
        //Debug.Log("Attempting field of view with a center of " + center + " and a range of " + visibility);

        Vector3Int[] viewHexes = HexCoordinates.GetFieldOfView(center, (int) visibility);
        foreach (Vector3Int coord in viewHexes)
        {
            fogTilemap.SetTile(HexCoordinates.CubeToOffset(coord), null);
           
        }


    }

}
