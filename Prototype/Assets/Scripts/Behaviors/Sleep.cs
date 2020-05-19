using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sleep : PieceBehaviour
{

	// add this script for pieces that sleep
    // isNocturnal determines if the piece sleeps at night or day
    // sleep sprite displays when asleep
    // home tile is the tile the piece tries to return to during dawn or dusk depending on isNocturnal
    // we need to get the time of day from PieceManager


	public bool isNocturnal;
	public Tilemap Terrain;
	public Tile HomeTile;
	public Sprite SleepSprite;

	public override void Begin()
	{

	}

	// Start is called before the first frame update
	void Start()
	{


	}

	private void Update()
	{
    // this shouldn't be in update and it's not getting the piecemanager state correclty
    // if (isNocturnal && PieceManager.state == night)
	// {
	//      this.gameObject.GetComponent<SpriteRenderer>().sprite = SleepSprite;
	// }
	}

}
