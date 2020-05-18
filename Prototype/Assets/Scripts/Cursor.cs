using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Cursor : MonoBehaviour
{

    Grid grid;
    public Tilemap cursorTilemap;
    private Text TextOutput;
    private Tilemap fogTilemap;


    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        TextOutput = GameObject.Find("Text").GetComponent<Text>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click

        transform.position = grid.CellToWorld(coordinate);

        
			
        if (Input.GetMouseButtonDown(0))
        {
            
            Piece piece = PieceManager.GetPieceAtPos(coordinate);
            Debug.Log("cursor location: " + coordinate);


            TextOutput.text += "cursor location: " + coordinate + "\n";
            if(fogTilemap.GetTile(coordinate) == null)
            {
                if (piece != null)
                {

                    TextOutput.text += piece.PieceName + "\n";
                }
            } else
            {
                TextOutput.text += "Darkness.\n";
            }
            
         

        }
    }
}
