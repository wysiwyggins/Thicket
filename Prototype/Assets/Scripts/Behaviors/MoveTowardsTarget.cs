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

	// --- Hunting: see the prey, else follow its scent, else wander ---

	public int smellRange = 2; // how many hexes away a hunter can pick up its prey's trail

	// Is the prey within sight? Uses the same day/night range and bramble blocking as the deer's own view.
	bool CanSeePrey()
	{
		Vector3Int self = HexCoordinates.OffsetToCube(grid.WorldToCell(transform.position));
		Vector3Int prey = HexCoordinates.OffsetToCube(grid.WorldToCell(piece.Prey.transform.position));
		return HexCoordinates.HasLineOfSight(self, prey, Vision.SightRange());
	}

	// Freshest of the prey's spoor on `cell`, or null.
	Spoor PreySpoorAt(Vector3Int cell)
	{
		Spoor freshest = null;
		foreach (Spoor spoor in SpoorManager.GetSpoorAtPos(cell))
		{
			if (spoor != null && spoor.source == piece.Prey && (freshest == null || spoor.order > freshest.order))
				freshest = spoor;
		}
		return freshest;
	}

	// Pick up the prey's trail within smellRange, then run along it towards fresher scent as far as
	// this piece can move this turn. Returns false if there's no trail nearby or we're already at its end.
	bool TryFollowTrail(out Vector3Int target)
	{
		Vector3Int self = grid.WorldToCell(transform.position);
		target = self;

		// strongest scent we can smell from here
		Spoor current = null;
		foreach (Vector3Int cube in HexCoordinates.GetHexesAtDistance(HexCoordinates.OffsetToCube(self), smellRange))
		{
			Vector3Int cell = HexCoordinates.CubeToOffset(cube);
			Spoor spoor = PreySpoorAt(cell);
			if (spoor != null && (current == null || spoor.order > current.order))
			{
				current = spoor;
				target = cell;
			}
		}
		if (current == null) return false;

		// follow the trail onwards from there, one fresher neighbour at a time
		for (int step = 0; step < range; step++)
		{
			Spoor next = null;
			Vector3Int nextCell = target;
			foreach (Vector3Int neighbor in HexCoordinates.GetNeighborsAtPos(target))
			{
				Spoor spoor = PreySpoorAt(neighbor);
				if (spoor != null && spoor.order > current.order && (next == null || spoor.order > next.order))
				{
					next = spoor;
					nextCell = neighbor;
				}
			}
			if (next == null) break;
			current = next;
			target = nextCell;
		}
		return target != self;
	}

	// --- Fleeing: animals that see something able to eat them run away from it ---

	bool fleeing; // so "flees" is only announced when an animal starts running

	// Animals that could eat this one and are within its sight (same range/bramble rules as hunting).
	List<Piece> VisibleThreats()
	{
		var threats = new List<Piece>();
		Vector3Int self = HexCoordinates.OffsetToCube(grid.WorldToCell(transform.position));
		int sight = Vision.SightRange();
		foreach (Piece other in PieceManager.AllPieces)
		{
			if (other == null || other == piece || other.IsBeingRemoved || !other.CanEat(piece)) continue;
			Vector3Int otherCube = HexCoordinates.OffsetToCube(grid.WorldToCell(other.transform.position));
			if (HexCoordinates.HasLineOfSight(self, otherCube, sight))
				threats.Add(other);
		}
		return threats;
	}

	// The open hex within one move that gets furthest from the nearest threat (ties: furthest from all of them).
	// Returns false if nowhere in reach is safer than staying put.
	bool TryFindEscape(List<Piece> threats, out Vector3Int escape)
	{
		var threatCubes = new List<Vector3Int>();
		foreach (Piece threat in threats)
			threatCubes.Add(HexCoordinates.OffsetToCube(grid.WorldToCell(threat.transform.position)));

		int nearest(Vector3Int cube)
		{
			int min = int.MaxValue;
			foreach (Vector3Int t in threatCubes) min = Mathf.Min(min, HexCoordinates.CubeDistance(cube, t));
			return min;
		}
		int total(Vector3Int cube)
		{
			int sum = 0;
			foreach (Vector3Int t in threatCubes) sum += HexCoordinates.CubeDistance(cube, t);
			return sum;
		}

		Vector3Int selfCell = grid.WorldToCell(transform.position);
		Vector3Int selfCube = HexCoordinates.OffsetToCube(selfCell);
		escape = selfCell;
		int bestNearest = nearest(selfCube);
		int bestTotal = total(selfCube);
		bool found = false;

		int moveHexes = Mathf.Max(1, range - 1); // range counts the starting hex
		foreach (Vector3Int cube in HexCoordinates.GetHexesAtDistance(selfCube, moveHexes))
		{
			if (cube == selfCube) continue;
			Vector3Int cell = HexCoordinates.CubeToOffset(cube);
			NavNode node = pf.GetNode(grid.CellToWorld(cell));
			if (node == null || node.IsBlocked()) continue;               // brambles, map edge, chickens we can't share with
			if (PieceManager.GetPiecesAtPos(cell).Length > 0) continue;   // don't run into anyone

			int n = nearest(cube), t = total(cube);
			if (n > bestNearest || (n == bestNearest && t > bestTotal))
			{
				bestNearest = n;
				bestTotal = t;
				escape = cell;
				found = true;
			}
		}
		return found;
	}

	void Wander()
	{
		int randomX = Random.Range(-5, 5);
		int randomY = Random.Range(-5, 8);
		randomWorldTarget = grid.CellToWorld(new Vector3Int(randomX, randomY, 0));
		NavNode node = pf.GetNode(randomWorldTarget);
		if (node != null && !node.IsBlocked()) // otherwise pick another spot next frame
			FindPath(randomWorldTarget);
	}

    private void Update()
	{
		if (state != State.FindingPath) return;

		// staying alive comes first
		List<Piece> threats = VisibleThreats();
		if (threats.Count > 0)
		{
			bool announce = !fleeing;
			fleeing = true;
			if (TryFindEscape(threats, out Vector3Int escapeCell))
			{
				if (announce)
					MessageManager.AddMessage("The " + piece.PieceName + " flees from the " + threats[0].PieceName + ".");
				Vector3 escapePosition = grid.CellToWorld(escapeCell);
				escapePosition.z = 0.0f;
				FindPath(escapePosition);
			}
			else
			{
				// cornered: nowhere safer in reach, so hold still rather than wander towards danger
				if (announce)
					MessageManager.AddMessage("The " + piece.PieceName + " freezes.");
				EndMove();
			}
			return;
		}
		fleeing = false;

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
			
			if (CanSeePrey())
			{
				// charge straight at it
				preyPosition = piece.Prey.transform.position;
				preyPosition.z = 0.0f;
				FindPath(preyPosition);
			}
			else if (TryFollowTrail(out Vector3Int trailCell))
			{
				// FindPath wants a world position, not a cell
				Vector3 trailPosition = grid.CellToWorld(trailCell);
				trailPosition.z = 0.0f;
				FindPath(trailPosition);
			}
			else
			{
				Wander(); // lost the prey: no sight, no scent
			}


			//else if (state == State.Move)
		}
		else
		{
			Wander();
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
