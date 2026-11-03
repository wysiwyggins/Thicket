---
title: refactoring
date: 2020-05-14 11:00
layout: post
tags: thicket, daily
exclude: true
hidden: true
image: thicket-eat.gif
author: Wiley Wiggins
---

Ok, refactoring was a very good idea. Nick suggested that we separate a bunch of stuff out from piece into behavior scripts, then get rid of the GameController script, make a pieceManager that is in charge of looping through pieces turns, and then have lists of behaviors on each piece that execute in order. Anyway, now enemy movement and eating pieces both work. 