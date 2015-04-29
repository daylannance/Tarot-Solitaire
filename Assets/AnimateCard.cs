using UnityEngine;
using System.Collections;

public class AnimateCard : MonoBehaviour {
	Animator anim;
	int bendTrigID;
	// Use this for initialization
	void Start () {
	 anim = GetComponent<Animator>();
	 bendTrigID = Animator.StringToHash("BendTrigger");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Space))
		{
			anim.SetTrigger (bendTrigID);
		}
	}
}
