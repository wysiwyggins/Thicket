using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    
    public delegate void PieceAction();

	public event PieceAction OnBeginMove;
	public event PieceAction OnCompleteMove;

	
   
    public string PieceName = "unnamed";


	public int Strength;
	public Color PieceColor;
    

    public bool isPlayer {get { return GetComponent<PlayerMovement>() != null; }}

	public Piece Mate;
	public bool isPredator;
	public List<Piece> preyList;
	public Piece Prey;

	public List<PieceBehaviour> pieceBehaviors = new List<PieceBehaviour>();
	PieceBehaviour curBehaviour;

	// --- Occupancy & pushing ---
	// A pushable piece (e.g. the chicken) never shares a hex: it won't move onto other animals, and
	// other animals can't move onto it unless they're stronger (a predator catching it).
	// The player's piece pushes it one hex along its direction of travel instead.

	public bool Pushable;

	const float pushSpeed = 4f;

	public Vector3Int Cell { get { return PieceManager.Instance.grid.WorldToCell(transform.position); } }

	// Can `mover` step onto `cell` without breaking the no-sharing rule?
	public static bool CanEnter(Piece mover, Vector3Int cell)
	{
		foreach (Piece other in PieceManager.GetPiecesAtPos(cell))
		{
			if (other == mover) continue;
			if (mover.Pushable) return false;
			if (other.Pushable && mover.Strength <= other.Strength) return false;
		}
		return true;
	}

	// The first pushable piece on `cell` other than `mover`, or null.
	public static Piece PushableAt(Vector3Int cell, Piece mover)
	{
		foreach (Piece other in PieceManager.GetPiecesAtPos(cell))
		{
			if (other != mover && other.Pushable) return other;
		}
		return null;
	}

	// Push this piece one hex further along the line from -> its current cell.
	// Returns false (and doesn't move) if the hex beyond is off the map, an obstacle, or occupied.
	public bool TryPush(Vector3Int from)
	{
		Vector3Int at = Cell;
		Vector3Int direction = HexCoordinates.OffsetToCube(at) - HexCoordinates.OffsetToCube(from);
		Vector3Int destination = HexCoordinates.CubeToOffset(HexCoordinates.OffsetToCube(at) + direction);

		Grid grid = PieceManager.Instance.grid;
		Vector3 destinationWorld = grid.CellToWorld(destination);
		SimplePF2D.NavNode node = grid.GetComponent<SimplePathFinding2D>().GetNode(destinationWorld);
		if (node == null || node.IsBlocked()) return false;
		if (PieceManager.GetPiecesAtPos(destination).Length > 0) return false;

		StartCoroutine(SlideTo(destinationWorld));
		MessageManager.AddMessage("The " + PieceName + " is pushed aside.");
		return true;
	}

	IEnumerator SlideTo(Vector3 target)
	{
		target.z = transform.position.z;
		while (Vector3.Distance(transform.position, target) > 0.01f)
		{
			transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * pushSpeed);
			yield return null;
		}
		transform.position = target;
	}

	public void BeginTurn()
	{
		
		if (OnBeginMove != null) OnBeginMove();
		curBehaviour = pieceBehaviors[0];
		SetBehaviour();
		MessageManager.AddMessage(PieceName + "'s Turn");

	}

	void SetBehaviour()
	{
		curBehaviour.OnCompleteMove += CurBehaviour_OnCompleteMove;
		curBehaviour.Begin();
	}

	private void CurBehaviour_OnCompleteMove()
	{
		curBehaviour.OnCompleteMove -= CurBehaviour_OnCompleteMove;
		int nextIndex = pieceBehaviors.IndexOf(curBehaviour) + 1;

		//if all behaviours have been done for this turn
		if (nextIndex >= pieceBehaviors.Count)
			EndTurn();
		else
		{
			curBehaviour = pieceBehaviors[nextIndex];
			SetBehaviour();
		}
	}

	public void EndTurn()
	{
		if (OnCompleteMove != null)
			OnCompleteMove();
	}


    void Awake()
    {
		// fog of war tinting is shared with brambles; see FogTint.cs
		fogTint = GetComponent<FogTint>();
		if (fogTint == null) fogTint = gameObject.AddComponent<FogTint>();
	}

	FogTint fogTint;

    // Start is called before the first frame update
    void Start()
    {
		fogTint.SetBaseColor(PieceColor);
	}

	private void OnEnable()
	{
		PieceManager.AllPieces.Add(this);
	}

	private void OnDestroy()
    {
        PieceManager.AllPieces.Remove(this);
    }




    // Update is called once per frame
    void Update()
    {
        
  
    }
}
