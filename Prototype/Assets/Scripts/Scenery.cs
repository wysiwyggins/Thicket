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
    Vector3Int pieceCoords
    { get { return grid.WorldToCell(transform.position); } }

    public string SceneryName = "unnamed";
    public string Description = "There's nothing here.";
    public bool Cleansing;
    public bool Obstacle;

    // Start is called before the first frame update
    void Begin()
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

        Debug.Log(pieceCoords);
        Debug.Log("yo");
        if (Obstacle)
        {
            pf.SetNavTileBlocked(grid.WorldToCell(transform.position), true);
        }
        SceneryManager.AllScenery.Add(this);
        
    }

    private void OnDestroy()
    {
        if (Obstacle)
        {
            pf.SetNavTileBlocked(grid.WorldToCell(transform.position), false);
        }
        SceneryManager.AllScenery.Remove(this);
    }

}
