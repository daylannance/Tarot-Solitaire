using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XBoxController;


public class CameraAngle : MonoBehaviour {
	public List<float> cameraAngles;
	bool givingItTime = false;
	int cameraAngleIndex = 2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
	}
	void Update()
	{
		
	}
	void OnGUI()
	{
		if(Event.current.type == EventType.scrollWheel||
			XBoxControllerMonoBehavior.controls[XBoxControlsEnum.D_PAD_Y].GetButtonDown()
			&& !givingItTime
			)
		{
			if(Event.current.delta.y > 0 || (XBoxControllerMonoBehavior.controls[XBoxControlsEnum.D_PAD_Y].GetValue (true) > .9f) && XBoxControllerMonoBehavior.controls[XBoxControlsEnum.D_PAD_Y].GetButtonDown())
			{
				cameraAngleIndex += 1;
				if(cameraAngleIndex >= cameraAngles.Count)
				{
					cameraAngleIndex = cameraAngles.Count-1;
				}
				StartCoroutine ("GiveItSomeTime");
			}
			if(Event.current.delta.y < 0  ||(XBoxControllerMonoBehavior.controls[XBoxControlsEnum.D_PAD_Y].GetValue (true) < -.9f) && XBoxControllerMonoBehavior.controls[XBoxControlsEnum.D_PAD_Y].GetButtonDown())
			{
				cameraAngleIndex -= 1;
				if(cameraAngleIndex < 0)
				{
					cameraAngleIndex = 0;
				}
				StartCoroutine ("GiveItSomeTime");
			}
			transform.rotation = Quaternion.AngleAxis(cameraAngles[cameraAngleIndex],new Vector3(1,0,0));
		}
	}
	void OnButtonPress(){}
	IEnumerator GiveItSomeTime()
	{
		givingItTime = true;
		yield return new WaitForSeconds(0.1f);
		givingItTime = false;
	}
	
	
}
