using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraBehavior : MonoBehaviour
{

	public Color color1 = Color.red;
	public Color color2 = Color.blue;
	public float duration = 20.0F;

	Camera cam;

	// Use this for initialization
	void Start()
	{
		cam = GetComponent<Camera>();
		cam.clearFlags = CameraClearFlags.SolidColor;
	}

	// Update is called once per frame
	void Update()
	{
		float t = Mathf.PingPong(Time.time, duration) / duration;
		cam.backgroundColor = Color.Lerp(color1, color2, t);
	}
}
