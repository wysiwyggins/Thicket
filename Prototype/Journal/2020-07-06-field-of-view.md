---
title: Field of View
date: 2020-07-06
layout: post
tags: 
- daily
- thicket
- games
exclude: true
hidden: true
author: Wiley Wiggins
---

I haven't posted about Thicket lately because I'm in a 6 week long compressed summer statistics course which is kind of kicking my ass. I am still consistently working on Thicket though. Right now I am working on [field of view](https://www.redblobgames.com/grids/hexagons/#field-of-view) for fog of war. This consists of a couple of functions that I ported from the Red Blobs site with the help of James Curry. Field of View uses a rangefinding function and then uses a line drawing function to draw a line to every tile that range finds. If the line hits an obstacle then we dump the hexes from that line, if not, we add them to the array. Then I clear that array of hexes from the Fog tilemap by adding a null tile to them.
Right now the range function works and the line drawing function mmmmmostly works? (It draws some slightly irregular lines sometimes and I'm not sure why) But the whole field of view function is choking for some reason on the method I use to check for obstacles, which are a type of object I call scenery which can have a bool called "opaque", instead of using the pathfinding tilemap which is a whole different thing. Now why this scenery check is causing a problem, I can't yet determine. I'm using it in the Spoor script (spoor is the scent object that animals leave behind) to check for water tiles, and it works fine there, and uses the same kind of coordinate (offset) input. for some reason in my field of view function it hangs everything up, however, with no error. I miss having a class instructor to bounce problems like this off of- James doesn't know/have Unity so he's best to ask about things like the psuedocode on the Redblobs hexgrid site, not stuff deep in my unity project like this.
