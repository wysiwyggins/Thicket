using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string pieceName = "unnamed";
    public GameObject pieceSprite;
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public int strength = 0;
    public int speed = 1;
    public bool isPlayer = false;
    public GameObject prey; //what the piece is seeking


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
