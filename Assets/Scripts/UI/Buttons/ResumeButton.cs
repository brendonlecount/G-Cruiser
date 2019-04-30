using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// resume racing button for pause and win menus
public class ResumeButton : MonoBehaviour {
	UserInterface ui;		// user interface script

	private void Start()
	{
		ui = FindObjectOfType<UserInterface>();	// find user interface script
	}

	// when pressed, set menuMode to None (racing mode)
	public void Resume()
	{
		if (ui != null)
		{
			ui.menuMode = MenuMode.None;
		}
	}
}
