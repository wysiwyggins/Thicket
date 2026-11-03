---
title: Field of View pt 2
date: 2020-07-19
layout: post
tags: 
- daily
- thicket
- games
exclude: true
hidden: true
image: fov.png
author: Wiley Wiggins
---

Field of view is working now. There were a couple of issues previously- one was related to a rounding error in [hex line drawing](https://www.redblobgames.com/grids/hexagons/#line-drawing), which James helpfully caught. Another was a missing hex-to-offset coordinate conversion. Now I have very nice looking FOV erasure of fog tiles.
Next steps will be to turn the fog on always and to map the vision range to time of day, so you see further during the day than at night, rather than night happening all at once, also I need to redraw the fog each turn so the fov is turn-by-turn rather than cumulative.
