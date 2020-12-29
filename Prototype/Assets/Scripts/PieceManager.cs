using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class PieceManager : MonoBehaviour
{
	public static PieceManager Instance;

    public static List<Piece> AllPieces = new List<Piece>();

	public delegate void PieceAction();
	public static event PieceAction OnRoundComplete;

	Piece currentPiece;

	public Grid grid;

	//private Text TextOutput;

	private Tilemap fogTilemap;

	public int hour;
	public float sunPerc
	{
		get { return hour / 8f; }
	}

	public enum State
	{
		Dawn,
		Day,
		Dusk,
        Night,
	}

	public static State state;


	public static Piece GetPieceAtPos(Vector3Int aCoord)
    {
        foreach (Piece aPiece in AllPieces)
        {
            Vector3Int coordinate = Instance.grid.WorldToCell(aPiece.transform.position);
            if (aCoord == coordinate)
            {
				return aPiece;
            }
        }
        return null;
    }

	public static Piece[] GetPiecesAtPos(Vector3Int aCoord)
	{
		List<Piece> pieces = new List<Piece>();
		foreach (Piece aPiece in AllPieces)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aPiece.transform.position);
			if (aCoord == coordinate)
			{
				pieces.Add(aPiece);
			}
		}
		return  pieces.ToArray();
	}


	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
	}

	private void Start()
	{
		state = State.Dawn;
		currentPiece = AllPieces[0];
		StartPieceTurn();
	}


	void StartPieceTurn()
	{
		currentPiece.OnCompleteMove += CurrentPiece_OnCompleteMove;
		//MessageManager.AddMessage("Win: " + CheckWin()); 
		currentPiece.BeginTurn();
		
	}

	private void CurrentPiece_OnCompleteMove()
	{
		currentPiece.OnCompleteMove -= CurrentPiece_OnCompleteMove;

		int nextIndex = AllPieces.IndexOf(currentPiece) +1;

		if (nextIndex >= AllPieces.Count)
		{
			if (OnRoundComplete != null)
				OnRoundComplete();

			nextIndex = 0;
			hour += 1;
			MessageManager.AddMessage("Moment: " + hour );
			
			CheckHour();
			
		}

		if (CheckWin())
		{
			MessageManager.AddMessage("No predators left!");
		}
		else
		{
			MessageManager.AddMessage("Nobody won");
		}
		//if (CheckLose())
		//{
		//	MessageManager.AddMessage("All prey have been eliminated!");
  //      }
		currentPiece = AllPieces[nextIndex];
		StartPieceTurn();
		


	}


    
    private bool CheckWin()
    {
        List<Piece> hunters = new List<Piece>();
        foreach (Piece aPiece in AllPieces)
        {

            if (aPiece.Prey != null) //this isn't whether a piece is prey or not, it's whether a piece HAS prey or not
            {
                hunters.Add(aPiece);
            }
        }
        if (hunters.Count > 0)
        {
            foreach (Piece aPiece in AllPieces) //for every piece on the board, if there's something still hunting it, don't stop play
            {
                foreach (Piece aHunter in hunters)
                {
                    if (aHunter.Prey == aPiece)
                        return false;
                }
            }
        }
        else
        {
            return true; // if there's no hunters on the board you win. Let's fine tune this to if nothing is hunting the player? 
        }
        return false; // this is just checking win, so we probably need a separate checklose() function
    }

	private bool CheckLose() //this isn't working yet. no message when prey are eliminated
    {
		List<Piece> hunters = new List<Piece>();
		foreach (Piece aPiece in AllPieces)
		{

			if (aPiece.Prey != null) //this isn't whether a piece is prey or not, it's whether a piece HAS prey or not
			{
				hunters.Add(aPiece);
			}
		}
		if (hunters.Count > 0)
		{
			foreach (Piece aPiece in AllPieces) //for every piece on the board, if there's something still hunting it, don't stop play
			{
				foreach (Piece aHunter in hunters)
				{
					if (aHunter.Prey == aPiece)
						return false;
				}
			}
		} else
        {
			return false; //there's no hunters
        }
		return true; //if no hunters have any prey
	}


    private void CheckHour()
    {
		
		if (hour == 2)
		{
			state = State.Day;
			//TextOutput.text += "The sun is high in the air now, you feel its warmth over your skin.\n";
			MessageManager.AddMessage("The sun is high in the air now, you feel its warmth over your skin.");
		}
		if (hour == 6)
		{
			state = State.Dusk;
			//TextOutput.text += "The sun dwindles, red, in the west. There is a shudder across the land as the wolf begins to stir in her den.\n";
			MessageManager.AddMessage("The sun dwindles, red, in the west.");
			


		}
		if (hour == 8)
		{
			state = State.Night;
			MessageManager.AddMessage("The sun is swallowed up by the dark horizon of the earth.");
            
        }
		if (hour >= 13)
		{
			state = State.Dawn;
			//TextOutput.text += "The air around you sings to life. The red sun wakes over the land.\n";
			MessageManager.AddMessage("The air around you sings to life. The red sun wakes over the land.");
			hour = 0;
		}
	}
}
