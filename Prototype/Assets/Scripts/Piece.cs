using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public string pieceName = "unnamed";
    public Vector3 location;
    public int strength = 0;
    public int speed = 1;
    public bool isPlayer = false;
    public GameObject sprite;
    public GameObject prey; //what the piece is seeking
    public Transform movePoint;

    // Start is called before the first frame update
    void Start()
    {
        GridLayout gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        Vector3Int cellPosition = gridLayout.WorldToCell(transform.position);
        transform.position = gridLayout.CellToWorld(cellPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
