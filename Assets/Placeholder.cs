using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public delegate void AddCardDelegate(Card card);
public delegate IEnumerator CoroutineWithArgs(Dictionary<string,object> parameters);
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
	public List<Card> tempList = new List<Card>(); //for passing cards between coroutines
	protected Queue<CoroutineArgs> animations = new Queue<CoroutineArgs>();
	public static bool isAnimating = false;

	
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
	public virtual void FixedUpdate()
	{
		if(!isAnimating && animations.Count > 0)
		{
			var routine = animations.Dequeue();
			StartCoroutine (routine.routineDelegate(routine.parameters));
		}
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
	public List<Card> GetCardAndChildren(Card card)
	{
		var index = cards.IndexOf (card);
		return cards.GetRange(index, cards.Count - index);
	}
	public List<Card> GetFaceUpCards()
	{
		List<Card> result = new List<Card>();
		foreach(var card in cards)
		{
			if(card.isFacingUp) result.Add(card);
		}
		return result;
	}
	public bool CheckNumericOrder(List<Card> list, int increment)
	{
		Card previousCard = null;
		bool result = true; //innocent until proven guilty
		foreach(var card in list)
		{
			if(previousCard != null)
			{
				if(previousCard.rank != card.rank + increment)
				{
					result = false;
					break;
				}
			}
			previousCard = card;	
		}
		
		return result;	
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
	protected virtual IEnumerator AnimateRemoveCoins()
	{
		if(cards.Count == 0) yield return null;
		//enforces serial execution of animation cue:
		isAnimating = true; 
		int quantityToRemove = 0; //more coins the more rapidly the coins are removed
		List<Card> cardsBeingRemoved = new List<Card>();
		//find the cards that are being removed, all parents should be changed already:
		foreach (var card in cards)
		{
			if(card.isBeingTransfered)
			{
				cardsBeingRemoved.Add (card);	
			}
		}
		//count how many coins we are removing:
		if(cardsBeingRemoved.Count > 0)
		{
			foreach(var card in cardsBeingRemoved)
			{	
				quantityToRemove += card.coinStash.coins.Count;
			}
		}
		if(quantityToRemove > 0)
		{
			//the more coins, the less the delay
			float delay = 1f/quantityToRemove; 
			//using for loop because I get a 'modified while iterating' error when I use a foreach
			for(int i=0; i<cardsBeingRemoved.Count; i++)
			{
				//keep removing coins from card until count is zero
				while(cardsBeingRemoved[i].coinStash.coins.Count > 0)
				{
					cardsBeingRemoved[i].coinStash.RemoveCoin();
					//yield return new WaitForSeconds(delay);	
				}
			}
		}
		//allow next animation to execute:
		isAnimating = false;
		yield return null;
	}
	public virtual bool ApplyRules(List<Card> faceupCards, List<Card> newCards)
	{
		return false;
	}
	
	public virtual void AddCards(List<Card> list)
	{	
		//if we are dealing, we put each card in its own list
		//we don't want to animate until all cards are added.
		//so we want to keep adding to tempList instead of 
		//clearing it.
		if(Game.game.state != GameState.Dealing)
		{
			tempList.Clear ();
		}
		//so we know what cards were added so we can later animate them:
		foreach(var card in list)
		{
			tempList.Add(card);
		}
		UndoManager.manager.haveAnyCardsMoved = true;
		Placeholder previousParent = null;
		if(list.Count > 0)
		{
			previousParent = list[0].parentPlaceholder;
		}
		foreach(var card in cards)
		{
			card.isBeingTransfered = false;
		}
		foreach(var card in list)
		{
			card.isBeingTransfered = true;
			//card.isAnimating = true;
			Parent (card);
			SetCardTransformPosition(card);
		}
		//CheckZodiacSequences();
		if(previousParent != null)
		{
			//previousParent.CheckZodiacSequences();
		}
		
	}
	protected virtual IEnumerator AnimateAddCoins(Dictionary<string,object> parameters)
	{
		yield return null;
	}
	public void AnimateMoveCards()
	{
		Dictionary<string,object> upParameters = new Dictionary<string,object>();
		upParameters.Add("reverse",true);
		Dictionary<string,object> downParameters = new Dictionary<string,object>();
		downParameters.Add("reverse",true);
		animations.Enqueue(new CoroutineArgs(AnimateCardsUpAndOver, upParameters));
		animations.Enqueue(new CoroutineArgs(AnimateCardsDown, downParameters));		
	}
	//
	public void AnimateChainTopToBottom()
	{
		if(tempList.Count > 0)
		{
			var top = tempList[0];
		}
		foreach(var card in tempList)
		{

		}
	}
	public void Parent(Card card)
	{
		if(card.parentPlaceholder != null)
		{
			card.previousParentPlaceholder = card.parentPlaceholder;
			card.RemoveSelfFromParent();
			card.renderer.sortingLayerID = this.GetComponent<Renderer>().sortingLayerID;
		}
		cards.Add (card);
		card.parentPlaceholder = this;
	}
	public virtual void SetCardTransformPosition(Card card)
	{
		if(cards.Count == 1)
		{
			card.transform.parent = cardsRootTransform;
			//card.transform.parent.localPosition = Vector3.zero;
			card.targetPosition = card.transform.parent.position;
		}
		else 
		{
			card.transform.parent = cardsRootTransform;
			//card.transform.parent.localPosition = offset * (cards.Count-1);
			card.targetPosition = cardsRootTransform.position + offset * cards.IndexOf(card);
		}
		card.transform.rotation = Quaternion.AngleAxis (cardAngle, new Vector3(1,0,0));
	}
	public IEnumerator AnimateCardsUpAndOver(Dictionary<string,object> parameters)
	{
		isAnimating = true;
		float animateHeight = 4;//(cards.Count > 0)? cards[0].width * 2: 1f;
		if((bool)parameters["reverse"])tempList.Reverse ();
		if(tempList.Count > 0)
		{
			for(int i = 0; i < tempList.Count; i++)
			{
				Card card = tempList[i];
				if((card.targetPosition - card.transform.position).magnitude < .01f) continue;
				var pos = card.targetPosition;
				pos.y += animateHeight;
				card.destinations.Add (pos,4,.01f,AnimationMode.ConstantSpeed);//StartCoroutine(card.AnimateMove (pos, .01f));
				//card.FlipUp ();
				yield return new WaitForSeconds(.01f);
			}
		}
		//yield return new WaitForSeconds(.3f);
		isAnimating = false;
	}
	public IEnumerator AnimateCardsDown(Dictionary<string,object> parameters)
	{	
		isAnimating = true;
		//depending on how cards are arranged, reverse
		//can keep the cards from going through eachother
	    //while animating
		if((bool)parameters["reverse"])
		{
			tempList.Reverse();
		}
		for(int i =0; i< tempList.Count; i++)
		{
			var card = tempList[i];
			if((card.targetPosition - card.transform.position).magnitude < .01f) continue;
			card.destinations.Add(card.targetPosition,4,.01f,AnimationMode.ConstantSpeed);
			yield return new WaitForSeconds(.1f);
		}
		isAnimating = false;
	}
	public void SetParentingTopToBottom(List<Card> list)
	{
		//this is all so the cards can follow eachother in a chain.
		for(int i = 0; i < list.Count; i++)
		{
			//if there is a card after index;
			if(i+1 < list.Count)
			{
				var parent = list[i];
				var child = list[i+1];
				parent.childCard = child;
				child.parentCard = parent;
			}
		}
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
		if(isAnimating) return;
		base.Clicked (evt);
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
