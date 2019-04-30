using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// all possible menu modes
public enum MenuMode { None, Pause, Blackout, Win, Start, Start2None }

// class used to manage the user interface - handles switching between menus
public class UserInterface : MonoBehaviour {
	public Cursor3D StartMenuCursor;		// start menu cursor (needs to be hidden when not in use)
	public GameObject RacingPanel;			// "Racing" screen panel
	public GameObject StartPanel;			// start menu panel

	InputController inputController;		// input controller
	CameraSwitcher cameraSwitcher;			// camera switcher
	Rigidbody player;						// player's rigidbody, needed to turn off physics interpolation when
											// entering menus to avoid misaligned cursor plane (annoying bug)

	GameObject HUD;							// heads up display
	GameObject PauseMenu;					// pause menu
	GameObject BlackoutMenu;				// blackout menu
	GameObject WinMenu;						// win menu

	// public getter/setter for menumode (applies menumode when set)
	public MenuMode menuMode
	{
		get
		{
			return _menuMode;
		}
		set
		{
			_menuMode = value;
			// apply appropriate menu by:
			//	setting the cursor lockstate
			//	hiding or showing the startMenuCursor
			//	setting the appropriate menu gameobjects active or inactive
			//	setting the timescale (pause or unpause) Note: pause happens in FixedUpdate
			//	enabling or disabling player input
			//	setting the cameraSwitcher menuMode
			//	note: rigidbody interpolation needs to be disabled when entering menus
			//		attached to the player, because the collider for the cursor plane
			//		updates its position once every physics frame, putting it out of
			//		sync with the camera if interpolation is enabled
			if (_menuMode == MenuMode.None)
			{
				Cursor.lockState = CursorLockMode.Locked;
				StartMenuCursor.Hide();
				PauseMenu.SetActive(false);
				BlackoutMenu.SetActive(false);
				WinMenu.SetActive(false);
				HUD.SetActive(true);

				player.interpolation = RigidbodyInterpolation.Interpolate;	// reenable interpolation
				Time.timeScale = 1f;

				inputController.acceptInput = true;
				cameraSwitcher.menuMode = _menuMode;
			}
			else if (_menuMode == MenuMode.Pause)
			{
				inputController.acceptInput = false;
				cameraSwitcher.menuMode = _menuMode;
				Cursor.lockState = CursorLockMode.None;
				StartMenuCursor.Hide();
				WinMenu.SetActive(false);
				BlackoutMenu.SetActive(false);
				PauseMenu.SetActive(true);
				HUD.SetActive(false);

				player.interpolation = RigidbodyInterpolation.None;
			}
			else if (_menuMode == MenuMode.Blackout)
			{
				inputController.acceptInput = false;
				cameraSwitcher.menuMode = _menuMode;
				Cursor.lockState = CursorLockMode.None;
				StartMenuCursor.Hide();
				WinMenu.SetActive(false);
				PauseMenu.SetActive(false);
				BlackoutMenu.SetActive(true);
				HUD.SetActive(false);

				player.interpolation = RigidbodyInterpolation.None;
			}
			else if (_menuMode == MenuMode.Win)
			{
				inputController.acceptInput = false;
				cameraSwitcher.menuMode = _menuMode;
				Cursor.lockState = CursorLockMode.None;
				StartMenuCursor.Hide();
				PauseMenu.SetActive(false);
				BlackoutMenu.SetActive(false);
				WinMenu.SetActive(true);
				HUD.SetActive(false);

				player.interpolation = RigidbodyInterpolation.None;
			}
			else if (_menuMode == MenuMode.Start)
			{
				inputController.acceptInput = false;
				cameraSwitcher.menuMode = _menuMode;
				Cursor.lockState = CursorLockMode.None;
				StartMenuCursor.Hide(false);
				PauseMenu.SetActive(false);
				BlackoutMenu.SetActive(false);
				WinMenu.SetActive(false);
				HUD.SetActive(false);
				RacingPanel.SetActive(false);
				StartPanel.SetActive(true);

				player.interpolation = RigidbodyInterpolation.Interpolate;
				Time.timeScale = 1f;
			}
			else if (_menuMode == MenuMode.Start2None)
			{
				inputController.acceptInput = false;
				cameraSwitcher.menuMode = _menuMode;
				Cursor.lockState = CursorLockMode.Locked;
				StartMenuCursor.Hide();
				PauseMenu.SetActive(false);
				BlackoutMenu.SetActive(false);
				WinMenu.SetActive(false);
				HUD.SetActive(true);

				player.interpolation = RigidbodyInterpolation.Interpolate;
				Time.timeScale = 1f;
			}
		}
	}
	MenuMode _menuMode = MenuMode.Start;

	// Use this for initialization
	void Start ()
	{
		inputController = FindObjectOfType<InputController>();				// find the input controller
		player = inputController.gameObject.GetComponent<Rigidbody>();		// find the player's rigidbody
		cameraSwitcher = FindObjectOfType<CameraSwitcher>();				// find the camera switcher

		// find the menus
		HUD = GameObject.FindWithTag("HUD");
		PauseMenu = GameObject.FindWithTag("Pause Menu");
		BlackoutMenu = GameObject.FindWithTag("Blackout Menu");
		WinMenu = GameObject.FindWithTag("Win Menu");

		// apply the start menu
		menuMode = MenuMode.Start;
	}

	bool controlDown = false;		// ensures keypresses are only registered once

	// Update is called once per frame
	// monitor input, manage start2none transition
	void Update()
	{
		if (menuMode == MenuMode.None)
		{
			// if not in a menu, pause when "cancel" is pressed
			if (Input.GetAxisRaw("Cancel") > 0f)
			{
				if (!controlDown)
				{
					menuMode = MenuMode.Pause;
				}
				controlDown = true;
			}
			else
			{
				controlDown = false;
			}
		}
		else if (menuMode == MenuMode.Start2None)
		{
			// if in Start2None mode (transitioning from start menu to racing mode)...
			if (cameraSwitcher.menuMode == MenuMode.None)
			{
				// monitor cameraSwitchre menuMode and update menuMode when "None" is reached
				menuMode = MenuMode.None;
			}
		}
	}

	// don't pause until you've gone a physics frame with interpolation turned off, to avoid cursor plane misalignment
	private void FixedUpdate()
	{
		if (menuMode == MenuMode.Blackout || menuMode == MenuMode.Pause || menuMode == MenuMode.Win)
		{
			Time.timeScale = 0f;
		}
	}
}
