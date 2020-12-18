﻿using System.Collections;
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
    }

    // Update is called once per frame
    void Update()
    {
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
