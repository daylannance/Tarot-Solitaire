using UnityEngine;
using System.Collections;

public class CoinBank : MonoBehaviour {
	public Transform spawnTransform;
	public static Transform spawnPoint;
	public Coin coinPrefab;
	public static Coin coinModel;
	// Use this for initialization
	void Start () {
		spawnPoint = spawnTransform;
		coinModel = coinPrefab;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public static Coin SpawnCoin(Zodiac zodiac)
	{
		Coin coin = Instantiate (coinModel) as Coin;
		coin.transform.parent = spawnPoint;
		coin.transform.localPosition = Vector3.zero;
		return coin;
	}
}
