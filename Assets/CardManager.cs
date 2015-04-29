using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum DeckArt{Dali, RiderWaite}
public class CardManager : MonoBehaviour {
	public GameObject cardPrefab;
	public GameObject ArcanaCardPrefab;
	public List<Card> cards;
	public static List<ArcanaCard> ArcanaCards = new List<ArcanaCard>();
	public GUIText cardInfoText;
	public GUITexture highlightedCardImage;
	public Transform majorsTransform;
	public Dictionary<string,int> materialIDs;
	public DeckArt deckArt;
	private static CustomMouse _currentHighlighted = null;
	public static CustomMouse currentHighlighted
	{
		get
		{ 
			return _currentHighlighted;
		}
		set
		{ 
			if(_currentHighlighted != null)
			{
				_currentHighlighted.UnHighlight();
			}
			_currentHighlighted = value;
			if(value != null)
			{
				_currentHighlighted.Highlight(Color.white);
			}
		}
	}
	public List<Card> MakeArcanaCards()
	{
		var pos = Vector3.zero;
		var mCards = new List<Card>();
		for(int i = 0; i < 22 ; i++)
		{
			
			GameObject obj = Instantiate (ArcanaCardPrefab, pos, Quaternion.identity) as GameObject;
			var card = obj.GetComponentInChildren<ArcanaCard>();
			mCards.Add (card);
			cards.Add(card);
			card.ArcanaCardRank = i;
			string path = "";
			switch(deckArt)
			{
			case DeckArt.Dali:
				path = "DaliCards/" + i;
				break;
			case DeckArt.RiderWaite:
				path = "RiderWaiteCards/" + i;
				break;
			}
			
			
			card.FlipUp();
			card.gameObject.GetComponent<Renderer>().materials[materialIDs["Picture"]].mainTexture = Resources.Load<Texture2D>(path);
			card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperLeft"]].mainTexture = Resources.Load<Texture2D>(path);
			card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperRight"]].mainTexture = Resources.Load<Texture2D>(path);	
		}
		//PlaceholderManager.manager.majorsDeck.AddCards (mCards);
		return mCards;
	}
	private static Card _currentCard;
	public static Card currentCard
	{
		get{return _currentCard;
			}
		set{_currentCard = value;}	
	}

	public static CardManager cardManager;
	
	public void DisplayCardInfo(string text, Texture texture)
	{
		cardInfoText.text = text;
		if(texture != null)
		{
			highlightedCardImage.texture = texture;
		}
	}
	void ClearCardInfoDisplay()
	{
		cardInfoText.text = "";
	}
	void Start () {
		if(cardManager == null)
		{
			cardManager = this;
			DontDestroyOnLoad(cardManager);
		}
		else 
		{
			Destroy (this);
			return;	
		}
		materialIDs = new Dictionary<string,int>{
			{ "CenterOfCardBack", 6},
			{ "CardBackMain", 5},
			{ "Picture", 2},
			{"Edges", 1},
			{ "PictureBorder",2},
			{ "UpperLeft", 6},
			{ "CardBackBorder",7},
			{ "UpperRight",3}
		};
		Vector3 pos = Vector3.zero;
		Card card;
		foreach(var suit in new List<Suit>{Suit.Wands,Suit.Cups,Suit.Swords,Suit.Coins})
		{
			for(int i = 0; i < 14; i++)
			{
				GameObject obj = Instantiate (cardPrefab, pos, Quaternion.identity) as GameObject;
				card = obj.GetComponentInChildren<Card>();
				cards.Add (card);
				string path = "";
				switch(deckArt)
				{
				case DeckArt.Dali:
					path = "DaliCards/" + suit + (i+1);
					break;
				case DeckArt.RiderWaite:
					path = "RiderWaiteCards/" + suit + (i+1);
					break;
				}
				
				card.gameObject.GetComponent<Renderer>().materials[materialIDs["Picture"]].mainTexture = Resources.Load<Texture2D>(path);
				path = "Numbers/" + (i+1);
				card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperLeft"]].mainTexture = Resources.Load<Texture2D>(path);
				string str = suit.ToString () + (i+1);
				Debug.Log (str);
				var zodiac = zodiacReference[str];
				card.zodiac = zodiac;
				card.rank = (Rank)i;
				card.suit = suit;
				if(zodiac != Zodiac.None)
				{
					var zodiacPath = "Zodiac/" + zodiac.ToString ();
					card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperRight"]].mainTexture = Resources.Load<Texture2D>(zodiacPath);
					card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperRight"]].SetTexture("_BumpMap", Resources.Load<Texture2D>(zodiacPath));
					card.gameObject.GetComponent<Renderer>().materials[materialIDs["UpperRight"]].SetTexture("_ParallaxMap", Resources.Load<Texture2D>(zodiacPath));	
					
				}
				
			}
			
		}
		BuildDeck ();
		PlaceholderManager.manager.MakeMajorsPlaceholders(MakeArcanaCards());
		
	}
	public void BuildDeck()
	{
		List<Card> list = new List<Card>();
		foreach(var card in cards)
		{
			list.Add(card);
		}
		PlaceholderManager.manager.deck.AddCards (list);
		Game.game.OnCardsReady(PlaceholderManager.manager.deck);
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}

	public Dictionary<string,Zodiac> zodiacReference = new Dictionary<string,Zodiac>
	{
		{"Wands1",Zodiac.None},
		{"Wands2", Zodiac.Aries},
		{"Wands3", Zodiac.Aries},
		{"Wands4",Zodiac.Aries},
		{"Wands5",Zodiac.Leo},
		{"Wands6",Zodiac.Leo},
		{"Wands7",Zodiac.Leo},
		{"Wands8",Zodiac.Sagittarius},
		{"Wands9",Zodiac.Sagittarius},
		{"Wands10",Zodiac.Sagittarius},
		{"Wands11",Zodiac.None},
		{"Wands12",Zodiac.None},
		{"Wands13",Zodiac.None},
		{"Wands14",Zodiac.None},
		{"Cups1",Zodiac.None},
		{"Cups2",Zodiac.Cancer},
		{"Cups3",Zodiac.Cancer},
		{"Cups4",Zodiac.Cancer},
		{"Cups5",Zodiac.Scorpio},
		{"Cups6",Zodiac.Scorpio},
		{"Cups7",Zodiac.Scorpio},
		{"Cups8",Zodiac.Pisces},
		{"Cups9",Zodiac.Pisces},
		{"Cups10",Zodiac.Pisces},
		{"Cups11",Zodiac.None},
		{"Cups12",Zodiac.None},
		{"Cups13",Zodiac.None},
		{"Cups14",Zodiac.None},
		{"Swords1",Zodiac.None},
		{"Swords2",Zodiac.Libra},
		{"Swords3",Zodiac.Libra},
		{"Swords4",Zodiac.Libra},
		{"Swords5",Zodiac.Aquarius},
		{"Swords6",Zodiac.Aquarius},
		{"Swords7",Zodiac.Aquarius},
		{"Swords8",Zodiac.Gemini},
		{"Swords9",Zodiac.Gemini},
		{"Swords10",Zodiac.Gemini},
		{"Swords11",Zodiac.None},
		{"Swords12",Zodiac.None},
		{"Swords13",Zodiac.None},
		{"Swords14",Zodiac.None},
		{"Coins1",Zodiac.None},
		{"Coins2",Zodiac.Capricorn},
		{"Coins3",Zodiac.Capricorn},
		{"Coins4",Zodiac.Capricorn},
		{"Coins5",Zodiac.Taurus},
		{"Coins6",Zodiac.Taurus},
		{"Coins7",Zodiac.Taurus},
		{"Coins8",Zodiac.Virgo},
		{"Coins9",Zodiac.Virgo},
		{"Coins10",Zodiac.Virgo},
		{"Coins11",Zodiac.None},
		{"Coins12",Zodiac.None},
		{"Coins13",Zodiac.None},
		{"Coins14",Zodiac.None}
	};
}
public enum Zodiac
{
	Aquarius,
	Pisces,
	Aries,
	Taurus,
	Gemini,
	Cancer,
	Leo,
	Virgo,
	Libra,
	Scorpio,
	Sagittarius,
	Capricorn,
	None
}