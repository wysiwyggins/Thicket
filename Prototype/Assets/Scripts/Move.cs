using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Move : Command
{

    // move is where everything happens,
    // move should be a function (pattern- command?)
    // that gets the mover location and the goal location,
    // the mover leaves behind a scent in the tiles it traverses.
    // if the mover moves onto aother piece of a lower strength, that piece is destroyed
    // any piece in water has a strength of zero


    private float velocity = 4;
    private Vector3 targetPosition;
    private bool isMoving = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
        if (Input.GetMouseButtonDown(0))
        {
            //get a hex cell from a mouse click
            Tilemap tilemap = GetComponent<Tilemap>();
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = tilemap.WorldToCell(mouseWorldPos);
            Debug.Log(coordinate);
            if (GameController.CurrentState == GameState.PlayerTurn)
            {
                //  PlayerMove(coordinate)

            }


        }

    }

    

    void SetTargetPosition()
    {

    }


    void PlayerMove(Vector3Int coordinate)
    {
        // get the player object
        // get the target hex cell
        // get the world position center of the target hex cell "coordinate"
        // transform.position = tilemap.GetCellCenterWorld(coordinate);


    }

    void AIMove(Vector3Int coordinate)
    {
        //get the ai object
        // get the target hex cell
        // get the world position center of the target hex cell "coordinate"
        // transform.position = tilemap.GetCellCenterWorld(coordinate);
    }
}
