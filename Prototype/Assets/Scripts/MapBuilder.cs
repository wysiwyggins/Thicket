using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapBuilder : MonoBehaviour
{

    public int map_size;
    public int brush;
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


    // Mapbuilder would
    // have variables for the size of the whole map. (maybe the map itself is hexagonal)
    // lays down floor tiles based on size var
    // randomly creates a maze of scenery and obstacle tiles
    // I'd like to use this- http://gamelogic.co.za/grids/features/examples-that-ship-with-grids/prims-maze-generation-algorithm-on-a-hexagonal-grid/
    // right now the obstacle bool of scenery doesn't actually do anything, and obstacle tiles are painted manually,
    // but something like this might work- pf.SetNavTileBlocked(grid.WorldToCell(transform.position), true);
    // randomly place water and animals and plants based on public variables for each


    // Start is called before the first frame update
    void Start()
    {
        //wipe out the old world to make room for the new world.
        map_origin = new Vector3Int(0,0,0);
        HexMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HexMap()
    {
        //this builds a hexagon-shaped map. There's redblobs implementations for other shapes, like triangles: https://www.redblobgames.com/grids/hexagons/implementation.html#hex-distance
        Vector3Int[] groundCoords = HexCoordinates.GetHexesAtDistance(map_origin, map_size);

        // This would make a ring of obstacle tiles around the map, but it's currently broken. Uses a neighbors array and CubeRing function that I probably did wrong.
        //Vector3Int[] boundaryCoords = HexCoordinates.CubeRing(map_origin, map_size + 1);
        foreach (Vector3Int coord in groundCoords)
        {
            floorTiles.SetTile(HexCoordinates.CubeToOffset(coord), groundTile);

        }
        //foreach (Vector3Int coord in boundaryCoords)
        //{
        //    navigationTiles.SetTile(HexCoordinates.CubeToOffset(coord), navTile);

        //}

    }







    // from pims maze generator page

    // core maze algorithm
    //public static IEnumerable GenerateMazeWalls(PointyHexGrid grid)
    //{
    //    IGrid < bool, pointyhexpoint = "" > walls = grid.CloneStructure(); //true indicates passable

    //    foreach (var point in walls)
    //    {
    //        walls[point] = point.GetColor2_4() == 0;
    //    }

    //    List wallList = new List();

    //    var newMaizePoint = grid.Where(p => p.GetColor2_4() == 0).RandomItem();
    //    var inMaze = new List();
    //    inMaze.Add(newMaizePoint);

    //    var edges = newMaizePoint.GetNeighbors();
    //    wallList.AddRange(edges);

    //    while (wallList.Any())
    //    {
    //        var randomWall = wallList.RandomItem();
    //        IEnumerable faces = GetEdgeFaces(randomWall).Where(p => grid.Contains(p));

    //        //At least one of the two faces must be in the maze
    //        if (faces.Any(point => !inMaze.Contains(point)))
    //        {

    //            newMaizePoint = faces.First(point => !inMaze.Contains(point));
    //            inMaze.Add(newMaizePoint);
    //            walls[randomWall] = true;

    //            yield return randomWall;

    //            // Add all edges that are not passages
    //            edges = newMaizePoint.GetNeighbors().Where(edge => !(walls[edge]));
    //            wallList.AddRange(edges);
    //        }
    //        else
    //        {
    //            wallList.Remove(randomWall);
    //        }
    //    }

    //    yield break;
    //}

    ////helper
    //public static IEnumerable GetEdgeFaces(PointyHexPoint point)
    //{
    //    int color = point.GetColor2_4();

    //    List faces = new List();

    //    switch (color)
    //    {
    //        case 0:
    //            //error!
    //            break;
    //        case 1:
    //            faces.Add(point + PointyHexPoint.East);
    //            faces.Add(point + PointyHexPoint.West);
    //            break;
    //        case 2:
    //            faces.Add(point + PointyHexPoint.SouthWest);
    //            faces.Add(point + PointyHexPoint.NorthEast);
    //            break;
    //        case 3:
    //            faces.Add(point + PointyHexPoint.SouthEast);
    //            faces.Add(point + PointyHexPoint.NorthWest);
    //            break;
    //    }

    //    return faces;
    //}

    //// shell
    //public class PrimsAlgorithmHex : GLMonoBehaviour, IResetable
    //{
    //    public Cell cellPrefab;
    //    public GameObject root;

    //    private readonly Vector2 cellDimensions = new Vector2(69, 80);

    //    public void Start()
    //    {
    //        Reset();
    //    }

    //    public void Reset()
    //    {
    //        root.transform.DestroyChildren();
    //        StartCoroutine(BuildGrid());
    //    }

    //    public IEnumerator BuildGrid()
    //    {
    //        var grid = PointyHexGrid.Hexagon(6);

    //        var map = new PointyHexMap(cellDimensions)
    //           .WithWindow(ExampleUtils.ScreenRect)
    //           .AlignMiddelCenter(grid)
    //           .Scale(.97f) //Makes cells overlap slightly; prevents border artefacts
    //           .To3DXY();

    //        foreach (var point in grid)
    //        {
    //            Cell cell = Instantiate(cellPrefab);
    //            cell.transform.parent = root.transform;
    //            cell.transform.localScale = Vector3.one;
    //            cell.transform.localPosition = map[point];
    //            cell.SetText("");
    //            int color = point.GetColor2_4();
    //            cell.SetFrame(color);
    //            grid[point] = cell;
    //        }

    //        foreach (var point in MazeAlgorithms.GenerateMazeWalls(grid))
    //        {
    //            grid[point].SetFrame(0);
    //            yield return null;
    //        }

    //        yield return null;
    //    }
    //}


}
