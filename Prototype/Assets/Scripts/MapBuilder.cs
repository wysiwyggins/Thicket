using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimplePF2D;

public class MapBuilder : MonoBehaviour
{

    public int map_radius;
    public int trees;
    public int flowers;
    public int wolves;
    public int bears;
    public int chickens;
    public int water;
    Vector3Int map_origin;

    Grid grid;

    public Tilemap floorTiles;
    public Tilemap navigationTiles;
    public Tile groundTile;
    public Tile navTile;
    public List<Scenery> obstacleTiles;


    // Mapbuilder would
    // have a mapsize variable and maybe a map shape variable
    // lays down floor tiles based on size var
    // randomly creates a maze of scenery and obstacle tiles
    // I'd like to use this- http://gamelogic.co.za/grids/features/examples-that-ship-with-grids/prims-maze-generation-algorithm-on-a-hexagonal-grid/
    // see also- https://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm
    // randomly place water and animals and plants based on public variables for each


    // Start is called before the first frame update
    void Start()
    {
        //wipe out the old world to make room for the new world.
        map_origin = new Vector3Int(0, 0, 0);
        HexMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HexMap()
    {
        //this builds a hexagon-shaped map. There's redblobs implementations for other shapes, like triangles: https://www.redblobgames.com/grids/hexagons/implementation.html#hex-distance
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_radius);

        Vector3Int[] boundaryCoords = HexCoordinates.CubeRing(map_origin, map_radius + 1);
        foreach (Vector3Int coord in groundCoords)
        {
            floorTiles.SetTile(HexCoordinates.CubeToOffset(coord), groundTile);

        }
        foreach (Vector3Int coord in boundaryCoords)
        {
            navigationTiles.SetTile(HexCoordinates.CubeToOffset(coord), navTile);

        }

    }


}




