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

	private Tilemap fogTilemap;
	public Tile fogTile;

	//private Text TextOutput;

	public int hour;

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

				Debug.Log("found a piece");
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
		fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
		//TextOutput = GameObject.Find("Text").GetComponent<Text>();


		string pieceNames = "pieces in order:\n";

		foreach(Piece aPiece in AllPieces)
		{
			pieceNames += aPiece.gameObject.name + "\n";
		}
		//Debug.Log(pieceNames);
	}

	public List<Vector3Int> TilesInRange(Vector3Int pos, int range)
	{
		List<Vector3Int> tiles = new List<Vector3Int>();

		//..populate list


		return tiles;
	}

	void StartPieceTurn()
	{
		currentPiece.OnCompleteMove += CurrentPiece_OnCompleteMove;
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


		currentPiece = AllPieces[nextIndex];
		StartPieceTurn();
		


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
			//TextOutput.text += "The sun is swallowed up by the dark horizon of the earth.\n";
			MessageManager.AddMessage("The sun is swallowed up by the dark horizon of the earth.");
			foreach (var position in fogTilemap.cellBounds.allPositionsWithin)
			{
				fogTilemap.SetTile(position, fogTile);
			}
		}
		if (hour >= 13)
		{
			state = State.Dawn;
			//TextOutput.text += "The air around you sings to life. The red sun wakes over the land.\n";
			MessageManager.AddMessage("The air around you sings to life. The red sun wakes over the land.");
			fogTilemap.ClearAllTiles();
			hour = 0;
		}
	}
}
