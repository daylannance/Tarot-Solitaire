using UnityEngine;
using System.Collections;

public class ScaleWithScreen : MonoBehaviour {
	public float proportionOfScreen;
	public enum Axis { x,y};
	public float aspectRatio;
	public Axis axis;
	// Use this for initialization
	void Start () {
		Proportionalize();
	}
	public void Proportionalize()
	{
		var rect = new Rect();
		switch(axis)
		{
		case Axis.x:
			rect.width = Screen.width * proportionOfScreen;
			rect.height = rect.width * aspectRatio;
			break;
		case Axis.y:
			rect.height = Screen.height * proportionOfScreen;
			rect.width = (1/aspectRatio) * rect.height;
			break;
		}
		GetComponent<GUITexture>().pixelInset = rect;	
	}
	
	// Update is called once per frame
}
