---
title: Offset vs Cube
date: 2020-05-24 11:00
layout: post
tags: thicket, daily
exclude: true
hidden: true
author: Wiley Wiggins
---

So There's a few ways of adressing a hex on a hexgrid. One is regular square grid style co-ordinates (just offset), and another is Vector3 cube co-ordinates- For a better description and visual aids, check out [this page on red blob games](https://www.redblobgames.com/grids/hexagons/#coordinates). 

So up until now I've been addressing the hexes with regular offset x,y coordinates (well, they're Vector3's but the z is just always set to zero, since tilemaps have a layering number so I don't have to mess with z space.) but because of the offset, I can't just use the kinds of functions I would on a square grid to do things like get grid spaces within a range. I'm basically still getting a square-like irregular shape on the hex grid when I want to be getting a circle. So I need to convert to three-vector cube co-ordinates. This (hopefully) will get me a lot simpler functions to get ranges and do other cool stuff like line-of sight math for fog of war. 

Catlike coding has an offset to [cube conversion tutorial](https://catlikecoding.com/unity/tutorials/hex-map/part-1/), and I'm working on that now. I'm getting a little antsy because this is like almost week 9 of a 10 week quarter and I need to get this experiment in a place where if it's not finished as a game at least it's capped off in a way that I can sum it up. I've learned so much about programming in Unity on this project, but there's still this vast gulf of stuff I don't know, and I have a lot of really weak spots as a beginning programmer. Progress not perfection. 