using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public delegate void PieceAction();
    public static event PieceAction OnCompleteMove;
    Vector3Int cellPosition;
    public string pieceName = "unnamed"; //this gets overridden with the name of the piece
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject prey; //what the piece is seeking
    Grid grid;

    private SimplePF2D.Path path;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GameController.instance.grid;

        SimplePathFinding2D pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //get a hex cell from a mouse click
            Vector3 position = transform.position;
            Vector3Int cellPosition = grid.WorldToCell(transform.position);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
            Debug.Log("piece position: " + cellPosition);
            Debug.Log("destination: " +coordinate);
            if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true)
            {

                path.CreatePath(position, mouseWorldPos);
                //MoveTo(coordinate);

            }
        }
    }
    

    public void MoveTo(Vector3Int targetCell)
    {

        // List<Vector3Int> CellPath = AStarPathFunction(targetCell);

        // List<Vector3> WorldCoords = CellToWorld(CellPath);

        // start a coroutine which animates the object along a path
        // StartCoroutine(FollowPath(aPath));
        transform.position = grid.GetCellCenterWorld(targetCell);
    }


}
