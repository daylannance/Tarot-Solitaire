using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ZodiacSigns : MonoBehaviour {
	List<ScaleWithScreen> signs = new List<ScaleWithScreen>();
	Dictionary<string,ScaleWithScreen> symbols = new Dictionary<string, ScaleWithScreen>();
	// Use this for initialization
	void Start () {
		List<string> signNames = new List<string>()
			{ "Aries", "Taurus", "Gemini", "Cancer", "Leo", "Virgo", "Libra", "Scorpio",
			"Sagittarius", "Capricorn", "Aquarius", "Pisces"};
		GameObject sample = new GameObject(); 
		foreach(string str in signNames)
		{
			GameObject obj = Instantiate(sample) as GameObject;
			var texture = obj.AddComponent<GUITexture>();
			texture.texture = Resources.Load<Texture>("Zodiac/" + str);
			var sign = obj.AddComponent<ScaleWithScreen>();
			signs.Add (sign);	
		}
		Destroy(sample);
		for(int i = 0; i < signs.Count; i++)
		{
			var sign = signs[i];
			sign.proportionOfScreen = 1.0f/12.0f;
			sign.axis = ScaleWithScreen.Axis.y;
			sign.aspectRatio = 1;
			sign.Proportionalize();
			sign.transform.position = new Vector3(1f- 1f/12f,1 -((1f/12f) * (i+1)),0);
			sign.transform.parent = transform;
			sign.transform.localScale = new Vector3(.001f,.001f,.001f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
