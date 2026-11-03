---
title: Why won't you move?
date: 2020-05-08 13:29:04
layout: post
tags: thicket, daily
exclude: true
hidden: true
image: thicket-stuck.gif
author: Wiley Wiggins
---

I've been stuck all week trying to figure out why my AI pieces aren't moving when they seem to be correctly making paths. I'm sure it's probably something easy about the way the [simplepathfinding2d](https://assetstore.unity.com/packages/tools/ai/simple-tile-pathfinding-136493) package I am using works, but I am stumped. 



Here's the current version of my piece.cs script which contains both the player and ai moving logic. the console statement I set up claims the path length is zero even though it's drawn a multi step path to the target. 

![console.log](./images/thicket-path-console.png)

{% highlight c# %}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    
    public delegate void PieceAction();
    public static event PieceAction OnCompleteMove;
    public static event PieceAction InvalidInput;

    //the world
    Grid grid;
    public Tilemap navmap;

    //piece attributes
    Vector3Int cellPosition;
    List<Vector3Int> cellPositions;
    public string pieceName = "unnamed";
    public int strength = 0; // can take pieces with strength under this number
    public int range = 1; //this is the move range
    public bool isPlayer = true;
    public GameObject prey; //what the piece is seeking
    Vector3Int pieceCoords;
    bool validatingMove;
   

    //pathing
    private SimplePF2D.Path path;
    private Rigidbody2D rb;
    private float moveSpeed = 4f; //speed for Moving()
    //Coroutine MoveIE;
    SimplePathFinding2D pf;
    bool following = false;

    // Start is called before the first frame update
    void Start()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>(); //why does any second piece added say this reference isn't set to an instance of an object?
        rb = GetComponent<Rigidbody2D>();
        pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
        PieceManager.AllPieces.Add(this);
        pieceCoords = grid.WorldToCell(transform.position);
        validatingMove = false;
        Debug.Log("Piece: " + pieceName + ", Location: " + pieceCoords);

    }


    private void OnDestroy()
    {
        PieceManager.AllPieces.Remove(this);
    }


    // Update is called once per frame
    void Update()
    {
        
            
        if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true) //it's the players turn, and i'm the player
        {   
            Vector3 location = transform.position;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0.0f;
			
            //how do we highlight the traversable cells within the range of the piece?


            if (Input.GetMouseButtonDown(0) && validPath() == false) //click the mouse
            {

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); //get a hex cell coordinate from a mouse click
                                                                            //Debug.Log("piece position: " + cellPosition);
                Debug.Log("destination: " + coordinate);
                path.CreatePath(location, mouseWorldPos); // generate a path

				Debug.Log("path length " + path.GetPathPointList().Count + "/" + range);
				if (path.GetPathPointList().Count > range)
				{
					path.Reset();
					Debug.Log("resetting path!");
				}

				

			}
			Debug.Log("path length " + path.GetPathPointList().Count);
            if (validPath() && !following) //once there's a path
            {
                cellPositions = path.GetPathPointList();
				validateMove();
			}
		}


        if (GameController.CurrentState == GameState.AITurn && isPlayer == false) //we need to change this to a list of all the ai pieces moving one at a time
        {
            if (prey)
            {
                Vector3 location = transform.position;
                Vector3 preyLocation = prey.transform.position;
                Debug.Log("Piece: " + pieceName + " is hunting: " + prey.name);
                Vector3Int selfCoord = grid.WorldToCell(location);
                Vector3Int preyCoord = grid.WorldToCell(preyLocation);
                Debug.Log("origin: " + selfCoord);
                Debug.Log("destination: " + preyCoord);
                path.CreatePath(location, preyLocation); // generate a path
				Debug.Log(path.GetPathPointList().Count);
                cellPositions = path.GetPathPointList(); 
                Debug.Log("goal is " + cellPositions.Count + " positions away."); // why is this saying it's 0 positions away?
				Debug.Log("following? " + following);
                if (path.IsGenerated() && !following) //once there's a path
                {
                    Debug.Log("There's a path!");

                   validateMove();
                }
            } else
            {
                Debug.Log("Piece: " + pieceName + " doesn't seem to be hunting anything.");
            }
            

        }

    }

	bool validPath()
	{
		return path.IsGenerated() && path.GetPathPointList().Count < range;
	}

    void validateMove()
    {

        validatingMove = true;
        if (cellPositions.Count <= range)
        {
            StartCoroutine(followPath());
        }
        else if (InvalidInput != null)
        {
			Debug.Log("This shouldn't happen!");
            InvalidInput();
            pf.DebugClearPathMarker();
        }
    }

    

    IEnumerator followPath()
    {
        following = true;
        List<Vector3Int> cellPositions = path.GetPathPointList(); // a list of grid positions in the path
        if (cellPositions.Count > range)
        {
            cellPositions.RemoveRange(range + 1, cellPositions.Count); //shave off the moves past piece.range for ai pieces
        }
        for (int i = 0; i < cellPositions.Count; i++) //Loop through them (lists have a "count", not a "length")
        {
            Debug.Log("Get path point");
            Vector3 targetPos = path.GetPathPointWorld(i);

            Vector3 vel = Vector3.zero;
            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, 0.2f, moveSpeed);

                //  transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * moveSpeed);
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("Reached path point");
            pieceCoords = grid.WorldToCell(transform.position);
            Debug.Log("Piece: " + pieceName + ", Location: " + pieceCoords);
            yield return new WaitForSeconds(0.05f);

        }

        if (OnCompleteMove != null)
            OnCompleteMove();
        path = new SimplePF2D.Path(pf);
        following = false;
    }

}

{% endhighlight %}