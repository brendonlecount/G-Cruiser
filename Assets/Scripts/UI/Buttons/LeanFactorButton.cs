using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// buttons for adjusting lean factor
public class LeanFactorButton : MonoBehaviour {
	public float increment;
	public Text value;

	GameplaySettings settings;

	private void OnEnable()
	{
		// find settings if not yet initialized
		if (settings == null)
		{
			settings = FindObjectOfType<GameplaySettings>();
		}

		// initialize text value
		if (settings != null)
		{
			value.text = settings.leanFactor.ToString("0.#");
		}
	}

	// increment lean factor and apply change to text
	public void Increase()
	{
		if (settings != null)
		{
			settings.leanFactor += increment;
			value.text = settings.leanFactor.ToString("0.#");
		}
	}

	// decrement lean factor and apply change to text
	public void Decrease()
	{
		if (settings != null)
		{
			settings.leanFactor -= increment;
			value.text = settings.leanFactor.ToString("0.#");
		}
	}
}
