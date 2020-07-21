﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneryManager : MonoBehaviour
{

	// This is a stub for a script that will eventually deal with scenery like water, plants fire etc,
	//haven't thought this out yet but I needed it to show scenery descriptions with the cursor.


	public static SceneryManager Instance;
	public static List<Scenery> AllScenery = new List<Scenery>();
	Grid grid;

	private void Start()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();

		string sceneryNames = "scenery in order:\n";
		foreach (Scenery aScenery in AllScenery)
		{
			sceneryNames += aScenery.gameObject.name + "\n";
		}
		//Debug.Log(sceneryNames);
	}

	public static Scenery GetSceneryAtPos(Vector3Int aCoord)
	{
		foreach (Scenery aScenery in AllScenery)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aScenery.transform.position);
			if (aCoord == coordinate)
			{
				return aScenery;
			}
		}
		return null;
	}

	public static Scenery[] GetSceneriesAtPos(Vector3Int aCoord)
	{
		List<Scenery> Sceneries = new List<Scenery>();
		foreach (Scenery aScenery in AllScenery)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aScenery.transform.position);
			if (aCoord == coordinate)
			{
				
				Sceneries.Add(aScenery);
			}
		}
		return Sceneries.ToArray();
	}


	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
	}

	

}
