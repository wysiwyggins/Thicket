---
title: Offset to cube coordinate conversions
date: 2020-06-13
layout: post
tags: thicket, daily
image: hexcube.png
exclude: true
hidden: true
author: Wiley Wiggins
---

I was able to make my own functions to convert from [offset coordinates](https://www.redblobgames.com/grids/hexagons/#coordinates-offset) (what unity tilemap and SimplePathFinding2d use) to [cube coordinates](https://www.redblobgames.com/grids/hexagons/#coordinates-cube) (what I presumably need to be able to get hex ranges and line of sight). I replaced the square range finding for highlighting moves (which was complicated and distorted every other column you moved) for this hex one- this is currently just moving out in each direction in a star shape, but with a little more puzzling I should be able to get the whole range (I hope). 

For anyone else who needs to convert from default Unity Tilemap offset coordinates to Cube, here's my functions. Refer to the redblobs site if you need to do flat-topped hexagons,

{% highlight c# %}
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
{% endhighlight %}

*update* and now full hex ranges work. Here's how I am getting the tile range to visualize where you can move - 

{% highlight c# %}
    Vector3Int PiecePosition = overlayTilemap.WorldToCell(transform.position);

    Vector3Int CubeCoords = HexCoordinates.OffsetToCube(PiecePosition); 
    
    int x = CubeCoords.x;
    int y = CubeCoords.y;
    int z = CubeCoords.z;
    // I love constantly fixing range by one
    int adjustedRange = range - 1;
    for (int i = -adjustedRange; i <= adjustedRange; i++)
    {
        for (int j = -adjustedRange; j<= adjustedRange; j++)
        {
            for (int k = -adjustedRange; k <= adjustedRange; k++)
            {
                if ( i + j + k == 0 )
                {
                    overlayTilemap.SetTile(HexCoordinates.CubeToOffset(new Vector3Int(x + i, y + j, z + k)), highlight);
                }
                
            }

        }
    }
{% endhighlight %}