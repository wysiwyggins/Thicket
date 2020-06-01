using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SceneryManager : MonoBehaviour
{

	// This is a stub for a script that will eventually deal with scenery like water, plants fire etc,
	//haven't thought this out yet but I needed it to show scenerty descriptions with the cursor.


	public static SceneryManager Instance;
	public static List<Scenery> AllSceneries = new List<Scenery>();
	Scenery currentScenery;
	Grid grid;




	public static Scenery GetSceneryAtPos(Vector3Int aCoord)
	{
		foreach (Scenery aScenery in AllSceneries)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aScenery.transform.position);
			if (aCoord == coordinate)
			{

				Debug.Log("found a Scenery");
				return aScenery;
			}
		}
		return null;
	}

	public static Scenery[] GetSceneriesAtPos(Vector3Int aCoord) //seems like this isn't getting any scenery yet?
	{
		List<Scenery> Sceneries = new List<Scenery>();
		foreach (Scenery aScenery in AllSceneries)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aScenery.transform.position);
			if (aCoord == coordinate)
			{
				Debug.Log("found a Scenery");
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

	private void Start()
	{
		grid = GameObject.Find("Grid").GetComponent<Grid>();
	}

}
