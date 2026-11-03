using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.SceneManagement;

public class PieceManager : MonoBehaviour
{
	public static PieceManager Instance;

    public static List<Piece> AllPieces = new List<Piece>();

	public delegate void PieceAction();
	public static event PieceAction OnRoundComplete;

	Piece currentPiece;
	public static Piece CurrentPiece { get { return Instance != null ? Instance.currentPiece : null; } }

	// TMP text on the Canvas showing whose turn it is, and the game-over message.
	// Assign in the Inspector, or it's found by name ("Title").
	public TMP_Text titleText;

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

	// --- Game over ---
	// Win: the deer is safe once no predators are left in play.
	// Lose: the deer is eaten.
	// Either way the board keeps running and R restarts.

	bool gameOver;

	// Called when a piece is taken out of play (currently: eaten, see ConsumePiece).
	public static void OnPieceRemoved(Piece removed, Piece eatenBy = null)
	{
		if (Instance == null || Instance.gameOver || removed == null) return;

		if (removed.isPlayer)
		{
			string cause = eatenBy != null ? " was caught by the " + eatenBy.PieceName : " was caught";
			Instance.EndGame("The " + removed.PieceName + cause + ".", "The " + removed.PieceName + cause);
			return;
		}

		if (!removed.isPredator) return;
		Piece player = null;
		foreach (Piece aPiece in AllPieces)
		{
			if (aPiece == null || aPiece == removed || aPiece.IsBeingRemoved) continue;
			if (aPiece.isPredator) return; // still hunted
			if (aPiece.isPlayer) player = aPiece;
		}
		if (player != null)
			Instance.EndGame("The " + player.PieceName + " is safe.", "The " + player.PieceName + " is safe");
	}

	void EndGame(string message, string title)
	{
		gameOver = true;
		MessageManager.AddMessage(message);
		MessageManager.AddMessage("Press R to restart game.");
		if (titleText != null)
			titleText.text = title + "<size=60%>\nPress R to restart</size>";
	}

	private void Update()
	{
		// legacy Input works because Active Input Handling is set to Both
		if (gameOver && Input.GetKeyDown(KeyCode.R))
			RestartGame();
	}

	void RestartGame()
	{
		MessageManager.ClearMessages();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void Start()
	{
		state = State.Dawn;
		if (titleText == null)
		{
			GameObject titleObject = GameObject.Find("Title");
			if (titleObject != null)
				titleText = titleObject.GetComponent<TMP_Text>();
		}
		currentPiece = AllPieces[0];
		StartPieceTurn();
	}


	void StartPieceTurn()
	{
		currentPiece.OnCompleteMove += CurrentPiece_OnCompleteMove;
		if (titleText != null && !gameOver) // keep the game-over title
			titleText.text = currentPiece.PieceName + "'s Turn";
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
			//MessageManager.AddMessage("No predators left!");
		}
		else
		{
			//MessageManager.AddMessage("Nobody won");
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
