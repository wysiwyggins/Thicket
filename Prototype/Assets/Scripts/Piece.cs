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
    private float moveSpeed = 4f; //speed for Moving()
    private bool isStationary = true; //not using this yet
    Coroutine MoveIE;
    SimplePathFinding2D pf;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameController.instance.grid;
        rb = GetComponent<Rigidbody2D>();
        pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
        PieceManager.AllPieces.Add(this);
    }


    private void OnDestroy()
    {
        PieceManager.AllPieces.Remove(this);
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

            if (Input.GetMouseButtonDown(0) && path.IsGenerated() == false) //click the mouse
            {

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
                //Debug.Log("piece position: " + cellPosition);
                Debug.Log("destination: " + coordinate);
                path.CreatePath(position, mouseWorldPos); // generate a path

            }
            if (path.IsGenerated() && !following) //once there's a path
            {
                StartCoroutine(followPath());
                isStationary = false;
            }
        }

    }

    bool following = false;

    IEnumerator followPath()
    {
        following = true;
        List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
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
            yield return new WaitForSeconds(0.05f);

        }

        if (OnCompleteMove != null)
            OnCompleteMove();
        path = new SimplePF2D.Path(pf);
        following = false;
    }

}