using UnityEngine;
using System.Collections.Generic;
using XBoxController;

namespace XBoxController
{
	
	public delegate void XBoxControlEvent(IControl control);
	public class XBoxControllerMonoBehavior : MonoBehaviour {
		[HideInInspector]
		public static XBoxControllerMonoBehavior controller;
		[HideInInspector]
		public static event XBoxControlEvent ButtonDown;
		[HideInInspector]
		public static event XBoxControlEvent ButtonUp;

		bool isDpad_Left_Down, isDpad_Right_Down, isDpad_Up_Down, isDpad_Down_Down;

		public static Dictionary<XBoxControlsEnum,IControl> controls =
			new Dictionary<XBoxControlsEnum, IControl>();
			
		// Use this for initialization
		void Awake() 
		{	
			InitializeControlsDictionary ();
			if(controller == null)
			{
				controller = this;
				DontDestroyOnLoad(this);
			}
			else 
			{
				Destroy (this);	
			}	
		}
		
		// Update is called once per frame
		void Update () {
			foreach(var control in controls.Values)
			{
				control.Update ();
				control.Dispatch();
			}	
		}
		void CheckButtonDown()
		{
			//sends button down event for each control that returns true
			foreach(var key in controls.Keys)
			{
				if(controls[key].GetButtonDown())
				{
					//dispatch event, key is the XBoxControl enum
					if(ButtonDown != null)ButtonDown(controls[key]);
					Debug.Log (key.ToString () + " pressed, value = " + controls[key].GetValue(false));
				}
			}
		}
		void CheckButtonUp()
		{
			foreach(var key in controls.Keys)
			{
				if(controls[key].GetButtonUp())
				{
					//dispatch event, key is the XBoxControl enum
					if(ButtonUp != null) ButtonUp(controls[key]);
					Debug.Log (key.ToString () + " released");
				}
			}
		}
		public static IControl GetControl(XBoxControlsEnum controlEnum)
		{
			return controls[controlEnum];
		}
		public static void AddControl(IControl control)
		{
			
			controls.Add (control.controlsEnum, control);
		}
		public static void InitializeControlsDictionary()
		{
				controls = new Dictionary<XBoxControlsEnum,IControl>();	
				AddControl(new Button(XBoxControlsEnum.A, "A"));
				AddControl(new Button(XBoxControlsEnum.B, "B"));
				AddControl(new Button(XBoxControlsEnum.X, "X"));
				AddControl(new Button(XBoxControlsEnum.Y, "Y"));
				AddControl(new Button(XBoxControlsEnum.RB,"RB"));
				AddControl(new Button(XBoxControlsEnum.LB,"LB"));
				AddControl(new Button(XBoxControlsEnum.LeftThumbButton,"LeftThumbButton"));
				AddControl(new Button(XBoxControlsEnum.RightThumbButton,"RightThumbButton"));
				AddControl(new Axis(XBoxControlsEnum.D_PAD_X, "D_PAD_X"));
				AddControl(new Axis(XBoxControlsEnum.D_PAD_Y, "D_PAD_Y"));
				AddControl(new AxisButton(XBoxControlsEnum.LT,"LT",Polarity.Positive));
				AddControl(new AxisButton(XBoxControlsEnum.RT,"RT",Polarity.Negative));
				AddControl(new Axis(XBoxControlsEnum.LeftThumb_X,"LeftThumbX"));
				AddControl(new Axis(XBoxControlsEnum.LeftThumb_Y,"LeftThumbY"));
				AddControl(new Axis(XBoxControlsEnum.RightThumb_X,"RightThumbX"));
				AddControl(new Axis(XBoxControlsEnum.RightThumb_Y,"RightThumbY"));
			
		}
		
	}
}
