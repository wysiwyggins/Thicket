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

}


//here's the redbloggames example functions. oddr means 'pointy-top' hexes.

function cube_to_oddr(cube):
{
    var col = cube.x + (cube.z - (cube.z & 1)) / 2;
    var row = cube.z;
    return OffsetCoord(col, row)
}
    

function oddr_to_cube(hex):
{
    var x = hex.col - (hex.row - (hex.row & 1)) / 2;
    var z = hex.row;
    var y = -x - z;
    return Cube(x, y, z);
}

// here's another example I found

public Vector3 getVector3Coord(Vector2 pos)
{

    int x = (int)pos.x, y = (int)pos.y;
    int tileX = x, tileZ = y - (x - (x & 1)) / 2, tileY = -tileX - tileZ;
    return new Vector3(tileX, tileY, tileZ);

}