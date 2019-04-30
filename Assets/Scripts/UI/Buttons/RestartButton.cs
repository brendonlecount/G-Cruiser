using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// restart button for pause, win, and blackout menu
// to-do: make controllers and array of all controllers in the scene
public class RestartButton : MonoBehaviour {
	public Timer timer;				// timer for resetting race
	public Controller controller;	// controller for resetting player position

	UserInterface ui;				// user interface

	private void Start()
	{
		ui = FindObjectOfType<UserInterface>();		// find user interface
	}

	// when pressed, set UI menuMode to Start, reset the controller, and reset the timer
	public void Restart()
	{
		if (ui != null)
		{
			ui.menuMode = MenuMode.Start;
		}
		controller.ResetController();
		timer.ResetTimer();
	}
}
