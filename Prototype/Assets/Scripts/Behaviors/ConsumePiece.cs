using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ConsumePiece : PieceBehaviour
{

	Grid grid;
	Tilemap navmap;
	//private Text TextOutput;
	public int Metabolism;
	const float PreyRemoveDelay = 1f; // seconds the eaten piece stays on the board
	int digestion;

	public enum State
	{
		Hungry,
		Full,
	}
	public static State state;



	//piece attributes
	Vector3Int pieceCoords
	{ get { return grid.WorldToCell(transform.position); } }

	public override void Begin()
	{
		//Debug.Log("ConsumePiece Began, " + piece.PieceName + " strength: " + piece.Strength);
		Piece[] neighbors = PieceManager.GetPiecesAtPos(pieceCoords);


		for (int i = 0; i < neighbors.Length; i++)
		{

			if (piece.CanEat(neighbors[i]))
			{
				Debug.Log("hunting " + neighbors[i].PieceName);
				Consume(neighbors[i]);
			}

		}

		SendCompleteMessage();
	}


	void Awake()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
        navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();
		digestion = Metabolism;
		//TextOutput = GameObject.Find("Text").GetComponent<Text>();

	}

	private void Update()
	{

			
	}

	void Consume(Piece prey)
    {
		Debug.Log("CHOMP: " + prey.PieceName);
		prey.IsBeingRemoved = true;
		// the kill is always seen, even out in the fog: both animals show until the prey is gone,
		// then the predator fades back into the fog
		prey.Reveal(PreyRemoveDelay);
		piece.Reveal(PreyRemoveDelay + 1.5f);
		Destroy(prey.gameObject, PreyRemoveDelay);
		//TextOutput.text += "The " + piece.PieceName + " catches the " + prey.PieceName + ".\n";
		MessageManager.AddMessage("The " + piece.PieceName + " catches the " + prey.PieceName );
		state = State.Full;
		PieceManager.OnPieceRemoved(prey, piece);
	}

	public bool IsFull()
	{
		//check piece manager against nocturnal...
		if (state == State.Full)
        {
			return true;
		}

			

		return false;
	}

	public bool isNapping()
	{
		//tick down the nap timer
		digestion -= 1;
		Debug.Log(piece.PieceName + " digestion: " + digestion); //why is this starting at 0?  it should be 1
		if (digestion < 0)
		{
			state = State.Hungry;
			Debug.Log(state);
			digestion = Metabolism;
			return false;
		}

		return true;

	}

}
