using UnityEngine;
using System.Collections;
using XBoxController;

public class WorldCursorControl : MonoBehaviour {
	Vector2 thumbstick;
	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		thumbstick = new Vector2(XBoxControllerMonoBehavior.controls[XBoxControlsEnum.LeftThumb_X].GetValue(true),
		                         XBoxControllerMonoBehavior.controls[XBoxControlsEnum.LeftThumb_Y].GetValue(true));
	}
	void FixedUpdate()
	{
		transform.localPosition += new Vector3(thumbstick.x * speed, 0, thumbstick.y * speed);
	}
}
