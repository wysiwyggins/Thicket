using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConsumePiece : PieceBehaviour
{

	Grid grid;
	Tilemap navmap;

	//piece attributes
	Vector3Int pieceCoords
	{ get { return grid.WorldToCell(transform.position); } }

	public int strength;

	enum State
	{
		Fasting,
		Consuming,
	}
	State state;

	public override void Begin()
	{
		state = State.Fasting;
	}

	void Start()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
        navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

	}

	private void Update()
	{
		Piece[] neighbors = PieceManager.GetPiecesAtPos(pieceCoords);

		for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i].strength < strength) {
				if (state == State.Consuming)
					Consume(neighbors[i]);
			}

		}

			
	}

	void Consume(Piece prey)
    {
		Debug.Log("CHOMP");
		Destroy(prey, 5);
		SendCompleteMessage();
	}

}
