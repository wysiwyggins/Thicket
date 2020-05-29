﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class ConsumePiece : PieceBehaviour
{

	Grid grid;
	Tilemap navmap;
	private Text TextOutput;
	public int Metabilism;
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
		Debug.Log("ConsumePiece Began, " + piece.PieceName + " strength: " + piece.Strength);
		Piece[] neighbors = PieceManager.GetPiecesAtPos(pieceCoords);

		for (int i = 0; i < neighbors.Length; i++)
		{

			if (neighbors[i].Strength < piece.Strength)
			{
				Debug.Log("hunting " + neighbors[i].PieceName);
				Consume(neighbors[i]);
			}

		}

		SendCompleteMessage();
	}


	void Start()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
        navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();
		TextOutput = GameObject.Find("Text").GetComponent<Text>();

	}

	private void Update()
	{
		

			
	}

	void Consume(Piece prey)
    {
		Debug.Log("CHOMP: " + prey.PieceName);
		Destroy(prey.gameObject, 1);
		TextOutput.text += "The " + piece.PieceName + " catches the " + prey.PieceName + ".\n";
		state = State.Full;
	}

}
