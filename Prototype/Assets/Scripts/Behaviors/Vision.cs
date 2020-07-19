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

    public int GetViewRange()
    {
        // the idea of this was to set the range based on time of day
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
        return viewRange;
    }


    public void UpdateFog(Vector3Int center)
    {
        
        //this would remove fog tiles from field of view
        int viewRange = GetViewRange();
        Debug.Log("Attempting field of view with a center of " + center + " and a range of " + viewRange);
        Vector3Int[] viewHexes = HexCoordinates.GetFieldOfView(center, viewRange);
        foreach (Vector3Int coord in viewHexes)
        {
            fogTilemap.SetTile(HexCoordinates.CubeToOffset(coord), null);
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
