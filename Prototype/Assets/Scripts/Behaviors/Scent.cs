using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SimplePF2D;private SimplePF2D.Path path;

public class Scent : MonoBehaviour
{
	public int Drydown;
	public GameObject SpoorPrefab;
	Piece piece;
	Grid grid;
	SimplePathFinding2D pf;
	private SimplePF2D.Path path;


	// Start is called before the first frame update
	void Start()
	{
		piece = GetComponent<Piece>();
		grid = GameObject.Find("Grid").GetComponent<Grid>();
	}

	public void Spoor()
	{
		LeaveSpoor(piece.PieceColor);
	}


	void LeaveSpoor(Color color)
	{
		GameObject spoorObject = Instantiate(SpoorPrefab, transform.position, transform.rotation) as GameObject;
		Spoor spoor = spoorObject.GetComponent<Spoor>();
		spoor.drydown = Drydown;
		spoor.color = color;
		spoor.source = piece;

	}

	public void Smell()
    {
		// this would pick up a scent at the end of the turn, and if it matches prey,
		// then it would scan neighbor tiles and set a new target, just like in MoveTowards,
		// except how do we assign a new target so that it doesn't reset the number of moves?
	}
}