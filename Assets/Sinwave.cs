using UnityEngine;

public class Sinwave
{
	public float hZ;
	public float amplitude;
	float t;
	public Sinwave(float amplitude, float hZ)
	{
		this.hZ = hZ;
		this.amplitude = amplitude;
	
	}
	public float NextValue()
	{
		t += Time.deltaTime * hZ * 2 * Mathf.PI;
		return Mathf.Cos(t) * amplitude + 1.5f;
	}
}
