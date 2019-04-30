using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// stores and applies gameplay settings, like lean factor and mouse sensitivity
public class GameplaySettings : MonoBehaviour {
	public float mouseSensitivityDefault;		// starting mouse sensitivity
	public float mouseSensitivityMax;			// maximum mouse sensitivity
	public float mouseSensitivityMin;			// minimum mouse sensitivity

	Controller[] controllers;					// controllers to report to (lean factor)
	CameraSwitcher cameraSwitcher;				// camera switcher to report to (mouse sensitivity)
	InputController inputController;			// input controller to report to (mouse sensitivity)

	// mouse sensitivity (how responsive the mouse is)
	public float mouseSensitivity
	{
		get
		{
			return _mouseSensitivity;
		}

		set
		{
			// clamp value between min and max
			if (value > mouseSensitivityMax)
			{
				_mouseSensitivity = mouseSensitivityMax;
			}
			else if (value < mouseSensitivityMin)
			{
				_mouseSensitivity = mouseSensitivityMin;
			}
			else
			{
				_mouseSensitivity = value;
			}
			// report mouse sensitivity to camera switcher and input controller
			cameraSwitcher.mouseSensitivity = mouseSensitivity;
			inputController.mouseSensitivity = mouseSensitivity;
		}
	}
	float _mouseSensitivity = 15f;

	// lean factor (how steep the bank is, relative to what is physically realistic)
	public float leanFactor
	{
		get
		{
			return _leanFactor;
		}

		set
		{
			// clamp value between 0 and 1
			if (value > 1f)
			{
				_leanFactor = 1f;
			}
			else if (value < 0f)
			{
				_leanFactor = 0f;
			}
			else
			{
				_leanFactor = value;
			}
			for (int i = 0; i < controllers.Length; i++)
			{
				controllers[i].leanFactor = leanFactor;		// report lean factor to controllers
			}
		}
	}
	float _leanFactor = 1f;

	private void Start()
	{
		controllers = FindObjectsOfType<Controller>();				// find controllers in scene
		cameraSwitcher = FindObjectOfType<CameraSwitcher>();		// find camera switcher
		inputController = FindObjectOfType<InputController>();		// find input controller

		// initialize settings
		leanFactor = 1f;
		mouseSensitivity = mouseSensitivityDefault;
	}
}
