using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    
    public delegate void PieceAction();

	public event PieceAction OnBeginMove;
	public event PieceAction OnCompleteMove;


    public static event PieceAction InvalidInput;

    //the world
   
    public string pieceName = "unnamed";
    public int strength = 0; // can take pieces with strength under this number
    public bool isPlayer {get { return GetComponent<PlayerMovement>() != null; }}

	public Piece prey;
   
	public List<PieceBehaviour> pieceBehaviors = new List<PieceBehaviour>();
	PieceBehaviour curBehaviour;

	public void BeginTurn()
	{
		if (OnBeginMove != null) OnBeginMove();
		curBehaviour = pieceBehaviors[0];
		SetBehaviour();
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


    // Start is called before the first frame update
    void Start()
    {
       

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
        
  
/*
      //  if (GameController.CurrentState == GameState.AITurn && isPlayer == false) //we need to change this to a list of all the ai pieces moving one at a time
        {
            if (prey)
            {
                Vector3 location = transform.position;
                Vector3 preyLocation = prey.transform.position;
                Debug.Log("Piece: " + pieceName + " is hunting: " + prey.name);
                Vector3Int selfCoord = grid.WorldToCell(location);
                Vector3Int preyCoord = grid.WorldToCell(preyLocation);
                Debug.Log("origin: " + selfCoord);
                Debug.Log("destination: " + preyCoord);
                path.CreatePath(location, preyLocation); // generate a path
				Debug.Log(path.GetPathPointList().Count);
                cellPositions = path.GetPathPointList(); 
                Debug.Log("goal is " + cellPositions.Count + " positions away."); // why is this saying it's 0 positions away?
				Debug.Log("following? " + following);
                if (path.IsGenerated() && !following) //once there's a path
                {
                    Debug.Log("There's a path!");

                   validateMove();
                }
            } else
            {
                Debug.Log("Piece: " + pieceName + " doesn't seem to be hunting anything.");
            }
            

        }
		*/
    }
}
