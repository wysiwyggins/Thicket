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


    private Vector2 velocity;
    private Vector3 direction;
    private bool hasMoved;

    // Update is called once per frame
    void Update()
    {
        if (velocity.x == 0)
        {
            hasMoved = false;
        } else if(velocity.x !0 && !hasMoved)
        {
            hasMoved = true;
            MoveByDirection();
        }
    }

    private void MoveByDirection()
    {
        if (velocity.x < 0) //Move Left
        {
            if (velocity.y > 0) //Move upper Left
            {
                direction = new Vector3(-0.5f, 0.5f);
            }
            else if(velocity.y < 0)
            {
                dire3ction = new Vector3(-0.5f, -0.5f);
            } else
            {
                //not done here- this was movement for keypresses so this might all be unusable
            }
        }
    }
}
