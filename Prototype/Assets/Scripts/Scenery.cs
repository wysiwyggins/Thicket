using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scenery : MonoBehaviour
{
    Grid grid;
    SimplePathFinding2D pf;
    Tilemap navigationTilemap;
    Tile blockTile;
    
    public string SceneryName = "unnamed";
    public string Description = "There's nothing here.";
    public bool Cleansing;
    public bool Obstacle;
    Vector3Int coordinates;
    // Start is called before the first frame update
    void Start()
    {
        pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        navigationTilemap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();
        blockTile = pf.BlockTile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SceneryManager.AllScenery.Add(this);
        //coordinates = grid.WorldToCell(transform.position);
        //if (Obstacle)
        //{
        //    pf.SetNavTileBlocked(coordinates, true);
        //}
    }

    private void OnDestroy()
    {
        //coordinates = grid.WorldToCell(transform.position);
        //if (Obstacle)
        //{
        //    pf.SetNavTileBlocked(coordinates, false);
        //}
        SceneryManager.AllScenery.Remove(this);
    }

}
