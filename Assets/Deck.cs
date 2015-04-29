using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deck : Placeholder {
    public Hand hand;
	public Discard discard;
	// Use this for initialization
	override public void Start () {
		base.Start ();
	}
	override public void AddCards(List<Card> list)
	{
		base.AddCards(list);
		foreach(var card in list)
		{
			if(list.IndexOf (card)!= list.Count -1)
			{
				card.FlipUp();
			}
			else if(!card.isFacingUp)
			{
				card.FlipDown();
			}
		}
	}
	override public void CheckZodiacSequences(){}
	// Update is called once per frame
	override public void Update () {
		base.Update ();
	}
	public IEnumerator Deal()
	{
		yield return null;
	}
	public IEnumerator Bridge(CallBack callback)
	{
		foreach(var card in cards)
		{
			card.FlipDown ();
			yield return new WaitForSeconds(0.05f);
		}
		callback();
	}
	override public void OnCardClicked(Card card)	
	{
		base.OnCardClicked(card);
		switch(Game.game.state)
		{
			case GameState.CardInHand:
				hand.RotateSet(NextSet(3));
				break;
				//intentional fall through
			case GameState.HandEmpty:
				hand.RotateSet(NextSet(3));
				break;
			case GameState.Paused:
				break;
			case GameState.SpecialCardInHand:
				break;
		}
	}
	public List<Card> NextSet(int quantity)
	{
		var cardSet = new List<Card>();
		if(cards.Count > 0) //if there is at least one card for another set:
		{
			for(int i = 0; i < quantity; i++)
			{
				if(cards.Count > 0)//...there may not have been enough cards for a complete set.
				{
					var card = cards[cards.Count - 1];
					if(card.parentPlaceholder && card.parentPlaceholder.cards.Contains(card))
					{
						cards.Remove(card);
					}
					cardSet.Add(card);			
				}
			}
			cardSet.Reverse();
		}
		else //...there were no cards at all
		{
			AddCards (discard.Recycle());
			Game.game.cardInHand = null;
		}
		return cardSet;
	}
	override public void Clicked(MouseEvent evt)
	{
		base.Clicked (evt);
		if(evt.button == MouseButton.Left)
		{
			switch(Game.game.state)
			{
			case GameState.CardInHand:
				//intentional fall through
			case GameState.HandEmpty:
				hand.RotateSet(NextSet(3));
				break;
			case GameState.Paused:
				break;
			case GameState.SpecialCardInHand:
				break;
			}
		}
	}
	public void Shuffle()
	{
		List<Card> tempList = new List<Card>();
		foreach(var card in cards)
		{
			tempList.Add (card);
		}
		cards.Clear ();
		while(tempList.Count > 0)
		{
			int random = Random.Range (0, tempList.Count -1);
			cards.Add (tempList[random]);
			tempList.RemoveAt(random);
		}
		Regroup();
	}
}

public delegate void CallBack();