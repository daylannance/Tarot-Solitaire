
using UnityEngine;
using System.Collections;

public class SuitFoil : MonoBehaviour {
	public Card card;
	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().material = Resources.Load<Material>("Suits/Materials/" + card.suit + "FoilMat");
	}
	// Update is called once per frame
	void Update () {
	
	}
	void SetTextures()
	{
		GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture2D>("Suits/" + card.suit + "Smooth"));
		GetComponent<Renderer>().material.SetTexture("_BumpMap", Resources.Load<Texture2D>("Suits/" + card.suit + "Normal"));
		GetComponent<Renderer>().material.SetTexture("_HeightMap", Resources.Load<Texture2D>("Suits/" + card.suit + "Smooth"));
	}
}