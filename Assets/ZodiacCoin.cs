using UnityEngine;
using System.Collections;

public class ZodiacCoin : MonoBehaviour {
	public Card card;
	
	// Use this for initialization
	void Start () {
		
		
	}
	void InitializeTextures()
	{
		GetComponent<Renderer>().material.SetTexture("_BumpMap",Resources.Load<Texture>("CupsHolographic 1"));//Zodiac/" + card.zodiac));
		GetComponent<Renderer>().material.SetTexture("_ParallaxMap",Resources.Load<Texture>("CupsHolographic"));//"Zodiac/" + card.zodiac));
	}	
	// Update is called once per frame
	void Update () {
	
	}
}
