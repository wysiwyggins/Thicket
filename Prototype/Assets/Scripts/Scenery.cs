using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenery : MonoBehaviour
{

    public string SceneryName = "unnamed";
    public string Description = "There's nothing here.";
    public bool Cleansing; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SceneryManager.AllScenery.Add(this);
    }

    private void OnDestroy()
    {
        SceneryManager.AllScenery.Remove(this);
    }

}
