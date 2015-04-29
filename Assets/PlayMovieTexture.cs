using UnityEngine;
using System.Collections;

public class PlayMovieTexture : MonoBehaviour {
	 MovieTexture movTexture;
	// Use this for initialization
	void Start () {
		
		movTexture = (MovieTexture)GetComponent<Renderer>().materials[1].GetTexture("_MainTex");
		movTexture.loop = true;
		movTexture.Play();
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
