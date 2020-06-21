using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Scent : MonoBehaviour
{
	public int Drydown;
	public GameObject SpoorPrefab;
	Piece piece;


	// Start is called before the first frame update
	void Start()
	{
		piece = GetComponent<Piece>();
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
		Debug.Log("boop");

		// this would pick up a scent at the end of the turn, and if it matches prey,
		// then it would scan neighbor tiles and set a new target, just like in MoveTowards,
		// except how do we assign a new target so that it doesn't reset the number of moves?
	}
}