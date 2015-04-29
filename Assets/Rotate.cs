using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public Vector3 rotationSpeed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void FixedUpdate()
	{
		transform.Rotate(rotationSpeed);
	}
}
