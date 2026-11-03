---
layout: post
title: tiles and pieces
type:
link: 
hidden: true
exclude: true
tags: 
    - daily
    - thicket
author: Wiley Wiggins
---

I played with those hexgrid helper classes for a while and then put them aside. I've decided to start from scratch and then return to them later. Right now I want to just conceptualize the most basic objects in the game, and if there's something I don't know how to do immediately I'll pseudocode it. 

Right off the bat, I know that there are *tiles* and *pieces*.


# tiles:

{% highlight c# %}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
    {
        public string tileName = "unamed";
        public GameObject tileTexture = null;
        public bool isWater = false;
        public bool isDark = false;
        public bool isSolid = false;
        public bool isFire = false;
        //should scent be a quality or another object that is a child of tile?
        //   can there be more than one scent or does a new scent replace an old scent <- probably

        // Start is called before the first frame update
        void Start()
        {
            //place the tile
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
{% endhighlight %}

So pieces in general would have these qualities and then I'd have 'types' of pieces that are just combinations of those qualities- like forest would have isDark = true and isSolid = false and then it would also have an associated peice of artwork. 


# Pieces: 
{% highlight c# %}
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class Piece : MonoBehaviour
    {
        public delegate void PieceAction();
        public static event PieceAction OnCompleteMove;

        public string pieceName = "unnamed"; //this gets overridden with the name of the piece
        public Vector3Int cellPosition;  //grid position
        public int strength = 0; // can take pieces with strength under this number
        public int range = 1; //this is the move range
        public bool isPlayer = true;
        public GameObject sprite;
        public GameObject prey; //what the piece is seeking
        Grid grid;
        

        // Start is called before the first frame update
        void Start()
        {
            grid = GameController.instance.grid;
            // snap all pieces to their grid
            Vector3Int cellPosition = grid.WorldToCell(transform.position);
            transform.position = grid.CellToWorld(cellPosition);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //get a hex cell from a mouse click
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
                Debug.Log(coordinate);
                if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true)
                {
                    MoveTo(coordinate);
                    
                
                }
            }
        }


        public void MoveTo(Vector3Int targetCell)
        {
            transform.position = grid.GetCellCenterWorld(targetCell);
            // List<Vector3Int> CellPath = AStarPathFunction(targetCell);

            // List<Vector3> WorldCoords = CellToWorld(CellPath);

            // start a coroutine which animates the object along a path
            // StartCoroutine(FollowPath(aPath));
        }


    }

{% endhighlight %}

likewise there would be animal types that would inherit from Piece and also have associated art.

I know this is the most obvious thing in the world but it helps me to think it out bit-by-bit.

*This is a stub! I will keep adding to this.*