using XBoxController;
using System;
using UnityEngine;

namespace XBoxController
{
	public abstract class AbstractControl:IControl
	{
		protected bool justPressed;
		protected bool justReleased;
		protected float changeSinceLastFrame = 0;
		protected string binding;
		public virtual bool isDown {get; protected set;}
		public float currentValue { get; protected set;}
		public XBoxControlsEnum controlsEnum { get; protected set;}
		public Polarity polarity;

		public AbstractControl(XBoxControlsEnum controlsEnum, string binding, Polarity polarity = Polarity.NA)
		{
			this.controlsEnum = controlsEnum;
			this.binding = binding;
			state = IControlState.AtRest;
			controlEvent += (x)=>{};
		}
	
		public virtual bool GetButtonDown(){ return justPressed;}
		public virtual bool GetButtonUp(){ return justReleased;}
		public virtual void Update()
		{
		}
		public float GetValue(bool withPolarity)
		{ 
			if(withPolarity) return currentValue;
			else return Mathf.Abs (currentValue);
		}
		public float GetMovementValue(bool withPolarity)
		{ 
			if(withPolarity) return changeSinceLastFrame;
			else return Mathf.Abs (changeSinceLastFrame);
		}
		public XBoxControlEvent controlEvent { get; protected set;}
		public IControlState state { get; protected set;}
		public virtual void Dispatch()
		{
			switch(state)
			{
				case IControlState.JustPressed:	
					controlEvent.Invoke(this);
					Debug.Log (Enum.GetName(typeof(XBoxControlsEnum), controlsEnum)+ " " + Enum.GetName(typeof(IControlState),state));
					break;
				case IControlState.JustReleased:
					controlEvent.Invoke (this);
					Debug.Log (controlsEnum + " " + state);
					break;
				case IControlState.StillDown:
					controlEvent.Invoke (this);
					break;	
			}
		}
	}
}

