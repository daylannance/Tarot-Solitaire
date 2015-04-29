using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodingChain : MonoBehaviour {

	public List<GameObject> links;
	// Use this for initialization
	void Start () {
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		foreach(var l in links)
		{
			l.GetComponent<Joint2D>().enabled = false;
		}
	}
}
