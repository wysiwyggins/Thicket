using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public delegate void PieceAction();
    public static event PieceAction OnCompleteMove;

    public string pieceName = "unnamed"; //this gets overridden with the name of the piece
    public Vector3Int cellPosition;  //grid position
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject sprite;
    public GameObject prey; //what the piece is seeking
    Grid grid;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GameController.instance.grid;
        // snap all pieces to their grid
        Vector3Int cellPosition = grid.WorldToCell(transform.position);
        transform.position = grid.CellToWorld(cellPosition);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //get a hex cell from a mouse click
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
            Debug.Log(coordinate);
            if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true)
            {
                MoveTo(coordinate);
                
             
            }
        }
    }


    public void MoveTo(Vector3Int targetCell)
    {
        transform.position = grid.GetCellCenterWorld(targetCell);
        // List<Vector3Int> CellPath = AStarPathFunction(targetCell);

        // List<Vector3> WorldCoords = CellToWorld(CellPath);

        // start a coroutine which animates the object along a path
        // StartCoroutine(FollowPath(aPath));
    }


}
