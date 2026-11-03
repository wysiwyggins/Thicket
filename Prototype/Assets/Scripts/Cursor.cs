using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class Cursor : MonoBehaviour
{

    Grid grid;
    //private Text TextOutput;
    private Tilemap fogTilemap;


    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        //TextOutput = GameObject.Find("Text").GetComponent<Text>();
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    Renderer[] renderers;
    bool cursorVisible = true;

    // The cursor is only usable while the player's piece is waiting for a move.
    static bool PlayerIsChoosingMove()
    {
        Piece current = PieceManager.CurrentPiece;
        if (current == null) return false;
        PlayerMovement movement = current.GetComponent<PlayerMovement>();
        return movement != null && movement.IsAwaitingInput;
    }

    void SetCursorVisible(bool visible)
    {
        if (visible == cursorVisible) return;
        cursorVisible = visible;
        foreach (Renderer r in renderers)
            r.enabled = visible;
    }

    // Update is called once per frame
    void Update()
    {
        // hidden outside the player's move, but still tracks the mouse and reports hex info on click
        SetCursorVisible(PlayerIsChoosingMove());

        Vector3 position = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;
        Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
        coordinate.x = Mathf.Clamp(coordinate.x, -6, 5); // need to plug this into the size of the map now that it's variable
        coordinate.y = Mathf.Clamp(coordinate.y, -5, 8);

        transform.position = grid.CellToWorld(coordinate);

        
			
        if (Input.GetMouseButtonDown(0))
        {
            
            Piece piece = PieceManager.GetPieceAtPos(coordinate);
            Scenery[] sceneries = SceneryManager.GetSceneriesAtPos(coordinate);
            Debug.Log("cursor location: " + coordinate.x + ","+ coordinate.y +" (cube:" + HexCoordinates.OffsetToCube(coordinate));



            //TextOutput.text += "cursor location: " + coordinate ;
            MessageManager.AddMessage("cursor location: " + coordinate.x + "," + coordinate.y);
            if (fogTilemap.GetTile(coordinate) == null)
            {
                if (piece != null)
                {

                    //TextOutput.text += piece.PieceName ;
                    MessageManager.AddMessage(piece.PieceName);

                }

                if (sceneries != null)
                {
                    foreach (Scenery scenery in sceneries)
                    {
                        MessageManager.AddMessage(scenery.SceneryName);
                    }
                    
                }
            } else
            {
                //TextOutput.text += "Darkness.\n";
                MessageManager.AddMessage("Darkness.");
            }
         

        }
    }
}
