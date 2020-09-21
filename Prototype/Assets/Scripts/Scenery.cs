using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scenery : MonoBehaviour
{
    Grid grid;
    public Tilemap navigationTilemap;
    public Tile navTile;

    public string SceneryName = "unnamed";
    public string Description = "There's nothing here.";
    public bool Cleansing;
    public bool Obstacle;
    public bool Opaque;
    Vector3Int pieceCoords
    { get { return grid.WorldToCell(transform.position); } }

    // Start is called before the first frame update
    void Begin()
    {
    }

    void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        navigationTilemap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        if (Obstacle)
        {
           navigationTilemap.SetTile(pieceCoords, navTile);
        }
        SceneryManager.AllScenery.Add(this);
        
    }

    private void OnDestroy()
    {
        if (Obstacle)
        {
           navigationTilemap.SetTile(pieceCoords, null);
        }
        SceneryManager.AllScenery.Remove(this);
    }

}