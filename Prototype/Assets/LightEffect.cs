using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class LightEffect : MonoBehaviour
{
	PostProcessVolume volume;
	public AnimationCurve weightOverTime;
	// Start is called before the first frame update
	void Start()
    {
		volume = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
		volume.weight = Mathf.MoveTowards(volume.weight, weightOverTime.Evaluate(PieceManager.Instance.sunPerc), Time.deltaTime * 0.2f);

    }
}
