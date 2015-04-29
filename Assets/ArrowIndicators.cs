using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowIndicators : MonoBehaviour {
	public List<SpriteRenderer> arrows;
	Color normalColor;
	// Use this for initialization
	void Awake()
	{
		normalColor = arrows[0].color;
	}
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void ForwardArrows()
	{
		transform.localRotation = Quaternion.AngleAxis (0, new Vector3(0,0,1));
		foreach(SpriteRenderer arrow in arrows)
		{
			arrow.color = normalColor;
		}
	} 
	public void ReverseArrows()
	{
		transform.localRotation = Quaternion.AngleAxis (180, new Vector3(0,0,1));
		StartCoroutine(ReverseArrowRoutine());
	}
	IEnumerator ReverseArrowRoutine()
	{
		
		foreach(var sprite in arrows)
		{
			sprite.color = Color.red;
			yield return new WaitForSeconds(0.02f);
			sprite.color = Color.blue;
		}
		for(int i = 0; i < 10; i++)
		{
			foreach(var sprite in arrows)
			{
				sprite.color = Color.blue;
			}
			yield return new WaitForSeconds(0.01f);
			foreach(var sprite in arrows)
			{
				sprite.color = Color.red;
			}
			yield return new WaitForSeconds(0.01f);
		}
	}
}
