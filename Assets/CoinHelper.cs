using UnityEngine;
using System.Collections;

/*	CoinHelper basically is just a static class for spawning coins. 
/*	It has a non-static gameObject for convenience of adjusting position
/*	and easily switching out the coin prefab.
*/
public class CoinHelper : MonoBehaviour {
	public static CoinHelper helper;
	public GameObject coinPrefab;
	// Use this for initialization
	void Start () {
		helper = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//makes coin, sets initial position to the helper Gameobject position:
	public static Coin GetCoin(Card card)
	{
		var obj = Instantiate(helper.coinPrefab) as GameObject;
		obj.transform.position = helper.transform.position;
		var coin = obj.GetComponent<Coin>();
		coin.card = card;
		return coin;	
	}
}
