using UnityEngine;
using System;

public class MouseEvent:EventArgs
{
	public RaycastHit hit;
	public MouseButton button;
	public Vector2 pixelPos;
	public MouseEvent(Vector2 pixelPos, RaycastHit hit, MouseButton button)	
	{
		this.hit = hit;
		this.pixelPos = pixelPos;
		this.button = button;
	}
}
