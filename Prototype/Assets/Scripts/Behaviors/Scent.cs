using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //set Spoor.drydown to Drydown
        //set color and transparency of spoor??
        //SpoorPrefab.SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //renderer.color = color;
    }

	void Smell()
    {
		//list the smells at a location
    }

}
