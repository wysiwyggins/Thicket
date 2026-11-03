using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using SimplePF2D;

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
	private Vector3 randomWorldTarget;

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
		//Debug.Log(gameObject.name + " movetowardstarget begin");
		sleep = GetComponent<Sleep>();
		if (sleep != null)
		{
			if (sleep.ShouldSleep())
			{
				SendCompleteMessage();
				return;
			}
		}
		//Debug.Log(gameObject.name + " reset path, entering state findingpath");
		path.Reset();
		state = State.FindingPath;
		startedMove = false;
		ChoosePrey();
		BlockForbiddenCells();
	}

	bool startedMove = false;

	// --- Routing around pieces we can't share a hex with (e.g. chickens) ---
	// Turns are taken one piece at a time, so during this piece's move we mark those hexes as
	// blocked in the pathfinder, and restore them when the move ends.

	readonly List<NavNode> tempBlocked = new List<NavNode>();

	void BlockForbiddenCells()
	{
		UnblockForbiddenCells();
		foreach (Piece other in PieceManager.AllPieces)
		{
			if (other == null || other == piece) continue;
			Vector3Int cell = other.Cell;
			if (Piece.CanEnter(piece, cell)) continue;
			NavNode node = pf.GetNode(grid.CellToWorld(cell));
			if (node != null && !node.IsBlocked()) // leave real obstacles alone
			{
				node.SetBlocked(true);
				tempBlocked.Add(node);
			}
		}
	}

	void UnblockForbiddenCells()
	{
		foreach (NavNode node in tempBlocked)
			node.SetBlocked(false);
		tempBlocked.Clear();
	}

	void EndMove()
	{
		state = State.Inactive;
		UnblockForbiddenCells();
		SendCompleteMessage();
	}

	void OnDestroy()
	{
		UnblockForbiddenCells(); // e.g. eaten mid-turn
	}

	// Start is called before the first frame update
	void Awake()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
		pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
		path = new SimplePF2D.Path(pf);
		navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

	}

	// Predators hunt the player. Done once per turn (it used to run every frame and grow preyList forever).
	void ChoosePrey()
	{
		if (!piece.isPredator) return;
		foreach (Piece aPiece in PieceManager.AllPieces)
		{
			if (aPiece != null && aPiece.isPlayer)
			{
				if (!piece.preyList.Contains(aPiece))
					piece.preyList.Add(aPiece);
				piece.Prey = aPiece;
			}
		}
	}

	// Hex of the freshest scent left by our prey next to us, if it's fresher than the scent we're standing on.
	// Returns false if we're not on the prey's trail, or we're at the freshest point of it.
	bool TryGetFresherScent(Vector3Int selfCoords, out Vector3Int fresherCoords)
	{
		fresherCoords = selfCoords;
		int bestDrydown = -1;
		foreach (Spoor spoor in SpoorManager.GetSpoorAtPos(selfCoords))
		{
			if (spoor.source == piece.Prey)
				bestDrydown = Mathf.Max(bestDrydown, spoor.drydown);
		}
		if (bestDrydown < 0) return false; // no trail here

		bool found = false;
		foreach (Vector3Int coord in HexCoordinates.GetNeighborsAtPos(selfCoords))
		{
			foreach (Spoor neighborSpoor in SpoorManager.GetSpoorAtPos(coord))
			{
				if (neighborSpoor.source == piece.Prey && neighborSpoor.drydown > bestDrydown)
				{
					bestDrydown = neighborSpoor.drydown;
					fresherCoords = coord;
					found = true;
				}
			}
		}
		return found;
	}

    private void Update()
	{
		if (state != State.FindingPath) return;

		if (sleep != null && sleep.ShouldGoHome())
		{
			if(sleep.Home != null)
            {
				homePosition = sleep.Home.transform.position;
				homePosition.z = 0.0f;
				FindPath(homePosition);
			} else
            {
				//List<Scenery> dens = new List<Scenery>;
				GameObject[] dens = GameObject.FindGameObjectsWithTag("den");
				sleep.Home = dens[0].GetComponent<Scenery>();
			}
			
		}
		else if (piece.Prey)
		{
			
			Vector3Int selfCoords = grid.WorldToCell(transform.position);

			if (TryGetFresherScent(selfCoords, out Vector3Int scentCoords))
			{
				// follow the trail one hex towards fresher scent (FindPath wants a world position, not a cell)
				Vector3 scentPosition = grid.CellToWorld(scentCoords);
				scentPosition.z = 0.0f;
				FindPath(scentPosition);
			}
			else
			{
				// no trail here, or we're already on its freshest point: head straight for the prey
				//eventually this will only happen if in line-of-sight
				preyPosition = piece.Prey.transform.position;
				preyPosition.z = 0.0f;
				FindPath(preyPosition);
			}


			//else if (state == State.Move)
		}
		else
		{
			//Debug.Log("The " + piece.PieceName + " is not sleepy, has no prey");
			int randomX = Random.Range(-5, 5);
			int randomY = Random.Range(-5, 8);

			//Debug.Log(piece.PieceName + " thinks its going to: "+ randomX +","+ randomY);
			randomWorldTarget = grid.CellToWorld(new Vector3Int(randomX, randomY, 0));
			NavNode node = pf.GetNode(randomWorldTarget);
			if (node != null && !node.IsBlocked()) // otherwise pick another spot next frame
			{
				FindPath(randomWorldTarget);
			}	
		}
	}


	void FindPath(Vector3 target)
	{
		// Update can call this several times in one frame (e.g. per spoor); only act while still looking for a path
		if (state != State.FindingPath) return;

		Vector3 location = transform.position;


		if (!startedMove && PathIsValid() == false)
		{
			startedMove = true;

			Vector3Int coordinate = grid.WorldToCell(target);
			//Debug.Log(piece.PieceName + " actual destination: " + coordinate);
			if (!path.CreatePath(location, target)) // generate a path
			{
				EndMove(); // target is blocked (e.g. a chicken is standing on it): skip this move
				return;
			}

			//Debug.Log("path length " + path.GetPathPointList().Count + "/" + range);
			if (path.GetPathPointList().Count > range)
			{
				path.Reset();
				//Debug.Log("resetting path!");
			}

		}

		if (path.HasFailed()) // no route exists (e.g. walled in by chickens and brambles): skip this move
		{
			EndMove();
			return;
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
			//Debug.Log("Get path point");
			Vector3 targetPos = path.GetPathPointWorld(i);

			// pushable pieces (chickens) don't share hexes: stop short rather than step into a forbidden one
			Vector3Int nextCell = grid.WorldToCell(targetPos);
			if (i != 0 && nextCell != pieceCoords && !Piece.CanEnter(piece, nextCell))
				break;

			Vector3 vel = Vector3.zero;
			while (Vector3.Distance(transform.position, targetPos) > 0.01f)
			{
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, 0.2f, moveSpeed);

				//  transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
				yield return null; // next frame
			}

			if(i != 0)
				OnEnterTile.Invoke();

			//Debug.Log("Reached path point");
			//Debug.Log("Piece: " + piece.PieceName + ", Location: " + pieceCoords);
			yield return new WaitForSeconds(0.05f);

		}

		path = new SimplePF2D.Path(pf);
		EndMove();
	}
}
