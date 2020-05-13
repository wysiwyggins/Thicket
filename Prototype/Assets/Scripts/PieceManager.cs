using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
	public static PieceManager Instance;

    public static List<Piece> AllPieces = new List<Piece>();

	Piece currentPiece;

	public Grid grid;


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
		currentPiece = AllPieces[0];
		StartPieceTurn();
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
			nextIndex = 0;

		currentPiece = AllPieces[nextIndex];
		StartPieceTurn();
	}
	
}
