using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public delegate void AddCardDelegate(Card card);
public enum Behaviour { Normal, ReverseAll, ReverseNumbers, ReverseZodiac, Stone }

public class Placeholder : CustomMouse {
	
	SpriteRenderer sprite;
	public Color onColor = new Color(1,1,1);
	public Color offColor;
	public int addedCards = 0;
	public List<Card> cards = new List<Card>();
	public Behaviour behaviour = Behaviour.Normal;
	public Transform cardsRootTransform;
	public Vector3 offset;
	public int id;
	public static int idCounter;
	
	override public bool isHighlighted
	{
		get { return _isHighlighted; }
		set 
		{ 
			if(value == false)
			{
				sprite.color = offColor;
			}
			else 
			{
				sprite.color = onColor;		
			}
			_isHighlighted = value;
		}
	}
	[HideInInspector]
	protected float cardAngle = -89.0f;
	// Use this for initialization
	public virtual void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		onColor = new Color(1,1,1);
		id = idCounter++;
	}
	public virtual void Start () 
	{
		sprite.color = offColor;
		GetComponent<Renderer>().sortingLayerID = LayerMask.GetMask (new string[]{"Cards", "CardsInHand"});
	}
	override public void UnHighlight()
	{
		isHighlighted = false;
		sprite.color = offColor;
	}
	public virtual void Update () {
	}
	public virtual void AddCardAndChildren(Card card)
	{

	}
	public virtual void RestoreBehaviour(Behaviour b)
	{
		if(behaviour != b)behaviour = b;
		else return;
	}
	public virtual List<Card> RemoveCardAndChildren(Card card)
	{
		List<Card> list = new List<Card>();
		int i = cards.IndexOf(card);
		while(i < cards.Count){
			list.Add (cards[i]);
			cards.RemoveAt(i);	
		}			
		return list;
	}
	override public void MouseOver()
	{
		CardManager.currentHighlighted = this;
	}
	public virtual void OnMouseExit()
	{
		//sprite.color = offColor;
	}
	
	public virtual bool ApplyRules(Card card)
	{
		return false;
	}
	public virtual void IdentifyZodiacSequences()
	{

	}
	public List<Card> GetFromToBottom(Card card)
	{
		int index = cards.IndexOf (card);
		int range = cards.Count - index;
		return cards.GetRange (index,range);
	}
	public List<Card> RemoveFaceupCards()
	{
		List<Card> list = new List<Card>();
		foreach(var card in cards)
		{
			if(card.isFacingUp) list.Add (card);
		}
		while(cards[cards.Count -1].isFacingUp)
		{
			cards.RemoveAt(cards.Count -1);
		}
		return list;
	}
	public List<Card> GetFaceupCards()
	{
		List<Card> list = new List<Card>();
		foreach(var card in cards)
		{
			if(card.isFacingUp) list.Add(card);
		}
		return list;
	}
	public virtual void Regroup()
	{
		List<Card> tempCards = new List<Card>();
		foreach(var card in cards)
		{
			tempCards.Add (card);
		}
		cards.Clear ();	
		AddCards (tempCards);
	}
	public virtual void CheckZodiacSequences()
	{
		
	}
	
	protected virtual void UpdateCoins()
	{
		
	}
	protected virtual IEnumerator AnimateAddCoins()
	{
		yield return null;
	}
	public virtual bool ApplyRules(Card card, Card newCard)
	{
		return false;
	}
	//Problem: Zodiac is being checked to see if a card should be added
	//and then also to see if the sequence has changed, and possibly a 
	//third time to see if coins need to be added or taken away.
	//There has to be a simpler way to apply rules.
	public virtual void AddCards(List<Card> list)
	{			
		UndoManager.manager.haveAnyCardsMoved = true;
		Placeholder previousParent = null;
		if(list.Count > 0)
		{
			previousParent = list[0].parentPlaceholder;
		}
		foreach(var card in list)
		{
			Parent (card);
			SetCardTransformPosition(card);
		}
		CheckZodiacSequences();
		if(previousParent != null)
		{
			previousParent.CheckZodiacSequences();
		}
		switch(Game.game.state)
		{
			case GameState.Dealing:
				break;
		}
		AnimateMoveCards ();
		
	}
	public void AnimateMoveCards()
	{
		List<Card> copyList = new List<Card>();
		foreach(var card in cards)
		{
			copyList.Add (card);
		}
		switch(Game.game.state)
		{
		case GameState.Dealing:
			foreach(var card in copyList)
			{
				card.transform.parent.localPosition = card.targetPosition;
			}
			break;
		default:
			StartCoroutine(AnimateMoveCardsCoroutine());
			break;
		}	
	}
	public void Parent(Card card)
	{
		if(card.parentPlaceholder != null)
		{
			card.RemoveSelfFromParent();
			card.GetComponent<Renderer>().sortingLayerID = this.GetComponent<Renderer>().sortingLayerID;
		}
		cards.Add (card);
		card.parentPlaceholder = this;
	}
	public virtual void SetCardTransformPosition(Card card)
	{
		if(cards.Count == 1)
		{
			card.transform.parent.parent = cardsRootTransform;
			//card.transform.parent.localPosition = Vector3.zero;
			card.targetPosition = Vector3.zero;
		}
		else 
		{
			card.transform.parent.parent = cardsRootTransform;
			//card.transform.parent.localPosition = offset * (cards.Count-1);
			card.targetPosition = offset * (cards.Count-1);
		}
		card.transform.parent.rotation = Quaternion.AngleAxis (cardAngle, new Vector3(1,0,0));
	}
	public IEnumerator AnimateMoveCardsCoroutine()
	{
		List<Card> copyList = new List<Card>();
		foreach(var card in cards)
		{
			copyList.Add (card);
		}
		float animateHeight =(cards.Count > 0)? cards[0].width * 2: 1f;
		copyList.Reverse ();
		foreach(var card in copyList)
		{
			if((card.targetPosition - card.transform.parent.localPosition).magnitude < .01f) continue;
			var pos = card.targetPosition;
			pos.z += animateHeight;
			card.StartCoroutine(card.AnimateMove (pos, .1f));
			card.FlipUp ();
			yield return new WaitForSeconds(.1f);
		}
		yield return new WaitForSeconds(.3f);
		copyList.Reverse ();
		foreach(var card in copyList)
		{
			if((card.targetPosition - card.transform.parent.localPosition).magnitude < .01f) continue;
			card.StartCoroutine(card.AnimateMove (card.targetPosition, .1f));
			yield return new WaitForSeconds(.1f);
		}
		StartCoroutine ("AnimateAddCoins");
	}
	public virtual void RepositionCards()
	{
		foreach(var card in cards)
		{
			SetCardTransformPosition(card);
		}
	}
	public virtual void CardClickedWithCardInHand(Card card, Card cardInHand)
	{
		
	}
	public virtual void OnCardClicked(Card card)
	{
		
	}
	override public void RecordUndo()
	{
		UndoManager.manager.SaveState ();
	}
	override public void Clicked(MouseEvent evt)
	{
		base.Clicked (evt);
		if(evt.button == MouseButton.Left)
		{
			Game.game.PlaceholderClicked(this);
		}
	}
	public virtual bool ReverseCards(Card card, out List<Card> reversedCards)
	{
		reversedCards = new List<Card>();
		if(cards.Count < 2) return false;
		reversedCards = RemoveFromCardToBottom (card);
		reversedCards.Reverse();
		AddCards (reversedCards);
		return true;		
	}
	public List<Card> RemoveFromCardToBottom(Card card)
	{
		List<Card> subset = new List<Card>();
		if(cards.Count > 0)
		{
			int index = cards.IndexOf(card);
			int count = cards.Count - index;
			subset = cards.GetRange(index, count);
			cards.RemoveRange(index,count);
		}
		return subset;
	}
	int CompareRank( Card x, Card y)
	{
		if((int)x.rank == (int)y.rank) return 0;
		if((int)x.rank < (int)y.rank) return 1;
		else return -1;
	}
	public virtual bool SortNumericalDescending(Card startCard)
	{
		if(GetFaceupCards().Count < 2) return false;
		List<Card> subset = RemoveFaceupCards();
		if(subset.Count < 2) return false;
		subset.Sort(CompareRank);
		AddCards (subset);
		return true;
	}
	public virtual bool SortByZodiac(Card startCard)
	{
		if(GetFaceupCards().Count < 2) return false;
		System.Comparison<Card> zodiacComparison = new System.Comparison<Card>(CompareZodiac);
		List<Card> subset = RemoveFaceupCards();
		subset.Sort (zodiacComparison);
		AddCards(subset);
		return true;
	}
	public virtual bool SortBySuit(Card startCard)
	{
		if(GetFaceupCards().Count < 2) return false;
		List<Card> subset = RemoveFaceupCards();
		subset.Sort (CompareSuit);
		AddCards(subset);
		return true; 
	}
	public virtual bool RevealOneUp()
	{
		bool isThereAFaceDownCard = false;
		foreach(var card in cards)
		{
			if(!card.isFacingUp) isThereAFaceDownCard = true;
		}
		if(!isThereAFaceDownCard) return false;
		foreach(var card in cards)
		{
			if(card.isFacingUp)
			{
				int i = cards.IndexOf(card);
				if(i > 0)
				{
					cards[i-1].FlipUp();
					return true;
				}
				return false;
			}
		}
		return false;
	}
	public virtual int CompareSuit(Card cardA, Card cardB)
	{
		int a = (int)cardA.suit;
		int b = (int)cardB.suit;
		if(a == b) return 0;
		if(a < b) return -1;
		if(a > b) return 1;
		return 0;	
	}
	public virtual int CompareZodiac(Card cardA, Card cardB)
	{
		int a = (int)cardA.zodiac;
		int b = (int)cardB.zodiac;
		int reverseFactor = 1;
		if(behaviour == Behaviour.ReverseAll) reverseFactor = -1;
		if( a == b) return 0;
		if(a < b) return -1 * reverseFactor;
		if(a > b) return 1 * reverseFactor;
		return 0;
	}
	public virtual bool MoveToBottom(Card card)
	{
		//AddCards automatically removes the card via the Parent() method
		if(!card.isFacingUp) return false;
		if(GetFaceupCards().Count < 2) return false;
		List<Card> tempList = new List<Card>();
		foreach(var copy in cards)
		{
			tempList.Add (copy);
		}
		cards.Clear ();
		tempList.Remove(card);
		AddCards (tempList);
		AddCards (new List<Card> { card });
		return true;
	}
	public virtual bool MoveRowToBottom(Card card)
	{
		bool somethingHappened = false;
		int index = cards.IndexOf(card);
		foreach(var cascade in PlaceholderManager.manager.cascades)
		{
			if(cascade.cards.Count > index)
			{
				if(cascade.cards[index].isFacingUp)
				{
					somethingHappened = cascade.MoveToBottom(cascade.cards[index]);		
				}
			}
		}
		PlaceholderManager.manager.UnHighlightAll();
		return somethingHappened;
	}
		
}
