using UnityEngine;
using System.Collections;

public class CameraBounds : MonoBehaviour {
	public Transform left,right,upper,lower;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void LateUpdate()
	{
		var pos = transform.position;
		pos.x = Mathf.Clamp (pos.x,right.position.x, left.position.x);
		pos.z = Mathf.Clamp (pos.z,upper.position.z, lower.position.z);
		transform.position = pos;
	}
}
