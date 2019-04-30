using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// start race button (sets menuMode and resets Timer)
public class StartButton : MonoBehaviour {
	public UserInterface ui;
	public Timer timer;

	public void StartRace()
	{
		timer.ResetTimer();
		ui.menuMode = MenuMode.Start2None;
	}
}
