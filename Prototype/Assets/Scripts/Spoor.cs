using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spoor : MonoBehaviour
{


    int drydown;
    // A Spoor is the object left each turn by the Scent behavior
    // Scent has an int called Drydown it gives to each Spoor, which is the number of turns a spoor takes to dissapear.
    // Spoor would get its color from Piece.color (and the opacity would derive from drydown as it ticks to zero)
    // animals with a prey will follow a trail of Spoor, moving from low to high Spoor.drydown
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckSpoor()
    {
        //tick down the drydown
        drydown -= 1;
        if (drydown < 0)
        {
            Destroy(this.gameObject, 1);
        }

    }
}
