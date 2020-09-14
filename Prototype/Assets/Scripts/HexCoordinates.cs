using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexCoordinates : MonoBehaviour
{

    static public List<Vector3Int> directions = new List<Vector3Int> { new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1) };

    
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

    static public Vector3Int Direction(int direction)
    {
        return directions[direction];
    }


    public static Vector3Int[] GetHexesAtDistance(Vector3Int center, int distance)
    {

        List<Vector3Int> Results = new List<Vector3Int>();

        int x = center.x;
        int y = center.y;
        int z = center.z;
        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                for (int k = -distance; k <= distance; k++)
                {
                    if (i + j + k == 0)
                    {
                        Results.Add(new Vector3Int(x + i, y + j, z + k));
                    }   
                }
            }
        }
        return Results.ToArray();
    }



    //public static Vector3Int[] CubeRing(Vector3Int center, int distance)
    //{
    //    List<Vector3Int> Results = new List<Vector3Int>();

    //    Vector3Int cube = center * distance;

    //    for (int i = 0; i <= 6; i++)
    //    {
    //        for (int j = 0; j <= distance; j++)
    //        {
    //            Results.Add(cube);
    //            cube = cube + (cube + directions[i]);//cube neighbor(cube, i);
    //        }
    //    }
    //    return Results.ToArray();
    //}

    public static int CubeDistance(Vector3Int a, Vector3Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
    }


    public static float Lerp(float a, float b, float t)
    {  
        return a + (b - a) * t;
    }

    public static Vector3Int CubeRound(Vector3 cube)
    {
        float rx = Mathf.Round(cube.x);
        float ry = Mathf.Round(cube.y);
        float rz = Mathf.Round(cube.z);

        float x_diff = Mathf.Abs(rx - cube.x);
        float y_diff = Mathf.Abs(ry - cube.y);
        float z_diff = Mathf.Abs(rz - cube.z);

        if ((x_diff > y_diff) && (x_diff > z_diff)) {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        return new Vector3Int((int)rx, (int)ry, (int)rz);
    }

    public static Vector3 CubeLerp(Vector3Int ai, Vector3Int bi, float t)
    {
        Vector3 a = new Vector3((float)ai.x + 0.0001f, (float)ai.y + 0.0001f, (float)ai.z + 0.0001f);
        Vector3 b = new Vector3((float)bi.x, (float)bi.y, (float)bi.z);

        return new Vector3(Lerp(a.x, b.x, t),
                    Lerp(a.y, b.y, t),
                    Lerp(a.z, b.z, t));
    }

    public static Vector3Int[] CubeLineDraw(Vector3Int origin, Vector3Int extent)
    {
        int N = CubeDistance(origin, extent);
        List<Vector3Int> Results = new List<Vector3Int>();
        for (int i = 0; i <= N; i++) {
            Results.Add(CubeRound(CubeLerp(origin, extent, (float)(1.0 / (float)N * (float)i))));
        }
        return Results.ToArray();
    }

    public static Vector3Int[] GetFieldOfView(Vector3Int origin, int range)
    {

        Debug.Log("origin" + origin + ", range: " + range);

        List<Vector3Int> Results = new List<Vector3Int>();
        Vector3Int[] targets = GetHexesAtDistance(origin, range);
        foreach (Vector3Int target in targets)
        {
            bool lineblocked = false;
            Vector3Int[] lineHexes = CubeLineDraw(origin, target);
            
            foreach(Vector3Int hexcoord in lineHexes)
            {
                
                Vector3Int coordinate = CubeToOffset(hexcoord);
                Scenery[] sceneries = SceneryManager.GetSceneriesAtPos(coordinate);
                if (sceneries != null)
                {   
                    foreach(Scenery scenery in sceneries)
                    {
                        if (scenery.Opaque)
                        {
                            lineblocked = true;
                        }
                    }
                }
            }
            if (lineblocked == false) 
            {
                Results.Add(target);
            }  
        }
        return Results.ToArray();
    }
}
