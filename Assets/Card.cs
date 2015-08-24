using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public enum AnimationMode{ AscendingChain, DescendingChain, ConstantSpeed}
public enum CardStatus { UpsideDown, NotUpsideDown}
public struct MaterialInfo
{
	public int index;
	public Color normalColor;
	public Color highlightColor;
	public Color blinkColor;
}
public class Card : CustomMouse {
	AudioSource audioSource;
	public float width, height, depth;
	public Rank rank;
	public Suit suit;
	public Rank substituteRank = Rank.NotSpecified;
	public Suit substituteSuit = Suit.NotSpecified;
	public Zodiac substituteZodiac = Zodiac.None;
	public int points = 0;
	public int tempPoints = 0;
	public Zodiac zodiac;
	public Animator anim;
	public Placeholder parentPlaceholder;
	public Placeholder previousParentPlaceholder = null;
	public Vector3 targetPosition;
	public Color pictureColor, UpperLeftColor, UpperRightColor;
	public Color edgeColor;
	int flipUp, flipDown;
	public bool wasUsed = false;
	public CustomHelpers.Sinwave glowWave;
	public CardStatus status = CardStatus.NotUpsideDown;
	public CoinStash coinStash;
	public bool isBeingTransfered = false;
	public bool isAnimating = false;
	public Renderer renderer;
	public Card parentCard, childCard;
	[HideInInspector]
	protected ClickDetector clickDetector;
	
	
	override public bool isHighlighted
	{ 
		
		get{ return _isHighlighted;}
		set
		{
			if(value)
			{
				if(isFacingUp)
				{
					CardManager.cardManager.DisplayCardInfo(ToString(), renderer.materials[CardManager.cardManager.materialIDs["Picture"]].mainTexture);
				}
				else 
				{
					CardManager.cardManager.DisplayCardInfo("",null);
				}
				_isHighlighted = value;
			}					
		}
	}
	override public void Highlight(Color color)
	{
		GetComponent<Renderer>().materials[materialColors["Picture"].index].color = color * 1.5f;
		isHighlighted = true;
	}
	public Destinations destinations;
	public bool isGlowing = false;
	public bool isFacingUp = false;
	public static int idCount = 0;
	public int id;
	public int coins;
	public int clicks = 0;
	public Color glowColor = new Color(.5f,1.5f,10f);
	private bool _isInZodiacSequence;
	public bool isInZodiacSequence
	{
		get
		{
			return _isInZodiacSequence;
		}
		set	
		{
			_isInZodiacSequence = value;
			SetIlluminateZodiacSymbol(value);
		}
	}
	void SetIlluminateZodiacSymbol(bool on)
	{
		if(on)
		{
			renderer.materials[materialColors["UpperRight"].index].color = Color.magenta * 1.5f;
		}
		else
		{
			renderer.materials[materialColors["UpperRight"].index].color = Color.red * 1.5f;//materialColors["UpperRight"].normalColor * 1.5f;
		}
	}
	override public void RecordUndo()
	{
		if(parentPlaceholder != null)
		{
			parentPlaceholder.RecordUndo ();
		}
	}
	public static Dictionary<string,MaterialInfo> materialColors;
	public virtual void Awake()
	{
		renderer=transform.GetComponentInChildren<Renderer>();
		audioSource = GetComponent<AudioSource>();
		id=idCount++;
		flipUp = Animator.StringToHash("FlipFaceUpTrigger");
		flipDown = Animator.StringToHash ("FlipFaceDownTrigger");
		width = GetComponentInChildren<Renderer>().bounds.size.x;
		height = GetComponentInChildren<Renderer>().bounds.size.y;
		depth = GetComponentInChildren<Renderer>().bounds.size.z;
		glowWave = new CustomHelpers.Sinwave(1.5f,1f);
		if(materialColors == null)
		{
			DefineInitialColors();
		}
		isInZodiacSequence = false;
		destinations = new Destinations(transform,this);
	}
	// Use this for initialization
	public virtual void Start () {
		clickDetector = GetComponentInChildren<ClickDetector>();
		clickDetector.mouseClick += Clicked;
	}
	void DefineInitialColors()
	{
		materialColors = new Dictionary<string,MaterialInfo>{
			{ "CenterOfCardBack"	, new MaterialInfo{ index = 6, normalColor = renderer.materials[6].color}},
			{ "CardBackMain"		, new MaterialInfo{ index = 5, normalColor = renderer.materials[5].color}},
			{ "Picture" 			, new MaterialInfo{ index = 2, normalColor = renderer.materials[2].color}},
			{ "Edges"				, new MaterialInfo{ index = 1, normalColor = renderer.materials[1].color}},
			{ "PictureBorder"		, new MaterialInfo{ index = 2, normalColor = renderer.materials[2].color}},
			{ "UpperLeft"			, new MaterialInfo{ index = 6, normalColor = renderer.materials[6].color}},
			{ "CardBackBorder"		, new MaterialInfo{ index = 7, normalColor = renderer.materials[6].color}},
			{ "UpperRight"			, new MaterialInfo{ index = 3, normalColor = UpperRightColor * 1.5f}}};
	}
	public void FlipDown()
	{
		
		anim.SetTrigger("FlipFaceDownTrigger");
		isFacingUp = false;
	}
	public void FlipUp()
	{
		UndoManager.manager.haveAnyCardsMoved = true;
		//audioSource.Play ();
		anim.SetTrigger(flipUp);
		isFacingUp = true;
	}
	public void Glow(Color color)
	{
		isGlowing = true;
		glowColor = color;
	}
	override public void MouseOver()
	{
		switch(Game.game.state)
		{
			case GameState.SelectingARow:
				if(parentPlaceholder is Cascade)
				{
					int row = parentPlaceholder.cards.IndexOf (this);
					PlaceholderManager.manager.HighlightRow(row);
				}
				break;
			default:
				CardManager.currentHighlighted = this;
				break;
		}
		
	}
	override public void MouseExit()
	{
		CardManager.currentHighlighted = null;
		UnHighlight();
	}
	override public void Clicked(MouseEvent evt)
	{
		base.Clicked(evt);
		if(isAnimating) return;
		if(evt.button == MouseButton.Left)
		{
			parentPlaceholder.OnCardClicked(this);
		}
	}
	public virtual void OnSelected()
	{

	}
	void Update () {
		if(Input.GetKeyDown (KeyCode.U))
		{
			anim.SetTrigger(flipUp);
		}
		if(Input.GetKeyDown (KeyCode.D))
		{
			anim.SetTrigger (flipDown);
		}
	}
	override public string ToString()
	{
		return "Card: " + rank.ToString() + " of " + suit.ToString() + ", Zodiac: " + zodiac.ToString ();
	}
	void FixedUpdate()
	{
		if(isGlowing)
		{
			Color color = glowColor;	
			color *= glowWave.NextValue ();
			//renderer.materials[CardManager.cardManager.materialIDs["Edges"]].color = color;
			renderer.materials[materialColors["UpperLeft"].index].color = color;
		}
		else {
			Color color = materialColors["UpperLeft"].normalColor;
			renderer.materials[materialColors["UpperLeft"].index].color = color;	
		}
		destinations.Update();
	}
	override public void UnHighlight()
	{
		GetComponent<Renderer>().materials[materialColors["Picture"].index].color = materialColors["Picture"].normalColor;
		if(!isGlowing)
		{
			GetComponent<Renderer>().materials[materialColors["UpperLeft"].index].color = materialColors["UpperLeft"].normalColor;
		}
		isHighlighted = false;
	}
	public Card Previous()
	{
		int index = parentPlaceholder.cards.IndexOf(this);
		if(index > 0) return parentPlaceholder.cards[index -1];
		else return null;
	}
	public Card Next()
	{
		int index = parentPlaceholder.cards.IndexOf(this);
		if(parentPlaceholder.cards.Count > (index + 1))
		{
			return parentPlaceholder.cards[index + 1];
		}
		else return null;
		
	}
	public IEnumerator AnimateMove(Vector3 targetPosition, float seconds)
	{
		isAnimating = true;
		Vector3 diff = targetPosition - transform.position;
		Vector3 step = diff * (Time.deltaTime/seconds); 
		float numberOfSteps = diff.magnitude/step.magnitude;
		for(int i = 0; i < numberOfSteps; i++)
		{
			transform.position += step;
			yield return null;
		}
		transform.position = targetPosition;
		isAnimating = false;
		yield return null;
	}
	public void RemoveSelfFromParent()
	{
		if(parentPlaceholder && parentPlaceholder.cards.Contains(this))
		{
			parentPlaceholder.cards.Remove(this);
		}
	}
	
}
public enum Rank
{
	Ace,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Ten,
	Page,
	Knight,
	Queen,
	King,
	NotSpecified
}
