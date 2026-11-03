using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoomer : MonoBehaviour
{

	public static Camera_Zoomer Instance;
	public float transitionSpeed;
	Camera cam;
	float currentTime;

	float targetSize;

	void Start()
	{
		cam = GetComponent<Camera>();
	}

	void Update()
	{
		currentTime = PieceManager.Instance.sunPerc;
		//Debug.Log("sunPerc:" + PieceManager.Instance.sunPerc);
		//cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, currentTime, Time.deltaTime * transitionSpeed); //wanted to zoom into player based on time of night and reduction of visibility, doing it wrong.
	}
}