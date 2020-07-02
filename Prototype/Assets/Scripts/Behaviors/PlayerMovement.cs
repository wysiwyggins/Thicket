using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using SimplePF2D;

public class PlayerMovement : PieceBehaviour
{
	//pathing
	private SimplePF2D.Path path;
	private float moveSpeed = 4f; //speed for Moving()

	//fog
	private Tilemap fogTilemap;
	private Tilemap overlayTilemap;

	SimplePathFinding2D pf;

	Grid grid;
	public Tilemap navmap;
	public Tile highlight;

	Vision vision;

	//piece attributes
	Vector3Int pieceCoords
	{ get { return grid.WorldToCell(transform.position); } }
	List<Vector3Int> cellPositions;
	

	public int range;

	
	public UnityEvent OnEnterTile;

	enum State
	{
		Inactive,
		WaitForInput,
		Move,
	}
	State state;

	public override void Begin()
	{
		path.Reset();
		state = State.WaitForInput;
		UpdateFog();
		vision = GetComponent<Vision>();
	}

	void Awake()
    {
        fogTilemap = GameObject.Find("Fog").GetComponent<Tilemap>();
        overlayTilemap = GameObject.Find("Overlays").GetComponent<Tilemap>();
		grid = GameObject.Find("Grid").GetComponent<Grid>(); 
		pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
		path = new SimplePF2D.Path(pf);
		navmap = GameObject.Find("NavigationTilemap").GetComponent<Tilemap>();

		UpdateFog();
	}

	
	void InputState()
	{

		Vector3 location = transform.position;
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mouseWorldPos.z = 0.0f;


		//lets try highlighting the tiles in range
		Vector3Int PiecePosition = overlayTilemap.WorldToCell(transform.position);

		Vector3Int CubeCoords = HexCoordinates.OffsetToCube(PiecePosition); 
		
		int x = CubeCoords.x;
		int y = CubeCoords.y;
		int z = CubeCoords.z;
		// I love constantly fixing range by one
		int adjustedRange = range - 1;
		for (int i = -adjustedRange; i <= adjustedRange; i++)
        {
			for (int j = -adjustedRange; j<= adjustedRange; j++)
            {
				for (int k = -adjustedRange; k <= adjustedRange; k++)
                {
					Vector3Int offsetCoords = HexCoordinates.CubeToOffset(new Vector3Int(x + i, y + j, z + k));
					NavNode node = pf.GetNode(grid.CellToWorld(offsetCoords));
					if ( i + j + k == 0 && !node.IsBlocked())
                    {
						overlayTilemap.SetTile(offsetCoords, highlight);
					}
					
				}

			}
        }

		
		//here's the old square range we did

		//int adjustedRange = range - 1;
		//for (int i =  -adjustedRange; i <= adjustedRange; i++)
		//{
		//	for (int j = (Math.Abs(i % 2)) - adjustedRange; j <= adjustedRange; j++)
		//	{
		//		overlayTilemap.SetTile(PiecePosition + new Vector3Int(j, i, 0), highlight);
		//	}
		//}
		

		if (Input.GetMouseButtonDown(0) && PathIsValid() == false) //click the mouse
		{
			Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
																	 //Debug.Log("piece position: " + cellPosition);
			Debug.Log("destination: " + coordinate);
			path.CreatePath(location, mouseWorldPos); // generate a path

			//let's test our line drawing function!
			//Vector3Int[] linenodes = HexCoordinates.CubeLineDraw(HexCoordinates.OffsetToCube(grid.WorldToCell(mouseWorldPos)), HexCoordinates.OffsetToCube(grid.WorldToCell(location)));
			//foreach (Vector3Int step in linenodes)
   //         {
			//	overlayTilemap.SetTile(HexCoordinates.CubeToOffset(step), highlight);
			//}
			//it worked!

			Debug.Log("path length " + path.GetPathPointList().Count + "/" + range);
			if (path.GetPathPointList().Count > range)
			{
				path.Reset();
				Debug.Log("resetting path!");
			}
		
		}

		if (PathIsValid()) //once there's a path, transition to moving
		{
			cellPositions = path.GetPathPointList();
			StartCoroutine(followPath());
			state = State.Move;
		}
	}


    // Update is called once per frame
    void Update()
    {
		if (state == State.WaitForInput)
			InputState();
		
			
	}

	bool PathIsValid()
	{
		return path.IsGenerated() && path.GetPathPointList().Count <= range;
	}


	IEnumerator followPath()
	{
		List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
		if (cellPositions.Count > range)
		{
			cellPositions.RemoveRange(range, cellPositions.Count); //shave off the moves past piece.range for ai pieces
		}
		for (int i = 0; i < cellPositions.Count; i++) //Loop through them (lists have a "count", not a "length")
		{
			Debug.Log("Get path point");
			Vector3 targetPos = path.GetPathPointWorld(i);

			Vector3 vel = Vector3.zero;
			while (Vector3.Distance(transform.position, targetPos) > 0.01f)
			{
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, 0.2f, moveSpeed);
				UpdateFog();
				//  transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
				yield return new WaitForEndOfFrame();
				
			}
			if(i != 0)
				OnEnterTile.Invoke();
			Debug.Log("Reached path point");
			Debug.Log("Piece: " + piece.PieceName + ", Location: " + pieceCoords);
			yield return new WaitForSeconds(0.05f);

		}

		SendCompleteMessage();
        overlayTilemap.ClearAllTiles();
		path = new SimplePF2D.Path(pf);
	}
    
    void UpdateFog()
    {

		


		Vector3Int PiecePosition = fogTilemap.WorldToCell(transform.position);
		Vector3Int CubeCoords = HexCoordinates.OffsetToCube(PiecePosition);

        // Line of sight fog removal not working right yet
        //if (vision != null)
        //{
        //	Debug.Log("player cubecoords: " + CubeCoords);
        //	vision.UpdateFog();
        //}

		// grabbing a range of hexes in the meantime
        int x = CubeCoords.x;
        int y = CubeCoords.y;
        int z = CubeCoords.z;
        // I love constantly fixing range by one
        int adjustedRange = range - 1;

        for (int i = -adjustedRange; i <= adjustedRange; i++)
        {
            for (int j = -adjustedRange; j <= adjustedRange; j++)
            {
                for (int k = -adjustedRange; k <= adjustedRange; k++)
                {
                    if (i + j + k == 0)
                    {
                        fogTilemap.SetTile(HexCoordinates.CubeToOffset(new Vector3Int(x + i, y + j, z + k)), null);
                    }

                }

            }
        }



        // old square bullshit
        //Vector3Int PiecePosition = fogTilemap.WorldToCell(transform.position);
        //      for(int i = -3; i <= 3; i++)
        //      {
        //          for(int j = -3; j <= 3; j++)
        //          {
        //		//SetTileColour(new Color(0,0,0), new Vector3Int(i, j, 0), fogTilemap); //i was experimenting with trying to change the fog color
        //	    fogTilemap.SetTile(PiecePosition + new Vector3Int(i, j, 0), null);
        //          }
        //      }
    }

    private void SetTileColour(int v, Vector3Int vector3Int, Tilemap fogTilemap)
    {
        throw new NotImplementedException();
    }

    private void SetTileColour(Color colour, Vector3Int position, Tilemap tilemap)
	{
		// Flag the tile, inidicating that it can change colour.
		// By default it's set to "Lock Colour".
		tilemap.SetTileFlags(position, TileFlags.None);

		// Set the colour.
		tilemap.SetColor(position, colour);
	}


}
