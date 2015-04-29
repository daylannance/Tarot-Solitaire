using UnityEngine;
using System.Collections;
public enum ScaleType { xFirst, yFirst}
public class PixelScale : MonoBehaviour {
	public ScaleType scaleType;
	public float screenProportion;
	public float yToxRatio;
	GUITexture texture;
	
	// Use this for initialization
	void Start () {
		texture = GetComponent<GUITexture>();
		Rect size = texture.GetScreenRect();	
		switch(scaleType)
		{
		case ScaleType.xFirst:
			size.width = (Screen.width * screenProportion) - size.width;
			size.height = (Screen.width *screenProportion * yToxRatio) - size.height;
			break;
		case ScaleType.yFirst:
			break;
		}
		size.x = -size.width/2;
		size.y = -size.height/2;
		texture.pixelInset = size;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
