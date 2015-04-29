using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bounds : MonoBehaviour {
	public Transform Front, Back, Left, Right, Top, Bottom;
	public List<GameObject> targets;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void LateUpdate()
	{
		foreach(var obj in targets )
		{
			var pos = obj.transform.position;
			pos.x = Mathf.Clamp (pos.x, Right.position.x, Left.position.x);
			pos.y = Mathf.Clamp (pos.y, Bottom.position.y, Top.position.y);
			pos.z = Mathf.Clamp (pos.z,	Front.position.z, Back.position.z);
			obj.transform.position = pos;
		}
	}
	
}
