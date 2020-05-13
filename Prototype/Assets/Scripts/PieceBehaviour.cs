using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
	public delegate void PieceAction();
	public event PieceAction OnCompleteMove;
	public Piece piece
	{
		get
		{
			if (_piece == null)
				_piece = GetComponent<Piece>();
			return _piece;
		}
	}

	Piece _piece;

	public virtual void Begin()
	{

	}

	protected void SendCompleteMessage()
	{
		if (OnCompleteMove != null)
			OnCompleteMove();
	}
}
