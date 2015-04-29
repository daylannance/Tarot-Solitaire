using UnityEngine;
using System.Collections;
using XBoxController;

public class FollowTrigger : MonoBehaviour {
	public XBoxControlsEnum trigger;
	public float heightMultiplier;
	Vector3 startPosition;
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		float value = XBoxControllerMonoBehavior.controls[trigger].GetValue(true);
		transform.position = startPosition + new Vector3(0,value,0);
	}
}
