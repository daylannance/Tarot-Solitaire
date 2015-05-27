using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : Placeholder {
	public List<Transform> cardTransforms;
	public Deck deck;
	public Vector3 rotationBetweenCards;
	public Vector3 offsetBetweenCards;
	private Discard _discard;
	public Discard discard { get{ 
		return _discard;} 
		set{
			_discard = value;
		}}
	// Use this for initialization
	override public void Start () {
		base.Start ();
		SetCardSlotSpacing();
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update ();
		#if UNITY_EDITOR
		SetCardSlotSpacing();
		#endif
	}
	void SetCardSlotSpacing()
	{
		for(int i = 0; i < cardTransforms.Count; i++)
		{
			cardTransforms[i].rotation = cardsRootTransform.rotation * Quaternion.AngleAxis (rotationBetweenCards.z * i, new Vector3(0,0,1));
			cardTransforms[i].rotation = cardTransforms[i].rotation * Quaternion.AngleAxis (rotationBetweenCards.y, new Vector3(0,1,0));
			cardTransforms[i].position = cardsRootTransform.position + offsetBetweenCards * i;
		}
	}
	override public void AddCards(List<Card> cards)
	{
		base.AddCards(cards);
		foreach(var card in cards)
		{
			card.GetComponentInChildren<Renderer>().sortingLayerID = LayerMask.GetMask (new string[]{"Cards, CardsInHand"});
		}
		AnimateMoveCards();
	}
	public void RotateSet(List<Card> set )
	{
		foreach(var card in set)
		{
			//card.StopAllCoroutines();
			
		}
		var tempCards = new List<Card>();
		
		if(cards.Count > 0)
		{
			tempCards.AddRange (cards.GetRange(0,cards.Count));
			cards.Clear();		
			discard.AddCards(tempCards);
		}
		AddCards (set);
		if(cards.Count > 0)
		{
			Game.game.cardInHand = cards[cards.Count -1];
		}
	}
	override public void SetCardTransformPosition(Card card)
	{
		card.FlipUp();
		try
		{
			card.transform.SetParent (cardTransforms[cards.Count]);
		}
		catch(System.ArgumentOutOfRangeException e)
		{
			Debug.Log ("*+*+*+Caught an index out of range exception while clicking. Attempting to Undo");
			UndoManager.manager.Undo();
		}
		card.transform.GetChild(0).position = Vector3.zero;
		card.transform.GetChild (0).localRotation = new Quaternion(0,0,0,0);
		
		//base.SetCardTransformPosition(card);
		//card.FlipUp();
		//card.transform.parent.rotation = Game.game.mainCam.transform.rotation;
		//card.transform.parent.rotation *= Quaternion.AngleAxis(180, new Vector3(0,1,0));
		//int cardIndex = cards.IndexOf(card);
		//card.transform.parent.rotation *= Quaternion.AngleAxis (-20 + 20 * cardIndex, new Vector3(0,0,1));		
	}
	
	override public void OnCardClicked(Card card)
	{
		base.OnCardClicked (card);
		switch(Game.game.state)
		{
			case GameState.CardInHand:			
				//intentional fall through
			case GameState.HandEmpty:
				Game.game.cardInHand = cards[cards.Count -1];
				break;
		}
		
	}
}
