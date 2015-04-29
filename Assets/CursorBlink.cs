using UnityEngine;
using System.Collections;

public class CursorBlink : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine("Blink");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	IEnumerator Blink()
	{
		while(Application.isPlaying)
		{
			yield return new WaitForSeconds(.3f);
			GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
		}
	}
}
