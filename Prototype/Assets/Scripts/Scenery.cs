using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scenery : MonoBehaviour
{
    Grid grid;
    Tilemap navigationTilemap;
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

        // opaque scenery (brambles) stands above the fog, so it fades out of view like pieces do
        if (Opaque && GetComponent<SpriteRenderer>() != null && GetComponent<FogTint>() == null)
            gameObject.AddComponent<FogTint>();

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
        // on scene unload the Grid/tilemap may already be destroyed
        if (Obstacle && grid != null && navigationTilemap != null)
        {
           navigationTilemap.SetTile(pieceCoords, null);
        }
        SceneryManager.AllScenery.Remove(this);
    }

}