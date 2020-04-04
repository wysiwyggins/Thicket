using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string pieceName = "unnamed";
    public GameObject pieceTexture = null;
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public int strength = 0;
    public int speed = 1;
    public bool isPlayer = false;

    // should be a var for which scent object the piece leaves & maybe if we wanted
    // to get elaborate how 'stinky' it is? (how long the scent lasts),
    // maybe this could be dialed down to have a scentless animal?)


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
