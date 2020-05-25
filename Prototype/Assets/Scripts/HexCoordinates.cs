using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoordinates
{
    //https://catlikecoding.com/unity/tutorials/hex-map/part-1/

    public int X { get; private set; }

    public int Z { get; private set; }

    public HexCoordinates(int x, int z)
    {
        X = x;
        Z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x, z);
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X.ToString() + "\n" + Z.ToString();
    }


    //here's the redbloggames example functions, which I tried to convert from C++. oddr means 'pointy-top' hexes.

    //https://www.redblobgames.com/grids/hexagons/#coordinates

    //public static Vector3Int cube_to_oddr(Vector3Int cube)
    //{
    //    int col = cube.x + (cube.z - (cube.z & 1)) / 2;
    //    int row = cube.z;
    //    return new Vector3Int(col, row, 0);
    //}


    //public static Vector3Int oddr_to_cube(Vector3Int hex)
    //{
    //    int x = hex.col - (hex.row - (hex.row & 1)) / 2;
    //    int z = hex.row;
    //    int y = -x - z;
    //    return new Vector3Int( x, y, z);
    //}

}


