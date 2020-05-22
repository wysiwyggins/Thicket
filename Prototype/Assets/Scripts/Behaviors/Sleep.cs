using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
	public Sprite AwakeSprite;

	private Text TextOutput;
	private bool sleepcheck;

	public override void Begin()
	{
		sleepcheck = true;
	}

	// Start is called before the first frame update
	void Start()
	{
		TextOutput = GameObject.Find("Text").GetComponent<Text>();
	}

	public bool ShouldSleep()
	{
		//check piece manager against nocturnal...
		if (isNocturnal && PieceManager.state == PieceManager.State.Day)
			return true;
		if (!isNocturnal && PieceManager.state == PieceManager.State.Night)
			return true;

		return false;
	}
	public bool ShouldGoHome()
	{
		//check piece manager against nocturnal...
		if (isNocturnal && PieceManager.state == PieceManager.State.Dawn)
			return true;
		if (!isNocturnal && PieceManager.state == PieceManager.State.Dusk)
			return true;

		return false;
	}

	private void Update()
	{
		while (sleepcheck)
        {
			if (ShouldSleep())
			{
				this.gameObject.GetComponent<SpriteRenderer>().sprite = SleepSprite;
				TextOutput.text += "The " + piece.PieceName + " stops to sleep.";
			}
			else
			{
				this.gameObject.GetComponent<SpriteRenderer>().sprite = AwakeSprite;
				TextOutput.text += "The " + piece.PieceName + "wakes.";
			}
			sleepcheck = false;
			if (ShouldGoHome())
			{

			}
		}
		


		SendCompleteMessage();

	}

}
