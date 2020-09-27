using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexMazeGenerator : MonoBehaviour
{
    public Vector2Int mazeDimensions = new Vector2Int(16, 16);
    public int cellDistance = 3;
    public bool markNodeTiles = false;

    Tilemap _tilemap;

    public TileBase _pathTile;
    public TileBase _nodeTile;

    class MazeNode
    {
        public Vector2Int coord;
        public List<MazeNode> connections = new List<MazeNode>();
        public MazeNode(Vector2Int loc)
        {
            coord = loc;
        }
    }


    void Start()
    {
        _tilemap = this.GetComponentInChildren<Tilemap>();

        //All the nodes, and their connection that made up the maze, 
        //Is dictionary so can look up by grid coordinate.
        Dictionary<Vector2Int, MazeNode> mazeNodes = new Dictionary<Vector2Int, MazeNode>();

        
        
        //--- Center the camera on the maze
        Vector2 mazeCenter = mazeDimensions * cellDistance / 2;
        Camera.main.transform.position += new Vector3(mazeCenter.x, mazeCenter.y, 0);


        //--- Pick a random starting cell on the maze, and add it to the maze
        Vector2Int firstMazeCoord = new Vector2Int(Random.Range(0, mazeDimensions.x), Random.Range(0, mazeDimensions.y));
        var startingMazeNode = new MazeNode(firstMazeCoord);
        mazeNodes.Add(firstMazeCoord, startingMazeNode);

        //-- Get the frontier (cells not yet in the maze, but adjcent to existing maze cells)
        List<Vector2Int> frontier = new List<Vector2Int>();
        frontier.AddRange(getNeighbors(firstMazeCoord));

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
        fillInTraversableTilesRecursive(startingMazeNode);
    }


    void fillInTraversableTilesRecursive(MazeNode node)
    {
        Vector3Int nodeCoordV3 = new Vector3Int(node.coord.x * cellDistance, node.coord.y * cellDistance, 0);
        
        //Draw the tile at the node...
        _tilemap.SetTile(nodeCoordV3, markNodeTiles ? _nodeTile : _pathTile);

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
                    intermediate.x = (int) Mathf.MoveTowards(intermediate.x, connectedNodeCoordV3.x, 1);
                    incX = false;
                }
                else //if (intermediate.y != connectV3.x)
                {
                    intermediate.y = (int)Mathf.MoveTowards(intermediate.y, connectedNodeCoordV3.y, 1);
                    incX = true;
                }

                if (intermediate != connectedNodeCoordV3)
                {
                    _tilemap.SetTile(intermediate, _pathTile);
                }
            }

            //repeat again for the connected node
            fillInTraversableTilesRecursive(connectedNode);
        }

    }


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

    //List<Vector2Int> getOppositeNeighborPairs(Vector2Int v)
    //{
    //    int x = v.x;
    //    int y = v.y;
    //    List<Vector2Int> neighbors = new List<Vector2Int>();

    //    bool atXmin = x == 0;
    //    bool atXmax = x == _gridDim.x - 1;

    //    bool atYmin = y == 0;
    //    bool atYmax = y == _gridDim.y - 1;

    //    if (!atXmin && !atXmax) // E - W
    //    {
    //        neighbors.Add(new Vector2Int(x - 1, y));
    //        neighbors.Add(new Vector2Int(x + 1, y));
    //    }


    //    if (!atYmin && !atYmax)
    //    {
    //        if (y % 2 == 0 && !atXmin)
    //        {
    //            neighbors.Add(new Vector2Int(x, y - 1));
    //            neighbors.Add(new Vector2Int(x - 1, y + 1));

    //            neighbors.Add(new Vector2Int(x - 1, y - 1));
    //            neighbors.Add(new Vector2Int(x, y + 1));
    //        }

    //        if (y % 2 == 1 && !atXmax)
    //        {
    //            neighbors.Add(new Vector2Int(x, y - 1));
    //            neighbors.Add(new Vector2Int(x + 1, y + 1));

    //            neighbors.Add(new Vector2Int(x + 1, y - 1));
    //            neighbors.Add(new Vector2Int(x, y + 1));
    //        }
    //    }






    //    return neighbors;
    //}


}
