using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spoor : MonoBehaviour
{

	int drydown_max = 10;
	public int drydown;
	public Color color;
	public Piece source;

	// Order in which spoor was laid (higher = fresher). Drydown can't tell apart spoor left
	// during the same move, so trail-following uses this to know which way the trail runs.
	public int order;
	static int nextOrder;
	bool fading; // dried up or washed away; waiting for its delayed Destroy
	SpriteRenderer spriteRenderer;
	Grid grid;


	void Awake()
	{
		order = nextOrder++;
	}

	private void OnEnable()
	{
		PieceManager.OnRoundComplete += CheckSpoor;
		SpoorManager.AllSpoor.Add(this); // so animals can smell it (this was missing, so trails never worked)
	}

	private void OnDisable()
	{
		PieceManager.OnRoundComplete -= CheckSpoor;
		SpoorManager.AllSpoor.Remove(this);
	}

	// Stop being smellable now; the sprite lingers for a second before it's destroyed.
	void Fade()
	{
		if (fading) return;
		fading = true;
		SpoorManager.AllSpoor.Remove(this);
		Destroy(this.gameObject, 1);
	}


	// A Spoor is the object left each turn by the Scent behavior
	// Scent has an int called Drydown it gives to each Spoor, which is the number of turns a spoor takes to dissapear.
	// Spoor would get its color from Piece.color (and the opacity would derive from drydown as it ticks to zero)
	// animals with a prey will follow a trail of Spoor, moving from low to high Spoor.drydown
	// Start is called before the first frame update
	void Start()
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		SetColor();
		grid = GameObject.Find("Grid").GetComponent<Grid>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }


	void SetColor()
	{
		Color c = color;
		c.a = drydown / (float) drydown_max;
		spriteRenderer.color = c;
	}

    void CheckSpoor()
    {
        //tick down the drydown
        drydown -= 1;
		SetColor();
		Vector3Int coordinate = grid.WorldToCell(transform.position);

		Scenery[] sceneries = SceneryManager.GetSceneriesAtPos(coordinate);
		if (sceneries != null)
		{
			foreach (Scenery scenery in sceneries)
			{
				if(scenery.Cleansing)
                {
					if (!fading && source != null)
						MessageManager.AddMessage("The "+ scenery.SceneryName + " removes the scent of the " + source.PieceName);
					Fade();
				}
			}

		}
		if (drydown < 0)
        {
            Fade();
        }

    }
}
