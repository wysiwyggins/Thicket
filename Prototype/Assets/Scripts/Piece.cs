using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    //piece attributes
    public delegate void PieceAction();
    public static event PieceAction OnCompleteMove;
    public static event PieceAction InvalidInput;

    //the world
    Grid grid;
    public Tilemap navmap;

    Vector3Int cellPosition;
    List<Vector3Int> cellPositions;
    public string pieceName = "unnamed";
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject prey; //what the piece is seeking
    Vector3Int pieceCoords;
    bool validatingMove;
   

    //pathing
    private SimplePF2D.Path path;
    private Rigidbody2D rb;
    private float moveSpeed = 4f; //speed for Moving()
    private bool isStationary = true; //not using this yet
    //Coroutine MoveIE;
    SimplePathFinding2D pf;
    bool following = false;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>(); //why does any second piece added say this reference isn't set to an instance of an object?
        rb = GetComponent<Rigidbody2D>();
        pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
        PieceManager.AllPieces.Add(this);
        pieceCoords = grid.WorldToCell(transform.position);
        validatingMove = false;
        Debug.Log("Piece: " + pieceName + ", Location: " + pieceCoords);

    }


    private void OnDestroy()
    {
        PieceManager.AllPieces.Remove(this);
    }


    // Update is called once per frame
    void Update()
    {
        
            
            if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true) //it's the players turn, and i'm the player
            {   
                Vector3 position = transform.position;
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = 0.0f;


                //how do we highlight the traversable cells within the range of the piece?


                if (Input.GetMouseButtonDown(0) && path.IsGenerated() == false) //click the mouse
                {

                    Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
                                                                             //Debug.Log("piece position: " + cellPosition);
                    Debug.Log("destination: " + coordinate);
                    path.CreatePath(position, mouseWorldPos); // generate a path
                    

                }
                if (path.IsGenerated() && !following) //once there's a path
                {
                
                    cellPositions = path.GetPathPointList();
                
                if (validatingMove == false)
                {
                    Debug.Log("move is " + cellPositions.Count + " positions away.");
                    validateMove();
                }            
            }
        }


        if (GameController.CurrentState == GameState.AITurn && isPlayer == false) //we need to change this to a list of all the ai pieces moving one at a time
        {
            if (prey)
            {
                Vector3 preyLocation = prey.transform.position;
                Debug.Log("Piece: " + pieceName + " is hunting: " + prey.name);
                
                Vector3Int preyCoord = grid.WorldToCell(preyLocation); 
                Debug.Log("destination: " + preyCoord);
                path.CreatePath(transform.position, prey.transform.position); // generate a path
                cellPositions = path.GetPathPointList();
                Debug.Log("move is " + cellPositions.Count + " positions away.");

                if (path.IsGenerated() && !following) //once there's a path
                {
                    
                    StartCoroutine(followPath());
                }
            } else
            {
                Debug.Log("Piece: " + pieceName + " doesn't seem to be hunting anything.");
            }
            

        }

    }


    void validateMove()
    {

        validatingMove = true;
        if (cellPositions.Count <= range)
        {
            StartCoroutine(followPath());
            isStationary = false;
        }
        else if (InvalidInput != null)
        {
            InvalidInput();
            pf.DebugClearPathMarker();
        }
    }

    

    IEnumerator followPath()
    {
        following = true;
        List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
        if (cellPositions.Count > range)
        {
            cellPositions.RemoveRange(range + 1, cellPositions.Count);
        }
        for (int i = 0; i < cellPositions.Count; i++) //Loop through them (lists have a "count", not a "length")
        {
            Debug.Log("Get path point");
            Vector3 targetPos = path.GetPathPointWorld(i);

            Vector3 vel = Vector3.zero;
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, 0.2f, moveSpeed);

                //  transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Reached path point");
            pieceCoords = grid.WorldToCell(transform.position);
            Debug.Log("Piece: " + pieceName + ", Location: " + pieceCoords);
            yield return new WaitForSeconds(0.05f);

        }

        if (OnCompleteMove != null)
            OnCompleteMove();
        path = new SimplePF2D.Path(pf);
        following = false;
    }

 

}

