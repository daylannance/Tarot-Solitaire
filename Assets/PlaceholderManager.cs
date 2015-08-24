using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlaceholderManager : MonoBehaviour {
	public static PlaceholderManager manager;
	public GameObject placeholderPrefab;
	public GameObject majorArcanaPlaceholderPrefab;
	public Deck deck;
	public Deck majorsDeck;
	public Hand hand;
	public AcePile wands;
	public AcePile cups;
	public AcePile swords;
	public AcePile coins;
	public Discard discard;
	public Transform cascadesTransform;
	public Transform cascadeCardsTransform;
	public Transform majorsTransform;
	public Transform majorsCardsTransform;
	public float spacing;

	[HideInInspector]
	public List<AcePile> acePiles;
	[HideInInspector]
	public List<Placeholder> allPlaceholders = new List<Placeholder>();
	[HideInInspector]
	public List<Cascade> cascades = new List<Cascade>();
	public List<ArcanaPlaceholder> arcanas = new List<ArcanaPlaceholder>();
	
	// Use this for initialization
	void Start() {
		manager = this;
		for(int i = 0; i < 7; i++)
		{
			var obj = Instantiate (placeholderPrefab) as GameObject;
			BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
			Cascade cascade = obj.GetComponent<Cascade>();
			cascades.Add (cascade);
			cascade.transform.parent = cascadesTransform;
			cascade.cardsRootTransform.parent = cascadeCardsTransform;
			float width = boxCollider.bounds.size.x;
			cascade.transform.localPosition = new Vector3(i * (width + spacing),0,0);
			cascade.cardsRootTransform.localPosition = new Vector3(i * (width + spacing),0,0);
			cascade.transform.rotation = obj.transform.parent.rotation;
			cascade.cardsRootTransform.rotation = cascadeCardsTransform.rotation;
			obj.transform.localRotation *= Quaternion.AngleAxis(-90, new Vector3(1,0,0)); 
			allPlaceholders.Add (cascade);
		}
		//acePiles.AddRange(new List<AcePile>{wands,cups,swords,coins});
		allPlaceholders.AddRange(from p in acePiles select (Placeholder)p);
		allPlaceholders.AddRange (from p in cascades select(Placeholder)p);
		allPlaceholders.Add (deck);
		allPlaceholders.Add (hand);
		allPlaceholders.Add (discard);
		hand.deck = deck;
		hand.discard = discard;
		deck.hand = hand;
		deck.discard = discard;
	}
	public void MakeMajorsPlaceholders(List<Card> cards)
	{
		int count = 0;
		for(int i = 0; i < 4; i++)
		{
			
			for(int j = 0; j < 6; j++)
			{
				if(count < 22)
				{
					var obj = Instantiate (majorArcanaPlaceholderPrefab) as GameObject;
					BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
					ArcanaPlaceholder arcana = obj.GetComponent<ArcanaPlaceholder>();
					arcanas.Add (arcana);
					arcana.transform.parent = majorsTransform;
					arcana.cardsRootTransform.parent = majorsCardsTransform;
					float width = boxCollider.bounds.size.x;
					arcana.transform.localPosition = new Vector3(j * width,0,i*width*1.6f);
					arcana.cardsRootTransform.localPosition = new Vector3(j * width,0,i*width*1.6f);
					arcana.transform.rotation = obj.transform.parent.rotation;
					arcana.cardsRootTransform.rotation = majorsCardsTransform.rotation;
					obj.transform.localRotation *= Quaternion.AngleAxis(-90, new Vector3(1,0,0)); 
					arcana.AddCards (new List<Card> {cards[count]});						
					count++;
					allPlaceholders.Add (arcana);
				}	
			}
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
	public IEnumerator Deal()
	{
		Game.game.state =GameState.Dealing;
		CursorControl.cursor.enabled = false;
		for(int i = 0; i < 7; i++)	
		{
			for(int j = 6; j >= i; j--)
			{
				var card = deck.cards[deck.cards.Count -1];	
				cascades[j].AddCards(new List<Card>{card});
				yield return new WaitForSeconds(0.1f);
				if(j==i) card.FlipUp();
				else card.FlipDown();			
			}
		}
		//normally it is done after each time "AddCards" is called
		//but the cards go up and down repeatedly if you do that
		//while dealing. So we call it after all "AddCards" method calls
		//have been completed.
		foreach(var c in cascades)
		{
			c.AnimateDealCards();
		}
		foreach(var arcana in arcanas)
		{
			arcana.cards[0].FlipUp ();
		}
		UndoManager.manager.enabled = true;
		Game.game.state =GameState.HandEmpty;
		CursorControl.cursor.enabled = true;
	}
	public void HighlightRow(int row)
	{
		UnHighlightAll();
		foreach( var cascade in cascades)
		{
			if(cascade.cards.Count > row)
			{
				cascade.cards[row].Highlight(Color.green);
			}
		}
		
	}
	public void UnHighlightAll()
	{
		foreach( var cascade in cascades)
		{
			foreach(var card in cascade.cards)
			{
				card.UnHighlight();
			}
			
		}
	}
	
}
