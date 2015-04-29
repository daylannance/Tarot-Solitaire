using UnityEngine;
using System.Collections;

public class ChainLink : MonoBehaviour {
	AudioSource source;
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter2D(Collider2D other)
	{
		source.Play();
	}
	void OnTriggerExit2D(Collider2D other)
	{
		source.Play ();
	}
}
