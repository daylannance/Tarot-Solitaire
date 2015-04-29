using UnityEngine;
using System.Collections;
using System;

public class FlagsEnumTest : MonoBehaviour {
	[Flags]
	enum FlagsEnum
	{
		A = 1,
		B = 2,
		C = 4,
		D = 8,
		E = 16
	}
	FlagsEnum flags = FlagsEnum.A;
	int index = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.LeftBracket))
		{
			flags += 1;
			PrintFlags ();
		}
	}
	void PrintFlags()
	{
		switch(flags)
		{
			case FlagsEnum.A | FlagsEnum.B:
				Debug.Log(flags + " case: A");
				break;
			case FlagsEnum.B:
				Debug.Log(flags + " case: B");
				break;
			case FlagsEnum.C:
				Debug.Log(flags + " case: C");
				break;
			case FlagsEnum.D:
				Debug.Log(flags + " case: D");
				break;
			case FlagsEnum.E:
				Debug.Log(flags + " case: E");
				break;
			}
	}
}
