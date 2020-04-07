using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public string pieceName = "unnamed";
    public Vector3 location;
    public int strength = 0;
    public int speed = 1;
    public bool isPlayer = false;
    public GameObject sprite;
    public GameObject prey; //what the piece is seeking
    public Transform goalHex;


    // Start is called before the first frame update
    void Start()
    {
        goalHex.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
