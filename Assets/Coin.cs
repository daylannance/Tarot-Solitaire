using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
	public Vector3 targetLocalPosition;
	public Card card;
	public CoinStash parentStash;
	bool onTarget = false;
	Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = gameObject.AddComponent<Rigidbody>();	
		transform.rotation = transform.parent.rotation;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//check if the coin has fallen past the target
		if(transform.localPosition.z < targetLocalPosition.z)
		{
			//disable rigid body, just needed it for falling:
			Destroy (rb);
			//put at target position on the coinStash:
			transform.localPosition = targetLocalPosition;
			//animate the coin stash as if it has been hit:
			parentStash.Dip ();
		}
		//
		onTarget = true;	
	}
	void FixedUpdate()
	{
				
	}
	public IEnumerator LoseCoinCoroutine()
	{
		var minForce = 15f;
		var maxForce = 30f;
		var minTorque = 100f;
		var maxTorque = 200f;
		var body = gameObject.AddComponent<Rigidbody>();
		transform.SetParent (null,true);
		body.AddForce(Random.Range (-maxForce ,maxForce),
					Random.Range (minForce,maxForce),
					Random.Range (-maxForce,minForce),ForceMode.Impulse);
		body.AddTorque(Random.Range(minTorque,maxTorque),
						Random.Range (minTorque,maxTorque),
						Random.Range (minTorque,maxTorque));
		yield return new WaitForSeconds(.7f);
		Destroy (gameObject);
	}
}