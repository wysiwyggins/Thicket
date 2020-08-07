using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    public int map_size;
    public int bush;
    public int flowers;
    public int wolves;
    public int bears;
    public int chickens;
    public int water;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
