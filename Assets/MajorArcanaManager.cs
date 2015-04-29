using UnityEngine;
using System.Collections;

public class MajorArcanaManager : MonoBehaviour {
	public static MajorArcanaManager manager;
	public Transform activePosition;
	public Transform inactivePosition;
	public bool isArcanaVisible = false;
	
	// Use this for initialization
	void Start () {
		manager = this;
	}
	
	public void Show()
	{
		transform.position = activePosition.position;
		isArcanaVisible = true;
	}
	public void Hide()
	{
		transform.position = inactivePosition.position;
		isArcanaVisible = false;
	}
}
