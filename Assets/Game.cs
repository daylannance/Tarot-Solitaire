using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum GameState
{
	Paused,
	CardInHand,
	HandEmpty,
	SpecialCardInHand,
	CardInhandAndSpecialCardInHand,
	SelectingSpecialCard,
	SelectingARow,
	Dealing,
	ViewingNormalCard,
	ViewingSpecialCard
}
public delegate void GameStateUpdated(GameState state);
public class Game : MonoBehaviour {
	
	[HideInInspector]
	public Camera currentCam;
	public GameObject table;
	public Camera mainCam;
	public static Game game;
	public CardViewer cardViewer;
	public Hand hand;
	public static GameStateUpdated gameStateUpdated;
	Deck deck;
	Card _cardInHand;
	public Stack<GameState> stateStack = new Stack<GameState>();
	public Card cardInHand
	{
		get{ return _cardInHand;}
		set{
		 if(_cardInHand != null)
		 {
			_cardInHand.isGlowing = false; 			
		 }
		 _cardInHand = value;
		
		if(value == null)
		{
			state = GameState.HandEmpty;
		}
		else
		{
			state = GameState.CardInHand;
				_cardInHand.isGlowing = true;
		}
	}}
	public GameState _state = GameState.HandEmpty;
	public  GameState state { 
		get{ return _state;} 
		set{
			_state = value; 
			//gameStateUpdated(state); 
		}}
	// Use this for initialization
	void Start () {
		if(game == null)
		{
			game = this;
			DontDestroyOnLoad(game);
			DontDestroyOnLoad(currentCam);
		}
		else 
		{	
			Destroy (gameObject);	
		}
		currentCam = mainCam;
		CursorControl.cursor.MouseClicked += OnClick;
		CursorControl.cursor.enabled = false; //lock cursor so there are no collection modification errors during startup.
	}
	public void OnLeftClick(Card card)
	{

	}
	public void OnMiddleClick(Card card)
	{

	}
	public void OnRightClick(Card card)
	{

	}
	// Update is called once per frame
	void Update () {
		
	}
	public void CardClicked(Card card)
	{
		switch(state)
		{
			case GameState.Paused:
				break;
			case GameState.HandEmpty:
				if(card.isFacingUp)
				{
					cardInHand = card;
				}
				else if(card = card.parentPlaceholder.cards[card.parentPlaceholder.cards.Count-1])
				{
					card.FlipUp();
				}
				break;
			case GameState.CardInHand:
				if(card.parentPlaceholder != cardInHand.parentPlaceholder)
				{					
					CardClickedWithCardInHand(card, cardInHand);
				}	
				else if(cardInHand == null)
				{
					state = GameState.HandEmpty;
				}
				else {
					cardInHand = card;	
				}
				break;
			case GameState.SpecialCardInHand:
				break;
		}
	}
	public void ZoomToCloseup(Vector3 worldPos)
	{
		if(CardManager.currentHighlighted != null)
		{
			float height = CardManager.currentHighlighted.GetComponent<Renderer>().bounds.size.y;
			CameraManager.manager.ZoomToFit (4, worldPos);
		}
	}
	public void MiddleClicked(Vector3 worldPos)
	{
		switch(Game.game.state)
		{
			case GameState.HandEmpty:
				ZoomToCloseup(worldPos);
			break;
			case GameState.CardInHand:
				ZoomToCloseup(worldPos);
				//ShowMajors ();
				break;
			case GameState.SelectingSpecialCard:
				HideMajors ();		
				break;
		}
	}
	public void ShowMajors()
	{
		Game.game.cardInHand = null;
		MajorArcanaManager.manager.Show();
		table.SetActive(false);
		Game.game.state =GameState.SelectingSpecialCard;
	}
	public void HideMajors()
	{
		MajorArcanaManager.manager.Hide();
		Game.game.state =GameState.HandEmpty;
		table.SetActive(true);
	}
	public void PutCardBack()
	{
		if(cardInHand != null)
		{
			cardInHand = null;
		}
	}
	void CardClickedWithCardInHand(Card clickedCard, Card card)
	{
		clickedCard.parentPlaceholder.CardClickedWithCardInHand(clickedCard, card);
	}
	public void PlaceholderClicked(Placeholder p){}
	public void PlaceholderClicked(Cascade c)
	{
		switch(state)
		{
			case GameState.HandEmpty:
				break;
			case GameState.CardInHand:
				if(c.cards.Count == 0)
				{
					c.AddCards(cardInHand.parentPlaceholder.RemoveCardAndChildren(cardInHand));
					cardInHand = null;
				}		
				break;
		}

	}
	public void ShowCardViewer(Card card)
	{
		table.SetActive(false);
		stateStack.Push(state);
		state = GameState.ViewingNormalCard;
		Game.game.currentCam.enabled = false;
		Game.game.cardViewer.Show (card);
		
		
	}
	public void HideCardViewer()
	{
		table.SetActive(true);
		state = stateStack.Pop ();
		currentCam.enabled = true;
		cardViewer.Hide();
	}
	public void OnClick(MouseEvent evt)
	{
		if(evt.button == MouseButton.Right) UndoManager.manager.Undo();
	}
	public void CardDescriptionButtonPressed()
	{
		switch(Game.game.state)
		{
		case GameState.ViewingNormalCard:
		case GameState.ViewingSpecialCard:
			HideCardViewer();
			break;
		default:
			int cardMask=0;
			if(game.state == GameState.SelectingSpecialCard)
			{
				cardMask = CursorControl.cursor.specialCardMask;
			}
			else
			{
				cardMask = CursorControl.cursor.cardMask;
			}
			CustomMouse customMouse = CursorControl.cursor.GetTargetUnderCursor(cardMask);
			if(customMouse != null && customMouse is Card )
			{
				Card card = (Card)customMouse;
				if(card.isFacingUp)
				{
					ShowCardViewer(customMouse as Card);
				}	
			}
			break;
		}
		
	}
	public void PlaceholderClicked(Deck d){}
	public void PlaceholderClicked(AcePile a){}
	public void PlaceholderClicked(Hand d){}
	
	public void OnCardsReady(Deck cards)
	{	
		deck = cards;
		deck.Shuffle ();
		deck.StartCoroutine (deck.Bridge (new CallBack(Deal)));	
	}
	public void Deal()
	{
		StartCoroutine(PlaceholderManager.manager.Deal());
	}
}
