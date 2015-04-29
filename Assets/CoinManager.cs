	
using UnityEngine;
using System.Collections;

public class CoinManager : MonoBehaviour {
	public CoinMounter coinMounterPrefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public CoinMounter GetCoinMounter(Card card)
	{
		if(card.mounter) Destroy (card.mounter);
		CoinMounter mounter = Instantiate (coinMounterPrefab) as CoinMounter;
		var pos = transform.position;
		pos.z = card.GetCoinMountZ();
		mounter.transform.position = pos;
		mounter.transform.rotation = transform.rotation;
		mounter.card = card;
		return mounter;
	}
}

