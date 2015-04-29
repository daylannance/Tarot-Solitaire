using UnityEngine;
using System.Collections;

public class CardFlipTest : MonoBehaviour {
	Animator anim;
	int flipUp, flipDown;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		flipUp = Animator.StringToHash("FlipFaceUpTrigger");
		flipDown = Animator.StringToHash ("FlipFaceDownTrigger");
	}
	
	// Update is called once per frame
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
}
