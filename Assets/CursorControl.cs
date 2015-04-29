using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using XBoxController;


public enum MouseButton{Left,Right,Middle, None}
public delegate void OverstepEventHandler(Vector2 amount);
public delegate void MouseMoveEventHandler(Vector2 amount);
public delegate void MouseClickEventHandler(MouseEvent evt);

public class CursorControl : Placeholder{
	// Use this for initialization
	public event OverstepEventHandler Overstepped;
	public event MouseMoveEventHandler MouseMoved;
	public Vector2 upperLimit;
	public Vector2 lowerLimit;
	public Rect lockButtonRect;
	public static CursorControl cursor;
	public ArcanaCard specialCard;
	public Transform specialCardInHandTransform;
	public float mouseSpeed = 1;
	int currentDefaultMask = 12;
	public int tableMask, cardMask, specialCardMask;
	public List<Camera> cams;
	public bool usingSystemCursor = false;
	public GameObject systemCursorSprite;
	GUITexture texture;
	public event MouseClickEventHandler MouseClicked;
	public float thumbstickSpeed;
	
	void Awake()
	{
		cursor = this;
	}
	override public void Start () {
		Game.gameStateUpdated += OnGameStateUpdate;
		tableMask = 1 << LayerMask.NameToLayer ("Board");
		cardMask = 1 << LayerMask.NameToLayer ("Cards");
		specialCardMask = 1 << LayerMask.NameToLayer ("SpecialCards");
		currentDefaultMask = cardMask;
		texture = GetComponent<GUITexture>();
		UseCustomCursor();
		
		//Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	override public void Update () {
		//Screen.lockCursor = true;
		if(Input.GetKeyDown (KeyCode.P))
		{
			ToggleUsingSystemCursor();
		}
		if(usingSystemCursor) return;
		Vector3 movement = new Vector3(XBoxControllerMonoBehavior.GetControl (XBoxControlsEnum.LeftThumb_X).GetValue (true),
										XBoxControllerMonoBehavior.GetControl(XBoxControlsEnum.LeftThumb_Y).GetValue (true),
										0) * thumbstickSpeed;
		movement += new Vector3(Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"), 0) * mouseSpeed;
		if(MouseMoved != null) MouseMoved(movement);
		Vector3 newPos = transform.position+movement;
		Vector3 camMovement;
		Vector2 overstep = Vector2.zero;
		switch(Game.game.state)
		{
			case GameState.ViewingNormalCard:
			case GameState.ViewingSpecialCard:
				newPos.y = Mathf.Clamp (newPos.y, 0, 1);
				break;
			default:
				if(!CameraManager.manager.isZoomed)
				{
					if(newPos.y > upperLimit.y)
					{
						overstep.y = newPos.y - upperLimit.y;
						newPos.y = upperLimit.y;
					}
					else if(newPos.y < lowerLimit.y)
					{
						overstep.y = newPos.y - lowerLimit.y;
						newPos.y = lowerLimit.y;
					}
					if(newPos.x > upperLimit.x)
					{
						overstep.x = newPos.x - upperLimit.x;
						newPos.x = upperLimit.x;
					}
					else if(newPos.x < lowerLimit.x)
					{
						overstep.x = newPos.x - lowerLimit.x;
						newPos.x = lowerLimit.x;
					}
					Overstepped(overstep * GetTableUnitsPerPixel());
				}
				break;
		}
		newPos.x = Mathf.Clamp (newPos.x, 0,1);
		transform.position = newPos;
		SendEventIfMouseclick();
			
	}

	void SendEventIfMouseclick()
	{
		MouseButton button = MouseButton.None;
		if(Input.GetMouseButtonDown(0)
			|| Input.GetButtonDown("X"))
		{
			button = MouseButton.Left;		
			UndoManager.manager.SaveState();
		}
		else if(Input.GetMouseButtonDown (1)
			|| Input.GetButtonDown ("B"))
		{
			button = MouseButton.Right;
		}
		else if(Input.GetMouseButtonDown (2)
			|| Input.GetButtonDown ("Y"))
		{
			button = MouseButton.Middle;
		}
		if(button != MouseButton.None)
		{
			MouseEvent evt = new MouseEvent(GetCursorPixelPosition(),
											GetRaycastHitUnderCursor(),
											button);
			MouseClicked(evt);
			var target = GetTargetUnderCursor(currentDefaultMask);
			if(target)target.Clicked (evt);
		}
	}
	RaycastHit GetRaycastHitUnderCursor()
	{
		Vector2 pos = GetCursorPixelPosition();
		RaycastHit hit = new RaycastHit();
		Ray ray = Game.game.currentCam.ScreenPointToRay (pos);
		Physics.Raycast (ray, out hit, 10000.0f, currentDefaultMask);
		return hit;
	}
	float GetTableUnitsPerPixel()
	{
		Vector2 pos = GetCursorPixelPosition();
		RaycastHit hit = new RaycastHit();
		Ray ray = Game.game.currentCam.ScreenPointToRay (pos);
		Physics.Raycast (ray, out hit, 10000.0f, LayerMask.NameToLayer("All"));
		var x1 = hit.point.x;
		pos.x += Screen.width/20;
		ray = Game.game.currentCam.ScreenPointToRay (pos);
		Physics.Raycast (ray, out hit, 10000.0f, LayerMask.NameToLayer ("All"));
		var x2 = hit.point.x;
		return Mathf.Abs(x2 - x1) * 20;
	}
	void LeftClicked()
	{
		CustomMouse target = null;
		switch(Game.game.state)
		{
			case GameState.CardInHand:
				target = GetTargetUnderCursor(cardMask);
				
				break;
			case GameState.HandEmpty:
				target = GetTargetUnderCursor(cardMask);
			
				break;
			case GameState.SelectingSpecialCard:
				target = GetTargetUnderCursor(specialCardMask);
				//if(target != null) target.Clicked();
				break;
			case GameState.SpecialCardInHand:
				target = GetTargetUnderCursor(cardMask);
				//if(target != null) target.Clicked();
				break;		
			case GameState.SelectingARow:
				target = GetTargetUnderCursor(cardMask);
				//if(target != null) target.Clicked();
				break;				
		}
	}
	
	
	public void ToggleUsingSystemCursor()
	{
		if(usingSystemCursor)
		{
			UseCustomCursor();
		}
		else 
		{
			UseSystemCursor();
		}
		
	}
	public void UseSystemCursor()
	{
		usingSystemCursor = true;
		Screen.lockCursor = false;
		Cursor.visible = false;	
		texture.enabled = false;
		systemCursorSprite.SetActive(true);	
	}
	public void UseCustomCursor()
	{
		usingSystemCursor = false;
		Screen.lockCursor = true;
		Cursor.visible = false;	
		texture.enabled = true;
		systemCursorSprite.SetActive(false);
		transform.position = Game.game.currentCam.ScreenToViewportPoint(Input.mousePosition);
	}
	public CustomMouse GetTargetUnderCursor(int layerMask)
	{
		GameObject obj;
		CustomMouse target = null;
		for(int i = 0; i < cams.Count; i++)
		{
			Vector2 pixelPos = new Vector2(transform.position.x * Screen.width, transform.position.y * Screen.height);
			if(RaycastFromMouse (out obj, pixelPos,layerMask))
			{	
				if(obj != null)
				{
					target = obj.GetComponent<CustomMouse>(); 
				}
			}	
		}
		return target;		
	}
	public Vector3 GetWorldPositionUnderCursor(Camera cam)
	{
		Vector2 pixelPos = GetCursorPixelPosition();
		Ray ray = cam.ScreenPointToRay (pixelPos);
		RaycastHit hit;
		Vector3 pos;
		if (Physics.Raycast (ray, out hit, 10000.0f, tableMask))
		{
			pos = hit.point;
		}
		else 
		{
			pos = Vector3.zero;
		}
		return pos;
	}
	void OnGameStateUpdate(GameState state)
	{
		switch(state)
		{
		case GameState.CardInHand:
			currentDefaultMask = 1 <<	LayerMask.NameToLayer("Cards");
			break;
		case GameState.HandEmpty:
			currentDefaultMask = 1 <<	LayerMask.NameToLayer("Cards");
			break;
		case GameState.SelectingSpecialCard:
			currentDefaultMask =	1 << LayerMask.NameToLayer("SpecialCards") ;
			break;
		case GameState.SpecialCardInHand:
			currentDefaultMask =	1 << LayerMask.NameToLayer("Cards");
			break;
		}
	}
	Vector2 GetCursorPixelPosition()
	{
		return new Vector2(transform.position.x * Screen.width, transform.position.y * Screen.height);
	}
	void FixedUpdate()
	{
		GameObject obj;
		if(usingSystemCursor) return;	
		for(int i = 0; i < cams.Count; i++)
		{
			Vector2 pixelPos = GetCursorPixelPosition();
			try{
				if(RaycastFromMouse (out obj, pixelPos,currentDefaultMask))
				{
				
					var target = obj.GetComponent<CustomMouse>();
					if(target != null)
					{
						target.MouseOver();
					}
					
				}
				else 
				{
					if(CardManager.currentHighlighted != null)
					{
						CardManager.currentHighlighted.UnHighlight();
						CardManager.currentHighlighted = null;
					}
				}
			}catch(UnityException e){throw new UnityException("Something happened when the custom mouse raycasted. Just sayin." + e.Message);}
		}
		switch(Game.game.state)
		{
			case GameState.SelectingARow:
			case GameState.SpecialCardInHand:
				Vector2 pixelPos = new Vector2(transform.position.x * Screen.width, transform.position.y * Screen.height);
				RaycastHit hit = new RaycastHit();
				foreach(var cam in cams)
				{
					Ray ray = cam.ScreenPointToRay (pixelPos);
					if (Physics.Raycast (ray, out hit, 10000.0f, tableMask))
					{
						Vector3 pos = cardsRootTransform.position;
						pos.x = hit.point.x;
						pos.z = hit.point.z;
						cardsRootTransform.position = pos;
					}
				}
			break;
		}
	}
	
	bool RaycastFromMouse(out GameObject obj, Vector2 pos, int layerMask)
	{
		obj = null;
		RaycastHit hit = new RaycastHit();
		foreach(var cam in cams)
		{
			Ray ray = cam.ScreenPointToRay (pos);
			if (Physics.Raycast (ray, out hit, 10000.0f, layerMask))
			{
				obj = hit.collider.gameObject;
			}
		}
		return obj? true:false;
	}
	public void ToggleLockCursor()
	{
		Screen.lockCursor = !Screen.lockCursor;
	}
	
}
