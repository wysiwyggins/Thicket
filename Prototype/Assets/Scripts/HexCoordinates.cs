using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //new attempts at offset to axial and cube and viceversa


    public static Vector2Int OffsetToAxial(Vector3Int offset)
    {

        int x = offset.x;
        int z = offset.y;
        return new Vector2Int(x - z / 2, z);
    }

    public static Vector3Int CubeToOffset(Vector3Int cube)
    {
        int col = cube.x + (cube.z - (cube.z & 1)) / 2;
        int row = cube.z;
        return new Vector3Int(col, row, 0);
    }

    public static Vector3Int OffsetToCube(Vector3Int offset)
    {
        int x = offset.x - (offset.y - (offset.y & 1)) / 2;
        int z = offset.y;
        int y = -x - z;
        if (x + y + z != 0)
        {
            Debug.Log("the sum of cube vectors must always be zero!");
        }
        return new Vector3Int(x, y, z);
    }

    public static Vector3Int[] GetNeighborsAtPos(Vector3Int aCoord)
    {
        List<Vector3Int> Neighbors = new List<Vector3Int>();

        Vector3Int CubeCoords = HexCoordinates.OffsetToCube(aCoord); 
        int x = CubeCoords.x;
        int y = CubeCoords.y;
        int z = CubeCoords.z;
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x - 1, y + 1, z)));
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x, y + 1, z - 1)));
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x + 1, y, z - 1)));
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x + 1, y - 1, z)));
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x, y - 1, z + 1)));
        Neighbors.Add(HexCoordinates.CubeToOffset(new Vector3Int(x - 1, y, z + 1)));
        
        return Neighbors.ToArray();
    }


}
