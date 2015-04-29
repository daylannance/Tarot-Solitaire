using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcanaCard : Card {
	public int ArcanaCardRank;
	public GUIText descriptionText;
	public string title;
	override public void Clicked(MouseEvent evt)
	{
		if(evt.button == MouseButton.Left)
		{
			if(parentPlaceholder != null)
			{
				parentPlaceholder.OnCardClicked(this);
			}
		}
	}
	override public void OnSelected()
	{
		base.OnSelected();
		switch(ArcanaCardRank)
		{
			case 6:
				Game.game.state =GameState.SelectingARow;
				break;
			default:
				Game.game.state =GameState.SpecialCardInHand;
				break;
		}
	}
	public void Apply(Card card)
	{
		bool success = false;
		Cascade aph = (Cascade)card.parentPlaceholder;
		if(FilterPlaceholders(ArcanaCardRank,card)) return;
		switch(ArcanaCardRank)
		{
			case 0:
					success = aph.ReverseCards(card);
					aph.ReverseArrows();
					aph.behaviour = Behaviour.ReverseAll;
				break;
			case 1:
					success = aph.SortNumericalDescending(card);
				break;
			case 2:
					success = aph.SortByZodiac(card);
				break;
			case 3:
					success = aph.SortBySuit(card);
				break;
			case 4:
					success = aph.RevealOneUp();
				break;
			case 5:
					success = aph.MoveToBottom (card);
				break;
			case 6:
					success = aph.MoveRowToBottom(card);
				break;


		}
		if(success)
		{
			Done ();
		}
	}
	public void Done()
	{
		wasUsed = true;
		PlaceholderManager.manager.arcanas[ArcanaCardRank].AddCards(new List<Card>{this});
		if(Game.game.cardInHand == null)
		{
			Game.game.state =GameState.HandEmpty;
		}
		else {
			Game.game.state =GameState.CardInHand;
		}
			
	}
	bool FilterPlaceholders(int ArcanaCardRank, Card card)
	{
		bool doNotDoAnything = false;
		switch(ArcanaCardRank)
		{
		default:
			if(card.parentPlaceholder is AcePile)
			{
				doNotDoAnything = true;
			}
			break;
		}
		return doNotDoAnything;
	}
	public string GetTitle()
	{
		string title = "";
		switch(ArcanaCardRank)
		{
			case 0:
				title = "The Fool";
				break;
			case 1:
				title = "The Magician";
				break;
			case 2:
				title = "The High Priestess";
				break;
			case 3:
				title = "The Empress";
				break;
			case 4:
				title = "The Emperor";
				break;
			case 5:
				title = "The Hierophant";
				break;
			case 6:
				title = "The Lovers";
				break;
			case 7:
				title = "The Chariot";
				break;
			case 8:
				title = "Strength";
				break;
			case 9:
				title = "The Hermit";
				break;
			case 10:
				title = "Wheel of Fortune";
				break;
			case 11:
				title = "Justice";
				break;
			case 12:
				title = "The Hanged Man";
				break;
			case 13:
				title = "Death";
				break;
			case 14:
				title = "Temperance";
				break;
			case 15:
				title = "The Devil";
				break;
			case 16:
				title = "The Tower";
				break;
			case 17:
				title = "The Star";
				break;
			case 18:
				title = "The Moon";
				break;
			case 19:
				title = "The Sun";
				break;
			case 20:
				title = "Judgement";
				break;
			case 21:
				title = "The World";
				break;
		}
		return title;
	}
	public string GetDescription()
	{
		string description = "";
		switch(ArcanaCardRank)
		{
			case 0:
				description = "Reverses Card Order";
				break;
			case 1:
				description = "Sorts by Suit";
				break;
			case 2:
				description = "Sorts by Zodiac";
				break;
			case 3:
				description = "Reveals a card";
				break;
			case 4:
				description = "Unreverses cards";
				break;
			case 5:
				break;
			case 6:
				break;
			case 7:
				break;
		}
		return description;
	}
	override public void MouseOver()
	{
		base.MouseOver();
		//descriptionText.text = GetDescription();
	}
	override public string ToString()
	{
		return GetTitle();
	}
}
