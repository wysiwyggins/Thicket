---
title: maze creation
date: 2020-10-23 14:45:53
layout: post
categories: 
tags: 
- daily
- thicket
- games
exclude: true
hidden: true
image: mapbuilder.png
author: Wiley Wiggins
---

It's been a hairy quarter so far and I haven't had a lot of time to look at Thicket, but I have been playing with maze generation a little bit. Alex Rickett sketched out a possible implementation of Primm's Maze and I'm experimenting with parts of it. I'm drawing a hex-shaped island of ground based on a given radius int, then trying to draw a boundary around it (shown by these debug red dots) and then adding bramble scenery which are both opaque and add obstacle tiles (red dots). Right now the ring around the map is slightly too big and the brambles are drawing off the edge of the map, so it's probably problems related to coordinate conversions. Also I'm not sure if Alex's node-based solution is a good solution for me. James Curry also experimented with a grid-based version (all in php!) worth looking at [here](https://miscellany.3pb.net/mazegen/)
