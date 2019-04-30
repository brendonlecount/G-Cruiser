using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// updates text for big race timer
public class BigTimer : MonoBehaviour {
	public Text timeDisplay;		// output
	Timer timer;					// input (race time source)

	// Use this for initialization
	void Start () {
		timer = FindObjectOfType<Timer>();	// find the timer script
	}
	
	// Update is called once per frame
	void Update () {
		// update the time
		timeDisplay.text = timer.S2HMS(timer.GetTime());
	}
}
