using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cascade : Placeholder {
	public enum State { Frozen, NotFrozen, Sorting};
	public State state = State.NotFrozen;
	public bool reverseOrder;
	ArrowIndicators arrows;
	public CoinManager coinManager;
	// Use this for initialization
	override public void Start () {
		base.Start ();
		arrows = GetComponentInChildren<ArrowIndicators>();
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update();
	}
	public CoinMounter GetCoinMounter(Card card)
	{
		return coinManager.GetCoinMounter(card);
	}
	public void ReverseArrows()
	{
		arrows.ReverseArrows();
	}
	override public void AddCards(List<Card> cardList)
	{
		base.AddCards(cardList);
	}
	override public void CardClickedWithCardInHand(Card card, Card cardInHand)
	{
		if(card == cards[cards.Count -1] )
		{
			if( ApplyRules(card, Game.game.cardInHand))
			{
				AddCards(Game.game.cardInHand.parentPlaceholder.RemoveCardAndChildren(Game.game.cardInHand));			
				Game.game.cardInHand = null;
			}
			else
			{
				Game.game.cardInHand = card;
			}
		}
	}
	
	public override bool ApplyRules(Card card, Card newCard )
	{
		bool result = false;
		int targetRankDiff = -1;
		int targetZodiacDiff = 1;

		switch(behaviour)
		{
			case Behaviour.Normal:
				targetRankDiff = -1;
				targetZodiacDiff = 1;
				break;
			case Behaviour.ReverseAll:
				targetZodiacDiff = -1;
				targetRankDiff = 1;
				break;
		}
		
		if(newCard.rank == card.rank + targetRankDiff)  result = true;
		if(!card.isFacingUp) result = true;
		if(newCard.zodiac == card.zodiac + targetZodiacDiff) 
		{
			result = true;
			card.isInZodiacSequence = true;
			newCard.isInZodiacSequence = true;
		}	
		return result;
	}
	
	
	override public void Clicked(MouseEvent evt)
	{
		if(evt.button == MouseButton.Left)Game.game.PlaceholderClicked(this);
	}
	override public void OnCardClicked(Card card)
	{
		base.OnCardClicked(card);
		switch(Game.game.state)
		{
			case GameState.Paused:
				break;
			case GameState.HandEmpty:
				if(card.isFacingUp)
				{
					Game.game.cardInHand = card;
				}
				else if(card == cards[card.parentPlaceholder.cards.Count-1])
				{
					card.FlipUp();
					Game.game.cardInHand = null;
				}
				break;
			case GameState.CardInHand:
				if(Game.game.cardInHand == null) throw new UnityException("State is 'Card in Hand' but cardInHand is null!");
				if(this != Game.game.cardInHand.parentPlaceholder)
				{					
					CardClickedWithCardInHand(card, Game.game.cardInHand);
				}	
				if(Game.game.cardInHand != null)
				{
					Game.game.cardInHand = card;
				}
				break;
			case GameState.SpecialCardInHand:
				CursorControl.cursor.specialCard.Apply (card);
				break;
			case GameState.SelectingARow:
				CursorControl.cursor.specialCard.Apply (card);
				break;
		}
	}
	public override void CheckZodiacSequences()
	{
		int targetDiff = 1;
		switch(behaviour)
		{
		case Behaviour.Normal:
			targetDiff = 1;
			break;
		case Behaviour.ReverseAll:
			targetDiff = -1;
			break;
		}
		int consecutiveCount = 1;
		foreach(var card in cards)
		{
			if(card.Previous() != null && card.Previous().isFacingUp)
			{
				if(card.zodiac == card.Previous().zodiac + targetDiff)
				{
					if(!card.Previous().mounter) card.Previous ().mounter = coinManager.GetCoinMounter(card.Previous());
					card.Previous().isInZodiacSequence = true;
					card.Previous ().mounter.targetCount = consecutiveCount;
					consecutiveCount++;
					card.isInZodiacSequence = true;
					if(!card.mounter) card.mounter = coinManager.GetCoinMounter(card);
					card.mounter.targetCount = consecutiveCount;
					
				}
				else
				{
					card.isInZodiacSequence = false;
					consecutiveCount = 1;
				}
			}
			else 
			{
				//card.isInZodiacSequence = false;
			}
		}
		UpdateCoins ();
	}
	override protected void UpdateCoins()
	{
		StartCoroutine ("AnimateAddCoins");
	}
	override protected IEnumerator AnimateAddCoins()
	{
		int quantityToAdd = 0;
		foreach(var card in cards)
		{
			if(card.mounter == null) continue;
			quantityToAdd += card.mounter.targetCount - card.mounter.coins.Count;
		}
		float delay = 1f/quantityToAdd;
		//modified while iterating error when I used a foreach
		for(int i=0; i<cards.Count; i++)
		{
			if(!cards[i].mounter) continue;
			while(cards[i].mounter.targetCount > cards[i].mounter.coins.Count)
			{
				cards[i].mounter.AddCoin();
				yield return new WaitForSeconds(delay);	
			}
		}
		yield return null;
	}
	public bool ReverseCards(Card card, bool reverseRules = false, List<Card> reversedCards = null)
	{
		bool reversedAlready = behaviour != Behaviour.ReverseAll? true:false;
		if(reverseRules)
		{
			behaviour = Behaviour.ReverseAll;
			arrows.ReverseArrows();
		}
		if(reversedCards == null)
		{
			reversedCards = new List<Card>();
		}
		//if rules were already reversed and cards were already reversed return false
		return (!reversedAlready && reverseRules) || base.ReverseCards(card, out reversedCards);
	}
	
	public override void RestoreBehaviour(Behaviour b)
	{
		base.RestoreBehaviour(b);

		switch(b)
		{
			case Behaviour.Normal:
				arrows.ForwardArrows ();
				break;
			
		}
	}
	
}
