using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimplePF2D;

public class MapBuilder : MonoBehaviour
{
    // map elements
    public int map_radius = 7;
    public int trees;
    public int flowers;
    public int wolves;
    public int bears;
    public int chickens;
    public int water;
    Vector3Int map_origin;

    Grid grid;

    //tiles, and obstacles (obstacles are scenery, a class that contains bools for opacity and movement blocking)

    public Tilemap floorTiles;
    public Tilemap navigationTiles;
    public Tile groundTile;
    public Tile navTile;
    public List<Scenery> obstacleTiles;

    //Variables from Alex's maze generator

    //public Vector2Int mazeDimensions = new Vector2Int(16, 16);
    // ^ lets change this to something coupled with the radius of the ground tiles we are already drawing.
    public Vector2Int mazeDimensions;

    public int cellDistance = 3;
    public bool markNodeTiles = false;

    // we don't need the node and path tiles in our version
    //public TileBase _pathTile;
    //public TileBase _nodeTile;

    //alex's mazenode class
    class MazeNode
    {
        public Vector2Int coord;
        public List<MazeNode> connections = new List<MazeNode>();
        public MazeNode(Vector2Int loc)
        {
            coord = loc;
        }
    }


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
        // maze dimentions encompass our ground tiles which are drawn in a hexagon
        mazeDimensions = new Vector2Int(map_radius * 2, map_radius * 2);

        //All the nodes, and their connection that made up the maze, 
        //Is dictionary so can look up by grid coordinate.
        Dictionary<Vector2Int, MazeNode> mazeNodes = new Dictionary<Vector2Int, MazeNode>();


        //wipe out the old world to make room for the new world.
        map_origin = new Vector3Int(0, 0, 0);

        //create the map, this should probably be a coroutine?
        HexMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void HexMap()
    {
        //this builds a hexagon-shaped map. There's redblobs implementations for other shapes, like triangles: https://www.redblobgames.com/grids/hexagons/implementation.html#hex-distance


        //lay the ground
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_radius);
        foreach (Vector3Int coord in groundCoords)
        {
            floorTiles.SetTile(HexCoordinates.CubeToOffset(coord), groundTile);

        }


        //put a boundary around the ground (ring is a little big right now?)
        Vector3Int[] boundaryCoords = HexCoordinates.CubeRing(map_origin, map_radius + 1);
        foreach (Vector3Int coord in boundaryCoords)
        {
            navigationTiles.SetTile(HexCoordinates.CubeToOffset(coord), navTile);

        }

    }


}




