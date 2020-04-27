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
    private Rigidbody2D rb;
    private float moveSpeed = 10.0f;
    private bool isStationary = true;
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

            }
            if (path.IsGenerated())
            {
                
                StartCoroutine(followPath());
                isStationary = false;
            }
        }
            
    }

    IEnumerator followPath()
    {
        List<Vector3Int> cellPositions = path.GetPathPointList();
        for (int i = 0; i < cellPositions.Count; i++)
        {
            MoveIE = StartCoroutine(Moving(i));
            yield return MoveIE;
        }
    }

    IEnumerator Moving(int currentPosition)
    {
        
        while (rb.transform.position != path.GetPathPointWorld(currentPosition))
        {
            rb.transform.position = Vector3.MoveTowards(rb.transform.position, path.GetPathPointWorld(currentPosition), moveSpeed * Time.deltaTime);
            yield return null;
        }

    }

}

