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
        if (Obstacle)
        {
            navigationTilemap.SetTile(grid.WorldToCell(transform.position), blockTile);
        }
    }

    private void OnDestroy()
    {
        
        if (Obstacle)
        {
           //how do we destroy the blockTile at the same hex on navigationTilemap?
        }
        SceneryManager.AllScenery.Remove(this);
    }

}
