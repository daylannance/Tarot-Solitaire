using UnityEngine;

namespace XBoxController
{
	public class Button:AbstractControl, IControl	
	{
		public Button(XBoxControlsEnum typeEnum, string binding, Polarity polarity = Polarity.NA)
		:base(typeEnum,binding,polarity){}

		override public void Update()
		{
			if(GetButtonDown ())
			{
				justPressed = true;
				state = IControlState.JustPressed;
			}
			else if(Input.GetButton (binding))
			{
				state = IControlState.StillDown;
			}
			else if(GetButtonUp())
			{
				state = IControlState.JustReleased;
			}
			else state = IControlState.AtRest;
		}
		override public bool GetButtonDown()
		{
			return Input.GetButtonDown (binding);
		}
		override public bool GetButtonUp()
		{
			return Input.GetButtonUp (binding);
		}
	 
		float IControl.GetValue(bool withPolarity)
		{
			return Input.GetButton (binding)? 1:0;
		}
		float IControl.GetMovementValue(bool withPolarity)
		{
			if(Input.GetButtonDown (binding))
			{
				return 1;
			}
			else return 0;
		}
	}
}
