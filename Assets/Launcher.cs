using UnityEngine;
using System.Collections;

public class Launcher : MonoBehaviour {

	public GameObject missilePrefab;
	public Transform missileSpawn;
	GameObject missileObj;
	public float bulletSpeed;
	
	void Start()
	{
		LoadMissile();
	}

	void LoadMissile()
	{
		missileObj = Instantiate (missilePrefab) as GameObject;
		missileObj.transform.parent = missileSpawn;
		missileObj.transform.localPosition = Vector3.zero;
		missileObj.transform.rotation = missileSpawn.rotation;
	}

	void Update()
	{
		if(Input.GetKeyDown (KeyCode.Space))
		{
			Fire ();
		}
	}
	
	void Fire()
	{
		Rigidbody2D body = missileObj.AddComponent<Rigidbody2D>();
		body.gravityScale = 0;
		missileObj.transform.parent = null;
		body.velocity = -missileSpawn.right * bulletSpeed;
		LoadMissile();
	}
	void FixedUpdate()
	{
		transform.Rotate (new Vector3(0,0,1),1);
	}
}