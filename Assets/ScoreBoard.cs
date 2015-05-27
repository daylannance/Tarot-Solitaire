using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour {
	public static ScoreBoard board;
	Text scoreText;
	int points = 0;
	int pointsToAdd;
	// Use this for initialization
	void Start () {
		board = this;
		scoreText=GetComponent<Text>();
		scoreText.text = "PTS " + points;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public static void AddPoints(int pts)
	{
		board.pointsToAdd += pts;
		board.StartCoroutine("AnimateAddPoints");
	}
	static void UpdateScore()
	{
		board.scoreText.text = "PTS " + board.points;
	}
	IEnumerator AnimateAddPoints()
	{
		for(int i = 0; i < pointsToAdd; i++)
		{
			points += 1;
			UpdateScore ();
			yield return new WaitForSeconds(.3f);
		}
		pointsToAdd = 0;
	}
}
