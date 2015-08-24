using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class Cascade : Placeholder {
	public enum State { Frozen, NotFrozen, Sorting};
	public State state = State.NotFrozen;
	public bool reverseOrder;
	ArrowIndicators arrows;
	// Use this for initialization
	override public void Start () {
		base.Start ();
		arrows = GetComponentInChildren<ArrowIndicators>();
	}
	
	// Update is called once per frame
	override public void Update () {
		base.Update();
	}
	
	public void ReverseArrows()
	{
		arrows.ReverseArrows();
	}
	override public void FixedUpdate()
	{
		base.FixedUpdate ();
	}
	public void AnimateDealCards()
	{
		Dictionary<string,object> parameters = new Dictionary<string,object>();
		parameters.Add("reverse",false);
		animations.Enqueue(new CoroutineArgs(AnimateCardsDown, parameters));
	}
	override public void AddCards(List<Card> cardList)
	{
		base.AddCards(cardList);
		if(Game.game.state != GameState.Dealing)AnimateMoveCards();
	}
	override public void CardClickedWithCardInHand(Card card, Card cardInHand)
	{
		if(card == Game.game.cardInHand)
		{
			Game.game.cardInHand = null;
		}
		else if(card == cards[cards.Count -1] && cardInHand.parentPlaceholder != this)
		{
			List<Card> newCards = cardInHand.parentPlaceholder.GetCardAndChildren(Game.game.cardInHand);
			List<Card> faceupCards = GetFaceupCards ();
			//apply or ignore the rules
			
			if( ApplyRules(faceupCards, newCards) || Game.game.bypassRules)
			{
				AddCards(Game.game.cardInHand.parentPlaceholder.RemoveCardAndChildren(Game.game.cardInHand));	
				Game.game.cardInHand = null;
			}
			else {
				Game.game.cardInHand = card;
			}
		}
		else
		{
			Game.game.cardInHand = card;
		}
		
	}
	
	public override bool ApplyRules(List<Card> faceUpCards,List<Card> newCards )
	{
		if(Game.game.bypassRules) return true;
		//Any card can go on a facedown card:
		if(faceUpCards.Count ==0) return true;
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
		int[] poles = new int[]{1,-1};
		foreach(var diff in poles)
		{
			//cannot do descending order if there is only one card, except if behaviour is reverse
			//number or reverse all:	
			if(
				(diff == -1 && faceUpCards.Count == 1)&&
				(
					faceUpCards[0].parentPlaceholder.behaviour != Behaviour.ReverseNumbers
					|| faceUpCards[0].parentPlaceholder.behaviour != Behaviour.ReverseAll
				)
			) break;
 			
			if(CheckNumericOrder(faceUpCards, diff))
			{
				if(CheckNumericOrder(newCards, diff))
				{
					List<Card> lastAndFirstCard = new List<Card>();
					lastAndFirstCard.Add((Card)faceUpCards.Last ());
					lastAndFirstCard.Add ((Card)newCards.First());
					if(CheckNumericOrder (lastAndFirstCard, diff))
					{
						result = true;
					}
				}
			}
		}
		//if(newCard.zodiac == card.zodiac + targetZodiacDiff) 
		//{
		//	result = true;
		//	card.isInZodiacSequence = true;
		//	newCard.isInZodiacSequence = true;
		//}	
		return result;
	}
	
	
	override public void Clicked(MouseEvent evt)
	{
		if(evt.button == MouseButton.Left)
		{
			switch(Game.game.state)
			{
				case GameState.CardInHand:
					if(cards.Count == 0)
					{
						AddCards(Game.game.cardInHand.parentPlaceholder.RemoveCardAndChildren(Game.game.cardInHand));
						Game.game.cardInHand = null;
					}
					else if(ApplyRules(GetFaceupCards(),Game.game.cardInHand.parentPlaceholder.RemoveCardAndChildren(Game.game.cardInHand)))
					{
						AddCards(Game.game.cardInHand.parentPlaceholder.RemoveCardAndChildren(Game.game.cardInHand));
					    Game.game.cardInHand = null;				
					}
				break;
			}
		}
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
					Game.game.cardInHand = card;
				}
				break;
			case GameState.CardInHand:
				if(Game.game.cardInHand == null) throw new UnityException("State is 'Card in Hand' but cardInHand is null!");	
				CardClickedWithCardInHand(card, Game.game.cardInHand);	
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
				//I did this elsewhere too (this code should only happen once), just testing the coin mechanism
				//**Not sure if this code is being used!
				if(card.zodiac == card.Previous().zodiac + targetDiff)
				{
					card.Previous().isInZodiacSequence = true;
					card.Previous ().coinStash.targetCount = consecutiveCount;
					consecutiveCount++;
					card.isInZodiacSequence = true;
					card.coinStash.targetCount = consecutiveCount;
					
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
	}
	override protected void UpdateCoins()
	{
		CheckZodiacSequences();
		animations.Enqueue(new CoroutineArgs(AnimateAddCoins, new Dictionary<string,object>()));
	}
	override protected IEnumerator AnimateAddCoins(Dictionary<string,object> parameters)
	{
		if(cards.Count > 0)
		{
			CheckZodiacSequences();
			isAnimating = true;
			int quantityToAdd = 0;
			foreach(var card in cards)
			{
				quantityToAdd += card.coinStash.targetCount - card.coinStash.coins.Count;
			}
			if(quantityToAdd != 0)
			{
				float delay = 1f/quantityToAdd;
				//modified while iterating error when I used a foreach
				for(int i=0; i<cards.Count; i++)
				{
					while(cards[i].coinStash.targetCount > cards[i].coinStash.coins.Count)
					{
						cards[i].coinStash.AddCoin();
						yield return new WaitForSeconds(delay);	
					}
				}
			}
			isAnimating = false;
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
