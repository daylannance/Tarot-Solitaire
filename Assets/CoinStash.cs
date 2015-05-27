using UnityEngine;
using System.Collections.Generic;

public class CoinStash:MonoBehaviour
{
	public int targetCount;
	public Stack<Coin> coins = new Stack<Coin>();
	public GameObject coinPrefab;
	public Transform coinsTransform;
	public Vector3 coinsTransformRestPos;
	public Vector3 offsetPerCoin;
	Vector3 restPosition;
	void Start()
	{
		coinsTransformRestPos = coinsTransform.localPosition;
		restPosition = transform.position;
	}
	public void AddCoin()
	{
		GameObject coinObj = Instantiate(coinPrefab, coinsTransform.position, Quaternion.identity) as GameObject;
		coinObj.transform.SetParent (coinsTransform);
		var coin = coinObj.GetComponent<Coin>();
		coins.Push (coinObj.GetComponent<Coin>());
		coin.targetLocalPosition = Vector3.zero  + offsetPerCoin * (coins.Count - 1);
		//set coin at a height above so it can fall:
		var pos = coin.targetLocalPosition;
		pos.z += 15;
		coin.transform.localPosition = pos;
		//coin needs reference to parent stash to make it dip:
		coin.parentStash = this;
	}
	public void RemoveCoin()
	{
		coins.Pop().StartCoroutine("LoseCoinCoroutine");	
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