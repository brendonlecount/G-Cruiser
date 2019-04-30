using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// updates current environment label text
public class EnvironmentText : MonoBehaviour {
	public Text text;		// output text

	Map map;				// map

	private void OnEnable()
	{
		map = FindObjectOfType<Map>();	// find map script
	}

	// Update is called once per frame
	void Update () {
		text.text = map.environment;		// update output text
	}
}
