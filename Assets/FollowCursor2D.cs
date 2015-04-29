using UnityEngine;
using System.Collections;

public class FollowCursor2D : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnGUI()
	{
		Game.game.currentCam.enabled = true;
		transform.position = Game.game.currentCam.ScreenToViewportPoint(Input.mousePosition);
	}
}
