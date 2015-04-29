using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcanaPlaceholder : Placeholder {
	public int soleCardIndex;
	// Use this for initialization
	override public void Start () {
		base.Start ();
	}
 	override public void OnCardClicked(Card card)
	{
		base.OnCardClicked(card);
		switch(Game.game.state)
		{
			case GameState.SelectingSpecialCard:
				CursorControl.cursor.AddCards(new List<Card>{card});
				CursorControl.cursor.specialCard = (ArcanaCard)card;
				Game.game.state =GameState.SpecialCardInHand;
				Game.game.HideMajors();
				card.OnSelected();
				break;
		}
	}
	override public void AddCards(List<Card> list)
	{
		if(cards.Count > 1) throw new UnityException("Trying to add more than one card to an arcana placeholder!");
		foreach(var card in list)
		{
			if(!(card is ArcanaCard)) throw new UnityException("Trying to add a regular card to an arcana placeholder!");
			card.FlipUp();
		}
		base.AddCards(list);		
	}
	
}
