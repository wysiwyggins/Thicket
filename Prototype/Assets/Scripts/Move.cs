using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
       
    }

    void SetTargetPosition()
    {
        
    }

    void PlayerMove()
    {

    }

    void AIMove()
    {

    }
}
