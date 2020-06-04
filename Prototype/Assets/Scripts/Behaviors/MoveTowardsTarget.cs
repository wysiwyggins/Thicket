using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class MoveTowardsTarget : PieceBehaviour
{
	private SimplePF2D.Path path;
	private float moveSpeed = 4f; //speed for Moving()

	SimplePathFinding2D pf;

	Grid grid;
	Tilemap navmap;
	Sleep sleep;


	//piece attributes
	Vector3Int pieceCoords
	{ get { return grid.WorldToCell(transform.position); } }
	List<Vector3Int> cellPositions;

	private Vector3 homePosition;
	private Vector3 preyPosition;

	public int range;
	public UnityEvent OnEnterTile;

	enum State
	{
		Inactive,
		FindingPath,
		Move,
	}
	State state;

	public override void Begin()
	{
		Debug.Log(gameObject.name + " movetowardstarget begin");
		sleep = GetComponent<Sleep>();
		if (sleep != null)
		{
			if (sleep.ShouldSleep())
			{
				SendCompleteMessage();
				return;
			}
		}
		Debug.Log(gameObject.name + " reset path, entering state findingpath");
		path.Reset();
		state = State.FindingPath;
		startedMove = false;
	}

	bool startedMove = false;

	// Start is called before the first frame update
	void Start()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
		pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
		path = new SimplePF2D.Path(pf);
		navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

	}

	private void Update()
	{
		if (state != State.FindingPath) return;

		if (sleep != null && sleep.ShouldGoHome())
		{
			homePosition = sleep.Home.transform.position;
			homePosition.z = 0.0f;
			FindPath(homePosition);
		}
		else if (piece.prey)
		{
			preyPosition = piece.prey.transform.position;
			preyPosition.z = 0.0f;
			FindPath(preyPosition);
			//else if (state == State.Move)
		}
		else
		{
			Debug.Log("The " + piece.PieceName + " is not sleepy, has no prey");
			int randomX = Random.Range(-5, 5) + 1; //I know this is weird, this is the currrent range of the board plus an offset fix
			int randomY = Random.Range(-8, 8) -1 ;
			Debug.Log(piece.PieceName + " destination: "+ randomX +","+ randomY); 
			FindPath(new Vector3Int(randomX, randomY, 0));
		}
	}


	void FindPath(Vector3 target)
	{
		Vector3 location = transform.position;
		

		if (!startedMove && PathIsValid() == false) //click the mouse
		{
			startedMove = true;

			Vector3Int coordinate = grid.WorldToCell(target); //get a hex cell coordinate from a mouse click
																	 //Debug.Log("piece position: " + cellPosition);
			Debug.Log("destination: " + coordinate);
			path.CreatePath(location, target); // generate a path

			Debug.Log("path length " + path.GetPathPointList().Count + "/" + range);
			if (path.GetPathPointList().Count > range)
			{
				path.Reset();
				Debug.Log("resetting path!");
			}

		}

		if (path.IsGenerated()) //once there's a path, transition to moving
		{
			cellPositions = path.GetPathPointList();
			StartCoroutine(followPath());
			state = State.Move;
		}
	}



	bool PathIsValid()
	{
		return path.IsGenerated() && path.GetPathPointList().Count <= range;
	}


	IEnumerator followPath()
	{
		List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
		
		for (int i = 0; i < cellPositions.Count && i < range; i++) //Loop through them (lists have a "count", not a "length")
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

			if(i != 0)
				OnEnterTile.Invoke();

			Debug.Log("Reached path point");
			Debug.Log("Piece: " + piece.PieceName + ", Location: " + pieceCoords);
			yield return new WaitForSeconds(0.05f);

		}

		SendCompleteMessage();

		path = new SimplePF2D.Path(pf);
	}
}
