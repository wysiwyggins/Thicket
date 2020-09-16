using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scenery : MonoBehaviour
{
    Grid grid;
    SimplePathFinding2D pf;
    public Tilemap navigationTilemap;
    public Tile navTile;
    Vector3Int pieceCoords
    { get { return grid.WorldToCell(transform.position); } }

    public string SceneryName = "unnamed";
    public string Description = "There's nothing here.";
    public bool Cleansing;
    public bool Obstacle;
    public bool Opaque;

    // Start is called before the first frame update
    void Begin()
    {
        pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        //navigationTilemap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {

        if (Obstacle)
        {
            navigationTilemap.SetTile(grid.WorldToCell(transform.position), navTile);
            pf.SetNavTileBlocked(grid.WorldToCell(transform.position), true);
        }
        SceneryManager.AllScenery.Add(this);
        
    }

    private void OnDestroy()
    {
        if (Obstacle)
        {
            navigationTilemap.SetTile(grid.WorldToCell(transform.position), null);
            pf.SetNavTileBlocked(grid.WorldToCell(transform.position), false);
        }
        SceneryManager.AllScenery.Remove(this);
    }

}