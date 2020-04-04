using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isWater = false;
    public bool isDark = false;
    public bool isSolid = false;
    public bool isFire = false;
    public int x = 0;
    public int y = 0;
    public int z = 0;
    //should scent be a quality or another object that is a child of tile?
    //   can there be more than one scent or does a new scent replace an old scent <- probably

    // Start is called before the first frame update
    void Start()
    {
        //place the tile
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
