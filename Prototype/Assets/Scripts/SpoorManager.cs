using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpoorManager : MonoBehaviour
{


	public static SpoorManager Instance;
	public static List<Spoor> AllSpoor = new List<Spoor>();
	Grid grid;

	public static Spoor[] GetSpoorAtPos(Vector3Int aCoord)
	{
		List<Spoor> Spoors = new List<Spoor>();
		foreach (Spoor aSpoor in AllSpoor)
		{
			Vector3Int coordinate = Instance.grid.WorldToCell(aSpoor.transform.position);
			if (aCoord == coordinate)
			{

				Spoors.Add(aSpoor);
			}
		}
		return Spoors.ToArray();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
