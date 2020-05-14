using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : PieceBehaviour
{
	//pathing
	private SimplePF2D.Path path;
	private float moveSpeed = 4f; //speed for Moving()
								  //Coroutine MoveIE;
	SimplePathFinding2D pf;

	Grid grid;
	Tilemap navmap;

	//piece attributes
	Vector3Int pieceCoords
	{ get { return grid.WorldToCell(transform.position); } }
	List<Vector3Int> cellPositions;
	

	public int range;

	enum State
	{
		Inactive,
		WaitForInput,
		Move,
	}
	State state;

	public override void Begin()
	{
		path.Reset();
		state = State.WaitForInput;
	}

	// Start is called before the first frame update
	void Start()
    {
		grid = GameObject.Find("Grid").GetComponent<Grid>(); //why does any second piece added say this reference isn't set to an instance of an object?
		pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
		path = new SimplePF2D.Path(pf);
		navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

	}

	
	void InputState()
	{

		Vector3 location = transform.position;
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.z = 0.0f;

		if (Input.GetMouseButtonDown(0) && PathIsValid() == false) //click the mouse
		{
			Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
																	 //Debug.Log("piece position: " + cellPosition);
			Debug.Log("destination: " + coordinate);
			path.CreatePath(location, mouseWorldPos); // generate a path

			Debug.Log("path length " + path.GetPathPointList().Count + "/" + range);
			if (path.GetPathPointList().Count > range)
			{
				path.Reset();
				Debug.Log("resetting path!");
			}
		
		}

		if (PathIsValid()) //once there's a path, transition to moving
		{
			cellPositions = path.GetPathPointList();
			StartCoroutine(followPath());
			state = State.Move;
		}
	}


    // Update is called once per frame
    void Update()
    {
		if (state == State.WaitForInput)
			InputState();
		
			
	}

	bool PathIsValid()
	{
		return path.IsGenerated() && path.GetPathPointList().Count <= range;
	}


	IEnumerator followPath()
	{
		List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
		if (cellPositions.Count > range)
		{
			cellPositions.RemoveRange(range, cellPositions.Count); //shave off the moves past piece.range for ai pieces
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
			Debug.Log("Piece: " + piece.pieceName + ", Location: " + pieceCoords);
			yield return new WaitForSeconds(0.05f);

		}

		SendCompleteMessage();

		path = new SimplePF2D.Path(pf);
	}
}
