using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cursor : MonoBehaviour
{

    Grid grid;
    public Tilemap cursorTilemap;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click

        transform.position = grid.CellToWorld(coordinate);

        //Debug.Log("cursor location: " + coordinate);
        //Debug.Log(cursorTilemap.GetSprite(coordinate));
			
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
}
