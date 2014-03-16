using UnityEngine;
using System.Collections;

public class WinningConditions : MonoBehaviour {
	
	public GameObject []Players;
	public GUIText text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//Winning conditions are met
		//if (false /* win */ )
		{
			text.enabled = true;
			text.text = "Winner!";
			//should reset the level to default values
			//Application.LoadLevel(0);
			//sounds, hurray, etc
		}
	
	}
}
