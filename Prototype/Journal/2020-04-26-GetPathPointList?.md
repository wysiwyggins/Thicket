---
title: GetPathPointList?
layout: post
date: 2020-04-26 13:19:12
hidden: true
exclude: true
tags: 
    - daily
    - thicket
author: Wiley Wiggins
---
So, I made a lot of progress the other day with the addition of Simple PathFinding 2d... I'm successfully drawing debug paths that conform to my hexgrid. The next step is figuring out how to grab a list of Vector3Ints for grid locations along a path and then turn them to world positions with CellToWorld and then step through them one at a time (each piece has a range var that represents how many spaces they can move each turn).

I see a function in the Simple PathFinding 2d API called GetPathPointList() that should give me a list of Vector3Ints, but I seem to be using it incorrectly:

{% highlight c# %}
void Update()
    {
        if (GameController.CurrentState == GameState.PlayerTurn && isPlayer == true)
        {
            Vector3 position = transform.position;
            Vector3Int cellPosition = grid.WorldToCell(transform.position);
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0.0f;


            
            
            
            if (Input.GetMouseButtonDown(0))
            {

                Vector3Int coordinate = grid.WorldToCell(mouseWorldPos); 
                //get a hex cell coordinate from a mouse click
                Debug.Log("piece position: " + cellPosition);
                Debug.Log("destination: " + coordinate);
                path.CreatePath(position, mouseWorldPos);

            }
            if (path.IsGenerated())
            {
                List<Vector3Int> CellPath = path.GetPathPointList(); 
                //how do we use "pathpoints" from the API?
                //Debug.Log("Path: " + CellPath); 
                // I guess getpathpointlist isn't it? 
                //this says "Path: System.Collections.Generic.List`1[UnityEngine.
                //Vector3Int]"
                //List<Vector3> WorldCoords = grid.CellToWorld(CellPath); 
                //how do I convert a list of vec3ints to a list of vec3's? this isn't 
                //doing it.

                // start a coroutine which animates the object along a path
                //StartCoroutine(FollowPath(WorldCoords));
            }
        }
            
    }
    

    public IEnumerator FollowPath(List<Vector3> WorldCoords)
    {
        //  for Coordinate in WorldCoords {
        //  	transform.position = Coordinate
        //  	wait a sec }
    }
{% endhighlight %}

> Hi Wiley, 

> The GetPathPointList() function returns a list of tile coordinates that make up the path. You can use something like GetPathPointWorld() to get the world position of a particular path point. If you use GetPathPointWorld(n) it will return the world position of the nth tile in that path. 

> Hopefully that helps. If it doesn't let me know! 

> Regards, 

> Tyler