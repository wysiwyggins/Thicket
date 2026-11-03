---
title: thicket revival, unity 6
date: 2026-11-02
hidden: true
exclude: true
tags: 
    - daily
    - thicket
author: Wiley Wiggins
---

Because Thicket was the first ambitious Unity project I ever attempted as an undergraduate, it has a large number of fairly candid journal entries where I am struggling with basic Unity usage while getting help from an instructor. I decided to update is to URP/Unity 6 and add the journal entries in case the project is useful to the Materializing Design Course I'm currently in.

**Upgrade**

- Moved the project from 2020.2 to Unity 6. Switched to URP with the 2D renderer and rebuilt the day/night post-processing as URP volumes (PostFX-Day and PostFX-Night).
- All text is TextMeshPro now. Input handling is set to "Both", so the old mouse code still works and the new Input System is there when I want it.
- Removed packages I wasn't using (Ads, IAP, Analytics, old Post-Processing, etc.).
- Git cleanup: thousands of `Library/` files had been committed before the gitignore existed. They're untracked now, and the work lives on the `unity6-upgrade` branch.
- Fixed a few old bugs: an error when the scene unloads, a race at startup with the message log, and some movement code that could freeze a turn.

**Fog of war**

- Animals out of view get tinted to the fog colour. One hex past the edge of what you can see, you just get a shadowy head poking into the visible hexes; further out they're gone. Brambles work the same way.
- When something gets eaten, both animals flash into view even in the fog, so you see the kill happen.

**Turns**

- The cursor only shows on the deer's turn, and the Title text says whose turn it is.
- If the wolf gets taken out (the bear can eat it), the deer is safe and R restarts the game.

**New animal behaviour**

- Chickens can be pushed by the deer and don't share hexes with other animals. Other animals path around them, except the wolf and bear, which can still catch them.
- Rabbits and a turtle are in the game now. Rabbits move two hexes like the wolf. The turtle is slow, only the bear can eat it, and it chews through brambles, which opens new paths through the maze.
- Scent trails are followable now
- Animals flee from stronger animals

**Noticed for later**

- Nothing happens when the deer gets caught. It needs a lose state.
- Sleep and fullness are shared between all animals (static), so every wolf naps and gets full together.
