using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public Vector3 targetLocalPosition;
	public Card card;
	bool onTarget = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.localPosition = targetLocalPosition;
		transform.rotation = transform.parent.rotation;
		onTarget = true;	
	}
	void FixedUpdate()
	{
				
	}
}