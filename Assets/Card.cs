using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

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
	public Transform coinTransform;
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
	public Vector3 targetPosition;
	public Color pictureColor, UpperLeftColor, UpperRightColor;
	public Color edgeColor;
	int flipUp, flipDown;
	public bool wasUsed = false;
	public Sinwave glowWave;
	public CardStatus status = CardStatus.NotUpsideDown;
	public CoinMounter mounter;
	public Transform coinMounterTransform;
	public float GetCoinMountZ()
	{
		return targetPosition.z + coinMounterTransform.position.z -transform.position.z;
	}
	override public bool isHighlighted
	{ 
		
		get{ return _isHighlighted;}
		set
		{
			if(value)
			{
				if(isFacingUp)
				{
					CardManager.cardManager.DisplayCardInfo(ToString(), GetComponent<Renderer>().materials[CardManager.cardManager.materialIDs["Picture"]].mainTexture);
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
			GetComponent<Renderer>().materials[materialColors["UpperRight"].index].color = Color.magenta * 1.5f;
		}
		else
		{
			GetComponent<Renderer>().materials[materialColors["UpperRight"].index].color = Color.red * 1.5f;//materialColors["UpperRight"].normalColor * 1.5f;
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
		audioSource = GetComponent<AudioSource>();
		id=idCount++;
		flipUp = Animator.StringToHash("FlipFaceUpTrigger");
		flipDown = Animator.StringToHash ("FlipFaceDownTrigger");
		width = gameObject.GetComponent<Renderer>().bounds.size.x;
		height = gameObject.GetComponent<Renderer>().bounds.size.y;
		depth = gameObject.GetComponent<Renderer>().bounds.size.z;
		glowWave = new Sinwave(1.5f,1f);
		if(materialColors == null)
		{
			DefineInitialColors();
		}
		isInZodiacSequence = false;
	}
	// Use this for initialization
	public virtual void Start () {
		
		
	}
	void DefineInitialColors()
	{
		materialColors = new Dictionary<string,MaterialInfo>{
			{ "CenterOfCardBack"	, new MaterialInfo{ index = 6, normalColor = GetComponent<Renderer>().materials[6].color}},
			{ "CardBackMain"		, new MaterialInfo{ index = 5, normalColor = GetComponent<Renderer>().materials[5].color}},
			{ "Picture" 			, new MaterialInfo{ index = 2, normalColor = GetComponent<Renderer>().materials[2].color}},
			{ "Edges"				, new MaterialInfo{ index = 1, normalColor = GetComponent<Renderer>().materials[1].color}},
			{ "PictureBorder"		, new MaterialInfo{ index = 2, normalColor = GetComponent<Renderer>().materials[2].color}},
			{ "UpperLeft"			, new MaterialInfo{ index = 6, normalColor = GetComponent<Renderer>().materials[6].color}},
			{ "CardBackBorder"		, new MaterialInfo{ index = 7, normalColor = GetComponent<Renderer>().materials[6].color}},
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
		audioSource.Play ();
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
			GetComponent<Renderer>().materials[materialColors["UpperLeft"].index].color = color;
		}
		else {
			Color color = materialColors["UpperLeft"].normalColor;
			GetComponent<Renderer>().materials[materialColors["UpperLeft"].index].color = color;	
		}	
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
		Vector3 diff = targetPosition - transform.parent.localPosition;
		Vector3 step = diff * (Time.deltaTime/seconds); 
		float numberOfSteps = diff.magnitude/step.magnitude;
		for(int i = 0; i < numberOfSteps; i++)
		{
			transform.parent.localPosition += step;
			yield return null;
		}
		transform.parent.localPosition = targetPosition;
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
