using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scent : PieceBehaviour
{
    public int Drydown;
    public Transform SpoorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LeaveSpoor(piece.PieceColor);
    }

    void LeaveSpoor(Color color)
    {
        Instantiate(SpoorPrefab, transform.position, transform.rotation);
        //set Spoor.drydown to Drydown
        //set color and transparency of spoor??
        //SpoorPrefab.SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        //renderer.color = color;
    }

}
