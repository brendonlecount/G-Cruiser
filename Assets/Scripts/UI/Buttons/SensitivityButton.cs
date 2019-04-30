using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// buttons for increasing and decreasing mouse sensitivity
public class SensitivityButton : MonoBehaviour {
	public float increment;
	public Text value;

	GameplaySettings settings;

	private void OnEnable()
	{
		if (settings == null)
		{
			settings = FindObjectOfType<GameplaySettings>();	// find gameplay settings script
		}
		if (settings != null)
		{
			value.text = settings.mouseSensitivity.ToString();	// update text with current sensitivity
		}
	}

	// when pressed, increment sensitivity and update text with new value
	public void Increase()
	{
		if (settings != null)
		{
			settings.mouseSensitivity += increment;
			value.text = settings.mouseSensitivity.ToString();
		}
	}

	// when pressed, decrement sensitivity and update text with new value
	public void Decrease()
	{
		if (settings != null)
		{
			settings.mouseSensitivity -= increment;
			value.text = settings.mouseSensitivity.ToString();
		}
	}
}
