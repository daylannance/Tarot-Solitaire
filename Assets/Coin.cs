using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public Vector3 targetPosition;
	public Card card;
	bool onTarget = false;
	// Use this for initialization
	void Start () {
		GetComponent<Collider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(targetPosition == null) return;
		if(transform.position.y < targetPosition.y) 
		{
			transform.localPosition = targetPosition;
			transform.rotation = transform.parent.rotation;
			gameObject.GetComponent<Rigidbody>().useGravity = false;
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			onTarget = true;
		}	
	}
	void FixedUpdate()
	{
				
	}
}