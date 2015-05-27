using UnityEngine;
using System.Collections;

//this class is for if the collider is not
//on the same object as a script that needs to
//detect clicks. It can just get this component as
//and subscribe to 'mouseClick'.
public class ClickDetector : CustomMouse {
	public MouseClickEventHandler mouseClick;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	override public void Clicked(MouseEvent evt)
	{
		mouseClick.Invoke(evt);	
	}
}