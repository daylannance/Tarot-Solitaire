using UnityEngine;

namespace XBoxController
{
	public class AxisButton:AbstractControl, IControl
	{
		bool _isDown;

		public AxisButton(XBoxControlsEnum typeEnum, string binding, Polarity polarity = Polarity.NA)
		:base(typeEnum,binding,polarity)
		{
			this.polarity = polarity;
		}
		
		override public bool isDown 
		{
			get{ return _isDown;}
			protected set{ _isDown = value;}
		}
		void IControl.Update()
		{
			
			justPressed = false;
			justReleased = false;
			var lastValue = currentValue;
			//I have to do some things to the value first.
			//namely, if polarity is negative, but there is
			//a positive value, current value should then be zero
			float tempValue = Input.GetAxis(binding);
			changeSinceLastFrame = tempValue - lastValue;
			switch(polarity)//we only want one side of the axis for an AxisButton
			{
			case Polarity.Negative:
				if(Input.GetAxis (binding) < 0)
				{
					if(!isDown)//if it wasn't down last update
					{
						justPressed = true;
						isDown = true;
					}
				}
				else 
				{
					tempValue = 0;
				} //button goes only in one direction
				break;
			case Polarity.Positive:
				if(tempValue > 0)
				{
					if(!isDown)
					{
						justPressed = true;
						isDown = true;		
					}
				}
				else 
				{
					tempValue = 0;
				}
				break;
			}
			if(tempValue == 0)//we might have just set it to zero, or it may still be zero
			{
				if(isDown)//it was down last update
				{
					justReleased = true;		
				}
				isDown = false;
			}
			currentValue = tempValue;
		}
		bool IControl.GetButtonDown()
		{
			return justPressed;
		}
		bool IControl.GetButtonUp()
		{
			return justReleased;
		}
		float IControl.GetValue(bool withPolarity = false)
		{
			if(withPolarity)
			{
				return currentValue;
			}
			else return Mathf.Abs (currentValue);
		}
		float IControl.GetMovementValue(bool withPolarity)
		{
			if(withPolarity) return changeSinceLastFrame;
			else return Mathf.Abs (changeSinceLastFrame);
		}
	}
}
