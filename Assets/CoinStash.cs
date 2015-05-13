using UnityEngine;
using System.Collections.Generic;

public class CoinStash:MonoBehaviour
{
	public int targetCount;
	public List<Coin> coins;
	public GameObject coinPrefab;
	public Transform coinsTransform;
	public Vector3 coinsTransformRestPos;
	public Vector3 offsetPerCoin;
	void Start()
	{
		coinsTransformRestPos = coinsTransform.localPosition;
	}
	public void AddCoin()
	{
		GameObject coinObj = Instantiate(coinPrefab, coinsTransform.position, Quaternion.identity) as GameObject;
		coinObj.transform.SetParent (coinsTransform);
		var coin = coinObj.GetComponent<Coin>();
		coins.Add (coinObj.GetComponent<Coin>());
		coin.targetLocalPosition = Vector3.zero  + offsetPerCoin * (coins.Count - 1);
		//Dip ();
	}
	public void Dip()
	{
		var pos = coinsTransform.localPosition;
		pos.y -= .1f;
		coinsTransform.localPosition = pos;
	}
	void FixedUpdate()
	{
		EaseUp ();
	}
	void EaseUp()
	{
		var diff = coinsTransformRestPos - coinsTransform.localPosition;
		diff /= 4;
		coinsTransform.localPosition += diff;
	}
}