using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Vision : MonoBehaviour
{

    int viewRange;
    Vector3Int[] Results;
    Grid grid;

    private Tilemap overlayTilemap;
    //fog
    private Tilemap fogTilemap;

    public Tile highlight;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        overlayTilemap = GameObject.Find("Overlays").GetComponent<Tilemap>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
    }

    public Vector3Int[] GetView()
    {

        //at some point i'd like viewRange to increase and decrease with sunPerc
        //Debug.Log(PieceManager.Instance.sunPerc);

        //if (PieceManager.state == PieceManager.State.Night)
        //{
        //    viewRange = 2;
        //} else if (PieceManager.state == PieceManager.State.Dawn | PieceManager.state == PieceManager.State.Dawn )
        //{
        //    viewRange = 4;
        //} else
        //{
        //    viewRange = 10;
        //}

        viewRange = 5;
        Vector3Int coords = grid.WorldToCell(transform.position);
        Vector3Int cubecoords = HexCoordinates.OffsetToCube(coords);

        //Debug.Log("vision origin cubecoords: " + cubecoords);
        Results = HexCoordinates.GetFieldOfView(cubecoords, viewRange);
        return Results;
    }

    public void TestView()
    {
        //this should highlight visible tiles to test field of view
        Vector3Int[] view = GetView();
        foreach (Vector3Int coord in view)
        {
            overlayTilemap.SetTile(coord, highlight);
            Debug.Log(coord + " is visible");
        }
    }

    public void UpdateFog()
    {
        //this would remove fog tiles from field of view
        Vector3Int[] view = GetView();
        foreach (Vector3Int coord in view)
        {
            fogTilemap.SetTile(coord, null);
        }

        


        ////old range
        //Vector3Int PiecePosition = fogTilemap.WorldToCell(transform.position);
        //Vector3Int CubeCoords = HexCoordinates.OffsetToCube(PiecePosition);

        //int x = CubeCoords.x;
        //int y = CubeCoords.y;
        //int z = CubeCoords.z;
        //// I love constantly fixing range by one
        //int adjustedRange = range - 1;

        //for (int i = -adjustedRange; i <= adjustedRange; i++)
        //{
        //	for (int j = -adjustedRange; j <= adjustedRange; j++)
        //	{
        //		for (int k = -adjustedRange; k <= adjustedRange; k++)
        //		{
        //			if (i + j + k == 0)
        //			{
        //				fogTilemap.SetTile(HexCoordinates.CubeToOffset(new Vector3Int(x + i, y + j, z + k)), null);
        //			}

        //		}

        //	}
        //}

        // old square bullshit
        //Vector3Int PiecePosition = fogTilemap.WorldToCell(transform.position);
        //      for(int i = -3; i <= 3; i++)
        //      {
        //          for(int j = -3; j <= 3; j++)
        //          {
        //		//SetTileColour(new Color(0,0,0), new Vector3Int(i, j, 0), fogTilemap); //i was experimenting with trying to change the fog color
        //	    fogTilemap.SetTile(PiecePosition + new Vector3Int(i, j, 0), null);
        //          }
        //      }
    }

}
