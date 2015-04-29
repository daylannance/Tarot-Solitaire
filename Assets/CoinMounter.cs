using UnityEngine;
using System.Collections.Generic;

public class CoinMounter : MonoBehaviour 
{
	public List<Coin> coins = new List<Coin>();
	public int targetCount;
	public Vector3 offsetPerCoin;
	public Card card;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void AddCoin()
	{

		Coin coin = CoinBank.SpawnCoin(card.zodiac);
		coin.GetComponent<Renderer>().materials[2].SetTexture("_MainTex",Resources.Load <Texture>("Zodiac/" + card.zodiac + "Bump"));
		coin.GetComponent<Renderer>().materials[2].SetTexture("_BumpMap",Resources.Load <Texture>("Zodiac/" + card.zodiac + "Bump"));
		coins.Add (coin);
		coin.transform.parent = transform;
		coin.targetPosition = Vector3.zero;//coins.Count * new Vector3(0,0,coin.renderer.bounds.extents.z);
		coin.transform.localPosition = coin.targetPosition;
		coin.transform.position += new Vector3(0,10,0);
		coin.gameObject.AddComponent<Rigidbody>();
	}
}