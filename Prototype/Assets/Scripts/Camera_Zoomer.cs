using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoomer : MonoBehaviour
{
    public Camera maincamera;
    public static Camera_Zoomer Instance;

    float zoomlevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ZoomCamera(float zoomint, Vector3 playerposition)
    {
        zoomlevel = (float)zoomint /1000;
        maincamera.orthographicSize += zoomlevel;
    }
    public void ResetZoomCamera()
    {
        maincamera.orthographicSize = 6;
    }
}
