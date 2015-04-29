using UnityEngine;
using System.Collections;

public class FollowCursorWithPhysics : MonoBehaviour {
	public Camera cam;
	public float speed;
	Rigidbody2D rb2d;
	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	
	void FixedUpdate()
	{
		Vector3 worldPosition = cam.ScreenToWorldPoint(Input.mousePosition);
		Vector3 diff = worldPosition - transform.position;
		rb2d.velocity = new Vector2(diff.x,diff.y) * speed;
	}
}
