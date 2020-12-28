using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimplePF2D;

public class MapBuilder : MonoBehaviour
{
    // map elements
    public int map_radius = 7;
    public int trees_amount;
    public int flowers_amount;
    public int predator_amount;  //how many wolves
    public int apex_amount;  // how many bears
    public int chickens_amount;
    public int water_amount;
    Vector3Int map_origin;

    Grid grid;

    //tiles, and obstacles (obstacles are scenery, a class that contains bools for opacity and movement blocking)

    public Tilemap floorTiles;
    public Tilemap navigationTiles;
    public Tile groundTile;
    public Tile navTile;
    public Scenery water;
    public List<Scenery> obstacleTiles;
    public Piece predator;
    public Piece apex;
    public Piece flower;


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


    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();

        // maze dimentions encompass our ground tiles which are drawn in a hexagon
        mazeDimensions = new Vector2Int(map_radius * 2, map_radius * 2);

        //All the nodes, and their connection that made up the maze, 
        //Is dictionary so can look up by grid coordinate.
        Dictionary<Vector2Int, MazeNode> mazeNodes = new Dictionary<Vector2Int, MazeNode>();


        //wipe out the old world to make room for the new world.
        map_origin = new Vector3Int(0, 0, 0);

        //create the map, this should probably be a coroutine?
        HexMap();

        //--- Pick a random starting cell on the maze, and add it to the maze
        Vector2Int firstMazeCoord = new Vector2Int(Random.Range(0, mazeDimensions.x), Random.Range(0, mazeDimensions.y));
        var startingMazeNode = new MazeNode(firstMazeCoord);
        mazeNodes.Add(firstMazeCoord, startingMazeNode);

        //-- Get the frontier (cells not yet in the maze, but adjcent to existing maze cells) //changing this from vec2's to vec3's 
        List<Vector2Int> frontier = new List<Vector2Int>();
        frontier.AddRange(getNeighbors(firstMazeCoord));
        //started to try to use the existing cube coord GetNeighbors, but would have still needed to convert to Vec2Ints
        //frontier.AddRange(HexCoordinates.GetNeighborsAtPos(HexCoordinates.OffsetToCube(new Vector3Int(firstMazeCoord.y, firstMazeCoord.y, 0);

        //-- Keep track of every cell we've considered in the process
        List<Vector2Int> visited = new List<Vector2Int>();
        visited.Add(firstMazeCoord);

        int sanity = 9999; //prevent an infinite loop, probably not necessary anymore

        while (frontier.Count > 0 && sanity > 0)
        {
            sanity--;

            //Pick a random frontier cell, and mark visitied
            Vector2Int randomFrontierCoord = frontier[Random.Range(0, frontier.Count)];
            visited.Add(randomFrontierCoord);

            //Find existing maze node that leads to this frontier cell...
            List<Vector2Int> neighbors = getNeighbors(randomFrontierCoord);
            int randOffset = Random.Range(0, neighbors.Count);
            for (int i = 0; i < neighbors.Count; i++)
            {
                Vector2Int neighb = neighbors[(i + randOffset) % neighbors.Count];
                if (mazeNodes.ContainsKey(neighb)) //Found it!
                {
                    //Remove the cell from frontier...
                    frontier.Remove(randomFrontierCoord);

                    //Add it to the maze, connected the previous maze node
                    MazeNode newNode = new MazeNode(randomFrontierCoord);
                    mazeNodes[randomFrontierCoord] = newNode;
                    mazeNodes[neighb].connections.Add(newNode);

                    //Add the new maze node's adjacents cells to the frontier
                    foreach (var neighborCoord in neighbors)
                    {
                        if (!mazeNodes.ContainsKey(neighborCoord) && !frontier.Contains(neighborCoord) && !visited.Contains(neighborCoord))
                        {
                            frontier.Add(neighborCoord);
                        }
                    }

                    //only connect in 1 place
                    break;
                }
            }
        }

        //starting from the first mode, recursively fill in maze paths
        //fillInTraversableTilesRecursive(startingMazeNode);
        drawMaze();
    }

    void drawMaze()
    {
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_radius);
        bool[,] mazeMap = buildMaze();
        for(int x = 0-map_radius; x <= map_radius; x++)
        {
            for (int y = 0 - map_radius; y <= map_radius; y++)
            {
                for (int z = 0 - map_radius; z <= map_radius; z++)
                {
                    if ((x + y + z == 0) && (getMapBool(x,y,mazeMap)))
                    {
                        addScenaryAtLocation(new Vector3Int(x, y, z));
                    }
                }
            }
        }
    }

    Vector3Int randomNodeTile()
    {
        Vector3Int tile;
        do
        {
            tile = new Vector3Int(Random.Range(0 - map_radius, map_radius + 1),
            Random.Range(0 - map_radius, map_radius + 1),
            Random.Range(0 - map_radius, map_radius + 1));
        } while (!isNodeTile(tile) || (tile.x + tile.y + tile.z) != 0);

        return tile;
    }

    // We add map_radius when doing modulo operations as a cheap way to make sure we don't get a negative result.
    bool isNodeTile(Vector3Int coord)
    {
        if (!isInsideMap(coord))
        {
            return false;
        }

        return (((coord.z + map_radius) % 2 == 0) && ((coord.x + map_radius) % 3 == (coord.z + map_radius) % 3)) ||
        (((coord.z + map_radius) % 2 == 1) && ((coord.x + map_radius) % 3 == (coord.z + map_radius) % 3));
    }

    bool isInsideMap(Vector3Int coord)
    {
        return !(HexCoordinates.CubeDistance(new Vector3Int(0, 0, 0), coord) > map_radius);
    }

    bool[,] buildMaze()
    {
        int totalNodes = 0;
        int completedNodes = 0;
        bool[,] mazeMap = new bool[(map_radius*2)+1 , (map_radius*2)+1];
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_radius);
        foreach (Vector3Int coord in groundCoords)
        {
            if (isNodeTile(coord))
            {
                totalNodes++;
            }
            setMapBool(coord.x, coord.y, true, mazeMap);
        }
        List<Vector3Int> frontier = new List<Vector3Int>();
        openNode(randomNodeTile(), frontier, mazeMap);
        completedNodes++;
        while (completedNodes < totalNodes)
        {
            Vector3Int nextNode = frontier[Random.Range(0, frontier.Count)];
            frontier.Remove(nextNode);
            makePathToNode(nextNode, frontier, mazeMap);
            completedNodes++;
        }
        carveOutCutThroughs(mazeMap);
        addClearings(mazeMap);
        return mazeMap;
    }

    void addClearings(bool [,] mazeMap)
    {
        int[] clearingChances = new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4 };
        int numberOfClearings = clearingChances[Random.Range(0, 14)];
        Debug.Log("Number of clearings is " + numberOfClearings);
        for (int count = 0; count < numberOfClearings; count ++)
        {
            int radius = Random.Range(1, 3);
            if (Random.Range(0,6) == 0)
            {
                radius = 3;
            }

            Vector3Int clearingCenter = randomNodeTile();
            Vector3Int[] clearingCoords = HexCoordinates.GetHexesAtDistance(clearingCenter, radius);
            Debug.Log("Making size " + radius + " clearing (" + clearingCoords.Length + " hexes) at " +
                clearingCenter.x + ", " + clearingCenter.y + ", " + clearingCenter.z);
            foreach(Vector3Int coord in clearingCoords)
            {
                if (isInsideMap(coord))
                {
                    setMapBool(coord.x, coord.y, false, mazeMap);
                }
            }

        }
    }

    void carveOutCutThroughs(bool[,] mazeMap)
    {
        int percentChance = Random.Range(10, 36);
        Debug.Log("Cutthrough chance is " + percentChance + "%");
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_radius);
        foreach (Vector3Int coord in groundCoords)
        {
            if (getMapBool(coord.x, coord.y, mazeMap) && (Random.Range(1,101) <= percentChance))
            {
                setMapBool(coord.x, coord.y, false, mazeMap);
            }
        }
    }

    void makePathToNode(Vector3Int coord, List<Vector3Int> frontier, bool[,] mazeMap)
    {
        Vector3Int pathNode;
        do
        {
          pathNode = coord + HexCoordinates.directions[Random.Range(0, 5)];
        } while (!hasOpenNodeBorder(pathNode, mazeMap));

        setMapBool(pathNode.x, pathNode.y, false, mazeMap);
        openNode(coord, frontier, mazeMap);
    }

    bool hasOpenNodeBorder(Vector3Int coord, bool[,] mazeMap)
    {
        if (!isInsideMap(coord))
        {
            return false;
        }
        foreach (Vector3Int direction in HexCoordinates.directions)
        {
            if (isOpenNode(coord + direction, mazeMap))
            {
                return true;
            }
        }
        return false;
    }

    bool isOpenNode(Vector3Int coord, bool[,] mazeMap)
    {
        if (!isNodeTile(coord))
        {
            return false;
        }
        return (!getMapBool(coord.x, coord.y, mazeMap));
    }

    void openNode(Vector3Int coord, List<Vector3Int> frontier, bool[,] mazeMap)
    {
        setMapBool(coord.x, coord.y, false, mazeMap);
        foreach(Vector3Int nodeDirection in HexCoordinates.nodeDirections)
        {
            Vector3Int potentialNode = coord + nodeDirection;
            if (isNodeTile(potentialNode) && getMapBool(potentialNode.x, potentialNode.y, mazeMap))
            {
                if (!frontier.Contains(potentialNode))
                {
                    frontier.Add(potentialNode);
                }
            }
        }
    }

    bool getMapBool(int x, int y, bool[,] mazeMap)
    {
        return mazeMap[x + map_radius, y + map_radius];
    }

    void setMapBool(int x, int y, bool value, bool[,] mazeMap)
    {
        mazeMap[x + map_radius, y + map_radius] = value;
    }

    void addScenaryAtLocation(Vector3Int coord)
    {
        Vector3 coord2 = grid.CellToWorld(HexCoordinates.CubeToOffset(coord));
        Instantiate(obstacleTiles[Random.Range(0, obstacleTiles.Count)], coord2, Quaternion.identity);
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


    //alex's Vector2Int neighbor function
    List<Vector2Int> getNeighbors(Vector2Int v)
    {
        int x = v.x;
        int y = v.y;
        List<Vector2Int> neighbors = new List<Vector2Int>();

        bool atXmin = x == 0;
        bool atXmax = x == mazeDimensions.x - 1;

        bool atYmin = y == 0;
        bool atYmax = y == mazeDimensions.y - 1;

        if (!atYmin)
        {
            neighbors.Add(new Vector2Int(x, y - 1));
        }

        if (!atYmax)
        {
            neighbors.Add(new Vector2Int(x, y + 1));
        }

        if (!atXmin)
        {
            neighbors.Add(new Vector2Int(x - 1, y));

            if (y % 2 == 0)
            {
                if (!atYmin)
                {
                    neighbors.Add(new Vector2Int(x - 1, y - 1));
                }

                if (!atYmax)
                {
                    neighbors.Add(new Vector2Int(x - 1, y + 1));
                }
            }
        }

        if (!atXmax)
        {
            neighbors.Add(new Vector2Int(x + 1, y));


            if (y % 2 == 1)
            {
                if (!atYmin)
                {
                    neighbors.Add(new Vector2Int(x + 1, y - 1));
                }

                if (!atYmax)
                {
                    neighbors.Add(new Vector2Int(x + 1, y + 1));
                }
            }
        }

        return neighbors;
    }
    void fillInTraversableTilesRecursive(MazeNode node)
    {
        Vector3Int nodeCoordV3 = new Vector3Int(node.coord.x * cellDistance, node.coord.y * cellDistance, 0);

        //Draw the tile at the node...
        navigationTiles.SetTile(nodeCoordV3, markNodeTiles ? navTile : groundTile);

        //lets drop a piece of scenery instead of a tile
       //Instantiate(water, nodeCoordV3, Quaternion.identity);

        foreach (var connectedNode in node.connections)
        {
            Vector3Int connectedNodeCoordV3 = new Vector3Int(connectedNode.coord.x * cellDistance, connectedNode.coord.y * cellDistance, 0);

            bool incX = connectedNode.coord.x % 2 == 0;

            //Draw the tiles from this node, up to each connected node, alternating x/y
            Vector3Int intermediate = nodeCoordV3;
            while (intermediate != connectedNodeCoordV3)
            {
                incX |= intermediate.y == connectedNodeCoordV3.y;

                if (incX && intermediate.x != connectedNodeCoordV3.x)
                {
                    intermediate.x = (int)Mathf.MoveTowards(intermediate.x, connectedNodeCoordV3.x, 1);
                    incX = false;
                }
                else //if (intermediate.y != connectV3.x)
                {
                    intermediate.y = (int)Mathf.MoveTowards(intermediate.y, connectedNodeCoordV3.y, 1);
                    incX = true;
                }

                if (intermediate != connectedNodeCoordV3)
                {
                    // navigationTiles.SetTile(intermediate, navTile);
                    Instantiate(obstacleTiles[Random.Range(0, obstacleTiles.Count)], intermediate, Quaternion.identity);
                }
            }

            //repeat again for the connected node
            fillInTraversableTilesRecursive(connectedNode);
        }

    }

}





