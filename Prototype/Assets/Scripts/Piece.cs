using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    //piece attributes
    public delegate void PieceAction();
    public static event PieceAction OnCompleteMove;
    Vector3Int cellPosition;
    public string pieceName = "unnamed"; //this gets overridden with the name of the piece
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject prey; //what the piece is seeking

    //the world
    Grid grid;
    public Tilemap navmap;

    //pathing
    private SimplePF2D.Path path;
    private Vector3 nextPoint;
    private Rigidbody2D rb;
    private float speed = 0.5f;
    private bool isStationary = true;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GameController.instance.grid;
        rb = GetComponent<Rigidbody2D>();
        SimplePathFinding2D pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
        nextPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true)
        {
            Vector3 position = transform.position;
            Vector3Int cellPosition = grid.WorldToCell(transform.position);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0.0f;


            
            
            
            if (Input.GetMouseButtonDown(0))
            {

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
                Debug.Log("piece position: " + cellPosition);
                Debug.Log("destination: " + coordinate);
                path.CreatePath(position, mouseWorldPos);
                MoveTo(coordinate);

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
