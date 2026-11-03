using UnityEngine;
using UnityEngine.Rendering;

// Fades a URP post-processing Volume in and out over the day/night cycle.
[RequireComponent(typeof(Volume))]
public class LightEffect : MonoBehaviour
{
	Volume volume;
	public AnimationCurve weightOverTime;

	void Start()
	{
		volume = GetComponent<Volume>();
	}

	void Update()
	{
		volume.weight = Mathf.MoveTowards(volume.weight, weightOverTime.Evaluate(PieceManager.Instance.sunPerc), Time.deltaTime * 0.2f);
	}
}
