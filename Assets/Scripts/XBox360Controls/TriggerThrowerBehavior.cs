using UnityEngine;
using System.Collections;
using XBoxController;

public class TriggerThrowerBehavior : MonoBehaviour {
	public float strengthFactor;
	public XBoxControlsEnum control;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float value = XBoxControllerMonoBehavior.controls[control].GetMovementValue(true) * strengthFactor;
		if(value > 0)
		{
			value = value * value * value;
			GetComponent<Rigidbody>().AddForce(new Vector3(0,value,0));
		}
	}
}
