using UnityEngine;
using System.Collections;

public class CardViewer : MonoBehaviour {
	public Camera cam;
	public Transform cardTransform;
	public Card currentCard;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnGUI()
	{
		if(currentCard != null)
		{
			if(currentCard is ArcanaCard)
			{
				if(GUI.Button(new Rect(Screen.width - 200, Screen.height - 200, 150,40),"Use Card"))
				{

				}
			}
		}
	}
	public void Hide()
	{
		this.enabled = false;
		cam.gameObject.SetActive(false);
		Destroy (currentCard.gameObject);
		//description.text =  "";
		CursorControl.cursor.UseCustomCursor();
	}
	public void Show(Card card)
	{
		this.enabled = true;
		cam.gameObject.SetActive(true);
		//description.text = card.ToString();
		SetCurrentCard (card);	
		CursorControl.cursor.UseSystemCursor();
	}
	public void SetCurrentCard(Card card)
	{
		GameObject obj = Instantiate (card.transform.parent.gameObject) as GameObject;
		Card newCard = obj.GetComponentInChildren<Card>();
		currentCard = newCard;
		newCard.FlipUp();
		obj.transform.position = cardTransform.position;
		obj.transform.rotation = cardTransform.rotation;
		obj.transform.parent = cardTransform;	
	}
}
