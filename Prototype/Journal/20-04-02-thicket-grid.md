---
layout: post
title: Hexgrids
type:
link: 
exclude: true
tags: 
- daily
- thicket
- games
author: Wiley Wiggins
---

![click to move](/images/thicketmove.gif)

![hexgrid](/images/hexgrid.png)

After staring at [this](http://www.redblobgames.com/grids/hexagons/) and [this](http://catlikecoding.com/unity/tutorials/hex-map/part-1/) for a while and feeling kind of overwhelmed trying to make a hex grid from scratch, I found [this](https://assetstore.unity.com/packages/tools/terrain/hexmap-tools-121179) on the asset store, which has example code for making a hexgrid and doing pathfinding on it. Going to try to pick it apart as best I can and use it as a starting point.

![hexmap](/images/hexgrid1.jpg)
Ok, after watching [this](https://www.youtube.com/watch?time_continue=101&v=hZMD5dK4jIE&feature=emb_logo) I found there's already really good hex tilemapping in unity 2d projects (I'll prob still want to take the pathfinding from that asset store package. ) now that I've got this I might make some placeholder terrain tiles. I had to switch the project from 3d to 2d to get the tile palette to show up.

[commit 095d894872e9aba8e713418b4747427f9363135e](https://github.com/wysiwyggins/Thicket/commit/095d894872e9aba8e713418b4747427f9363135e)

![hexmap day 5](/images/thicket4-5.png)

[commit 077bd37694791e2e0a012f5d98c07807c276556f](https://github.com/wysiwyggins/Thicket/commit/077bd37694791e2e0a012f5d98c07807c276556f)

So now what I am looking for is a way to do click to move and pathfinding with the built in unity grid system. There's a GetTile function that I think I can use, I'd like to find an example though. What i'm looking for is:
highlight all the tiles that are within the range of the moving piece
get the hex of the mouse position
listen for a click, change the goal of the moving piece to that hex if it's a valid hex.

I'm still really just in the research phase so far until I figure out how best to do this basic stuff.

![hexmap day 6](/images/2layers.png)
[commit f40f91bec551c9f0051e917836a5285e0b6fd954](https://github.com/wysiwyggins/Thicket/commit/f40f91bec551c9f0051e917836a5285e0b6fd954)

![click to move](/images/thicketmove.gif)
OK! I got basic click-to-move working on the hexgrid!
