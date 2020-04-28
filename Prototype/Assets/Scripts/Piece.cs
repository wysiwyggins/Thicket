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
    public Vector3Int[] cellPositions;
    public string pieceName = "unnamed"; 
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject prey; //what the piece is seeking

    //the world
    Grid grid;
    public Tilemap navmap;

    //pathing
    private SimplePF2D.Path path;
    private Rigidbody2D rb;
    private float moveSpeed = 2; //speed for Moving()
    private bool isStationary = true; //not using this yet
    Coroutine MoveIE;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GameController.instance.grid;
        rb = GetComponent<Rigidbody2D>();
        SimplePathFinding2D pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true) //it's the players turn, and im the player
        {
            Vector3 position = transform.position;
            //Vector3Int cellPosition = grid.WorldToCell(transform.position);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0.0f;


            
            
            
            if (Input.GetMouseButtonDown(0)) //click the mouse
            {

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
                //Debug.Log("piece position: " + cellPosition);
                Debug.Log("destination: " + coordinate);
                path.CreatePath(position, mouseWorldPos); // generate a path

            }
            if (path.IsGenerated()) //once there's a path
            {
                
                StartCoroutine(followPath());
                isStationary = false;
            }
        }
            
    }

    IEnumerator followPath()
    {
        List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
        for (int i = 0; i < cellPositions.Count; i++) //Loop through them (lists have a "count", not a "length")
        {
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
            
        }
    }

    IEnumerator Moving(int positionNumber)
    {

        while (transform.position != path.GetPathPointWorld(positionNumber)) //as  long as you're not at the destination (world point converted from grid point)
        {
            Debug.Log("Moving to: " + path.GetPathPointWorld(positionNumber));
            transform.position = Vector3.MoveTowards(transform.position, path.GetPathPointWorld(positionNumber), moveSpeed * Time.deltaTime); //this is not happy
            //transform.position = path.GetPathPointWorld(currentPosition);
            yield return null;
        }
        isStationary = true;

    }

}

