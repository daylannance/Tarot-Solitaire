using UnityEngine;
namespace XBoxController
{
	public class Axis:AbstractControl,IControl
	{
		
		public Axis(XBoxControlsEnum typeEnum, string binding, Polarity polarity = Polarity.NA)
			:base(typeEnum,binding,polarity){}
		
		
		override public void Update()
		{
			justPressed = false;
			justReleased = false;
			float lastValue = currentValue;
			currentValue = Input.GetAxisRaw(binding);
			changeSinceLastFrame = currentValue - lastValue;
			if(Mathf.Abs(currentValue) > Mathf.Epsilon)
			{
				if(!isDown)
				{
					justPressed = true;
					isDown = true;
				}
			}
			else
			{
				if(isDown)justReleased = true;
				isDown = false;		
			}
		}
		bool IControl.GetButtonDown()
		{
			return justPressed;
		}
		bool IControl.GetButtonUp()
		{
			return justReleased;
		}
		float IControl.GetValue(bool withPolarity)
		{
			return Input.GetAxisRaw (binding);
		}
		float IControl.GetMovementValue(bool withPolarity)
		{
			if(withPolarity) return changeSinceLastFrame;
			else return Mathf.Abs (changeSinceLastFrame);
		}
		
	}
}