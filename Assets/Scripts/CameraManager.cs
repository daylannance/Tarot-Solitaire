using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum View{ Far, Middle, Closeup}
public class CameraManager:MonoBehaviour
{
	[HideInInspector]
	public View view = View.Far;
	delegate void EmptyDelegate();
	EmptyDelegate Move;
	public static CameraManager manager;
	Vector3 targetPosition;
	Camera cam;
	public bool isZoomed = false;
	Vector3 defaultPosition;
	public float springConstant;
	int middleFitHeight = 10;
	int closeupFitHeight = 4;
	

	// Use this for initialization
	void Start () {
		if(manager == null)
		{
			manager=this;
			DontDestroyOnLoad(this);
		}
		else 
		{
			//don't destoy the gameObject this is attached to.
			Destroy (this);		
		}
		defaultPosition = transform.position;
		cam = GetComponent<Camera>();
		CursorControl.cursor.Overstepped += new OverstepEventHandler(OnCursorOverstep);
		CursorControl.cursor.MouseClicked += new MouseClickEventHandler(OnMouseClick);
		Move = ()=>{};
	}
	
	void OnCursorOverstep(Vector2 amount)
	{
		//pan camera with cursor if it overstepped
		
		Vector3 cameraMovement = Vector3.zero;
		switch(view)
		{
			case View.Far:
				cameraMovement = new Vector3(-amount.x, 0f, -amount.y);
				break;
			case View.Middle:	
				cameraMovement = new Vector3(-amount.x, 0f, -amount.y);
				break;
			case View.Closeup:
				cameraMovement = new Vector3(-amount.x, 0f, -amount.y);
				break;
		}
		transform.position += cameraMovement;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Move();
	}
	void NextView(int direction)
	{
		// -1 to zoom out,
		// 1 to zoom in,
		// 0 to cycle
		if(direction > 0)
		{
			switch(view)
			{
				case View.Far:
					view = View.Middle;
					break;
				case View.Middle:
					view = View.Closeup;
					break;
				case View.Closeup:
					//already closest, do nothing
					break;
			}
		}
		if(direction < 0) 
		{
			switch(view)
			{
			case View.Far:
				//already farthest, do nothing.
				break;
			case View.Middle:
				view = View.Far;
				break;
			case View.Closeup:
				view = View.Middle;
				break;
			}	
		}
		if(direction == 0)
		{
			switch(view)
			{
			case View.Far:
				view = View.Middle;
				break;
			case View.Middle:
				view = View.Closeup;
				break;
			case View.Closeup:
				view = View.Far;
				break;
			}	
		}
	}
	public void ZoomToFit(float height, Vector3 pos)
	{		
		var targetDistance = CameraUtility.GetDistanceOfFrustumHeight(height, cam);
		Vector3 vectorToPos = pos - transform.position;
		Vector3 directionToPos = vectorToPos.normalized;
		Vector3 vectorToDesiredDistance = targetDistance * directionToPos;
		Vector3 diffVector =  vectorToPos - vectorToDesiredDistance;
		targetPosition = transform.position + diffVector;	
		Move = Spring;
	}
	void Spring()
	{
		var distanceToTarget = targetPosition - transform.position;
		if(distanceToTarget.magnitude <  1f)
		{
			//set delegate empty, the idea is that it might
			//be faster than checking a boolean on every frame.
			Move = ()=>{};
			return;
		}
		var fractionalDistance = distanceToTarget/springConstant;
		transform.position += fractionalDistance;
	}
	void OnMouseClick(MouseEvent evt)
	{
		if(evt.button == MouseButton.Middle)
		{
			NextView (0);
			switch(view)
			{
			case View.Far:
				targetPosition = defaultPosition;
				Move = Spring;	
				break;
			case View.Middle:
				ZoomToFit(10,evt.hit.point);
				break;
			case View.Closeup:
				ZoomToFit(5,evt.hit.point);
				break;		
			}
		}
	}	
}
