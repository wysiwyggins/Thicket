using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Cursor : MonoBehaviour
{

    Grid grid;
    public Tilemap cursorTilemap;
    //private Text TextOutput;
    private Tilemap fogTilemap;


    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        //TextOutput = GameObject.Find("Text").GetComponent<Text>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
        coordinate.x = Mathf.Clamp(coordinate.x, -5, 5);
        coordinate.y = Mathf.Clamp(coordinate.y, -5, 8);

        transform.position = grid.CellToWorld(coordinate);

        
			
        if (Input.GetMouseButtonDown(0))
        {
            
            Piece piece = PieceManager.GetPieceAtPos(coordinate);
            Scenery scenery = SceneryManager.GetSceneryAtPos(coordinate);
            Debug.Log("cursor location: " + coordinate);


            //TextOutput.text += "cursor location: " + coordinate ;
            MessageManager.AddMessage("cursor location: " + coordinate );
            if (fogTilemap.GetTile(coordinate) == null)
            {
                if (piece != null)
                {

                    //TextOutput.text += piece.PieceName ;
                    MessageManager.AddMessage(piece.PieceName);

                }

                if (scenery != null)
                {
                    MessageManager.AddMessage(scenery.SceneryName);
                }
            } else
            {
                //TextOutput.text += "Darkness.\n";
                MessageManager.AddMessage("Darkness.");
            }
            
         

        }
    }
}
