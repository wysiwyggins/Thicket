---
title: refactoring
date: 2020-05-13 11:00
layout: post
tags: thicket, daily
exclude: true
hidden: true
author: Wiley Wiggins
---

The suggestion for my AI movement woes is to refactor and create a player script and an ai script that both inherit from piece. This kind of depresses me a little but I'll try it. 

my original conception of this was that each piece could eventually have a little player surfaced control panel where you could stack simple rules, and player control was simply a rule- basically all pieces are the same, player control was just a rule that could be added. If this doesn't make sense, imaginge emoji simulator: [https://ncase.me/sim/](https://ncase.me/sim/) but with one possible rule that you can add that stops the simulation and listens for a player movement command. There's not really 'types' of emoji in emoji simulator, they're just empty objects with a bunch of rules, and a sprite image and its all interchangable. 

All of this is totally academic though if I can't get a piece to execute a move on its own though. I've started trying to separate the player and ai logic into two different scripts and I'm already making a big mess and creating a bunch of duplicated code, which I hate... I hope that one day I will have enough mastery of Unity to be able to make good design decisions on the fly instead of just flailing until it works. I just have to remind myself that at one point long ago I was like this with CSS and now I think fluidly with it and can design well and refactor on the fly. 