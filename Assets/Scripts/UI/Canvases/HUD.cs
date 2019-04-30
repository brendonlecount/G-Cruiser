using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// manages heads up display (HUD)
// to-do: smooth out reporting to only update occasionally?
public class HUD : MonoBehaviour {
	public Timer timer;				// race time related information
	public Controller controller;	// speed and acceleration information

	public Text speedometer;
	public Text gReadout;
	public Text counter;
	public Text time;

	const float MPS2MPH = 2.23694f;
	float g;

	// Use this for initialization
	void Start () {
		// initialize gravitaty constant for G calculation
		g = Mathf.Abs(Physics.gravity.y);
	}

	private void Update()
	{
		// update speedometer text
		speedometer.text = (controller.GetSpeed() * MPS2MPH).ToString("N0") + " mph";
		// update g readout text
		gReadout.text = (controller.GetAcceleration() / g).ToString("N1") + " g";
		// update checkpoint counter text
		counter.text = timer.GetCheckpointCount().ToString("N0");
		// update timer text
		time.text = timer.S2HMS(timer.GetTime());
	}

}
