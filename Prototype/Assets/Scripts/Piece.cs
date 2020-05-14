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
    

    public bool isPlayer {get { return GetComponent<PlayerMovement>() != null; }}

	public Piece Mate;
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
        
  
    }
}
