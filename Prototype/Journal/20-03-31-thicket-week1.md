---
layout: post
title: Thicket week 1
type:
link: 
category: games
hidden: true
exclude: true
tags: 
- daily
- thicket
- games
author: Wiley Wiggins
---

Set up a page and post tag to serve as my process book for a solo game project for school, these are now hidden from the main feed.

I started out with some anxiety about my decision to use a hex grid, and to do a turn-based game, since I am terrible at math and everything I've worked on that involved pathfinding I've asked someone smarter than me to help. I wrote to Adam Saltsman to ask about making turn based games in Unity. I also found some resources for [working with hex grids](https://www.redblobgames.com/grids/hexagons/#angles) and a free [hex grid class](https://assetstore.unity.com/packages/templates/systems/hexgrid-27440#content) on the asset store that I can look at. [Here's another tutorial](https://catlikecoding.com/unity/tutorials/hex-map/part-1/).

This should be fun(?) I haven't worked with Unity in like a year and I was never particularly... advanced in my usage, but I always managed to hack something out and I just have to trust that I will continue to manage to hack things out.

From Adam Saltsman 3/30/20:
>SO, TURN-BASED UNITY GAMES:
>
>there's kind of a lot of ways to skin this cat and a lot of different depths you could go to on different aspects of it but here's some initial thoughts...
>
>i would run as much of your game in / out of coroutines as possible. this is probably true for a real-time game as well but EXTRA TRUE for a turn-based game. you can do things like have a game loop that is a coroutine, and then it can call "yield return EvaluatePlayerMove();" or whatever, right, and then when that's done, start taking player input again right. it was totally life-changing for overland to approach it this way
>
>you can run surprisingly "stupid" path-finding routines SUPER EFFICIENTLY as long as you do your best to like cache results and reuse arrays and lists and stuff (e.g. doing .Clear() on a private variable list instead of always doing "new List<>()" or whatever)
>
>running stuff naively out of the unity hierarchy / monobehaviours will get SUPER CLUNKY REALLY FAST. maybe more so in a turn-based game than in a real-time game, cuz you have to do a lot of evaluation of stuff in sometimes the same frame
>
>i guess all this can be summarized as turn-based games are fucken weird because they spend MOST of their time doing NOTHING, and then every once in a while have to crunch a BUNCH of numbers in one or two frames, and you also have to spend a lot of time like... waiting for moves to evaluate or waiting for players to pick a move. so this is where the motivation comes from for coroutines and trying to avoid allocation in your pathfinding
>
>think as hard as you can about how much UI you really wanna do haha. try to avoid putting all the complexity into your individual units because you will get scaling problems (e.g. if you can have a party of 4, and each unit can be quite complex, thats a Fueckin Hassle)
>
>that's whats on the top of my head rn anyways. if you have any specific questions / concerns hmuuuu
