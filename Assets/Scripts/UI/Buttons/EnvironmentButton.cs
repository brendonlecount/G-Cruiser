using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// environment related buttons
public class EnvironmentButton : MonoBehaviour {
	Map map;

	private void OnEnable()
	{
		map = FindObjectOfType<Map>();	// find the map script
	}

	// skip to next environment
	public void NextEnvironment()
	{
		map.environmentIndex += 1;
	}

	// skip to previous environment
	public void PreviousEnvironment()
	{
		map.environmentIndex -= 1;
	}
}
