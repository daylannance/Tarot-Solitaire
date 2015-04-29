using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Suit{Wands = 0,Cups = 1,Swords = 2,Coins = 3,  NotSpecified = 4};
public class AcePile : Placeholder {
	public Suit suit;
	// Use this for initialization
	override public void Start () {
		base.Start ();
		cardAngle = -90;
		
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update ();
	}
	override public bool ApplyRules(Card card, Card newCard)
	{
		bool result = false;
		//ghetto way to see if there is only one card
		if(newCard.parentPlaceholder.cards.Count -1 == newCard.parentPlaceholder.cards.IndexOf(newCard))
		{
			if(newCard.suit == suit)
			{
				if(cards.Count == 0) 
				{
					if(newCard.rank == Rank.Ace)
					{
						result = true;
					}	
				}
				else if(newCard.rank == cards[cards.Count -1].rank + 1)
				{
					result = true;
				}
			}
		}
		return result;
	}
	override public void AddCards(List<Card> list)
	{
		base.AddCards(list);
	}
	override public void Clicked(MouseEvent evt)	
	{
		if(evt.button == MouseButton.Left)
		{
			switch(Game.game.state)
			{
				case GameState.CardInHand:
					if(ApplyRules(null, Game.game.cardInHand))
					{
						AddCards(Game.game.cardInHand.parentPlaceholder.GetFromToBottom(Game.game.cardInHand));
						Game.game.cardInHand = null;		
					}
					break;
			}
		}	
	}
	override public void OnCardClicked(Card card)
	{
		switch(Game.game.state)
		{
			case GameState.Paused:
				break;
			case GameState.HandEmpty:
				break;
			case GameState.CardInHand:
				if(this != Game.game.cardInHand.parentPlaceholder)
				{					
					if(ApplyRules(null, Game.game.cardInHand))
					{
						AddCards(Game.game.cardInHand.parentPlaceholder.GetFromToBottom(Game.game.cardInHand));
						Game.game.cardInHand = null;		
					}
				}				
				break;
		}
	}
}
