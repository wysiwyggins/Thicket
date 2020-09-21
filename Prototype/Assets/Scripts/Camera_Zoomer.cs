using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Zoomer : MonoBehaviour
{

	public static Camera_Zoomer Instance;
	public float transitionSpeed;
	Camera camera;
	float currentTime;

	float targetSize;

	void Start()
	{
		camera = GetComponent<Camera>();
	}

	void Update()
	{
		currentTime = PieceManager.Instance.sunPerc;
		//Debug.Log("sunPerc:" + PieceManager.Instance.sunPerc);
		//camera.orthographicSize = Mathf.MoveTowards(camera.orthographicSize, currentTime, Time.deltaTime * transitionSpeed); //wanted to zoom into player based on time of night and reduction of visibility, doing it wrong.
	}
}