using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastChangeMaterial : MonoBehaviour {
	public GameObject targetPracticeSprite;
	Dictionary<string,Sprite> sprites = new Dictionary<string,Sprite>();
	GameObject cursor;
	GameObject currentSprite;
	// Use this for initialization
	void Start () {
		var allsprites = Resources.LoadAll<Sprite>("Textures/Fonts/Alphabet");
		for(int i = 0; i < allsprites.Length; i++)
		{
			sprites.Add (allsprites[i].name, allsprites[i]);
		}
		cursor = Instantiate (Resources.Load<GameObject>("Cursor")) as GameObject;
		cursor.GetComponentInChildren<Renderer>().enabled = false;
		currentSprite = cursor;

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown (0))
		{
			PlaceCursor();
		}
		HandleTyping ();
	}
	void HandleTyping()
	{
		var str = Input.inputString;
		if(string.IsNullOrEmpty(str)) return;
		if(sprites.ContainsKey(str)) 
		{
			GameObject obj = Instantiate (targetPracticeSprite) as GameObject;
			var renderer = obj.GetComponentInChildren<SpriteRenderer>();
		    renderer.sprite = sprites[str];
			Vector3 pos = currentSprite.transform.position;
			obj.transform.rotation = currentSprite.transform.rotation;
			pos += currentSprite.transform.right * currentSprite.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x/2;
			pos += obj.transform.right * obj.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.x/2;
			obj.transform.position = pos;
			currentSprite = obj;
		}	
	}
	void PlaceCursor()
	{
		RaycastHit hit;
		Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray,out hit,1000f, LayerMask.NameToLayer("All") ))
		{
			Debug.Log ("hit! Normal = " + hit.normal + " Point = " + hit.point);
			cursor.transform.position = hit.point;	
			cursor.transform.LookAt(cursor.transform.position +  hit.normal);
			cursor.transform.localRotation *= Quaternion.AngleAxis (180,new Vector3(0,1f,0));
			cursor.transform.position += new Vector3(0,.1f,0);
			cursor.GetComponentInChildren<Renderer>().enabled = true;
			currentSprite = cursor;
		}//if(Physics.Raycast(transform.position, transform.forward,))
	}
	void PlaceCharacter(Sprite sprite)
	{
		RaycastHit hit;
		Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray,out hit,1000f, LayerMask.NameToLayer("All") ))
		{
			Debug.Log ("hit! Normal = " + hit.normal + " Point = " + hit.point);
			GameObject obj = Instantiate(Resources.Load<GameObject>("Cursor"),hit.point, Quaternion.identity) as GameObject;
			obj.transform.LookAt(obj.transform.position +  hit.normal);
			obj.transform.position += new Vector3(0,.1f,0);
			
		}//if(Physics.Raycast(transform.position, transform.forward,))	
	}
}
