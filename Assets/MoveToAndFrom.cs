using UnityEngine;
using System.Collections;

public class MoveToAndFrom : MonoBehaviour {
	public Vector2 speedA;
	public Vector2 speedB;
	bool isSpeedA = true;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D>().velocity = speedA;
		Debug.Log ("Should be moving now.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
		if(isSpeedA)
		{
			GetComponent<Rigidbody2D>().velocity = speedB;
			isSpeedA = false;
		}
		else
		{
			GetComponent<Rigidbody2D>().velocity = speedA;
			isSpeedA = true;
		}
	}
	void OnTriggerEnter2D(Collider2D collision)
	{
		if(isSpeedA)
		{
			GetComponent<Rigidbody2D>().velocity = speedB;
			isSpeedA = false;
		}
		else
		{
			GetComponent<Rigidbody2D>().velocity = speedA;
			isSpeedA = true;
		}
	}
}
