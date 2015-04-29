using UnityEngine;
using System.Collections;

public static class CameraUtility
{
	public static float GetFrustumHeigtAtDistance(float distance, Camera cam)
	{
		var frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		return frustumHeight;	
	}
	public static float GetDistanceOfFrustumHeight(float frustumHeight, Camera cam)
	{
		var distance = frustumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		return distance;
	}
}
