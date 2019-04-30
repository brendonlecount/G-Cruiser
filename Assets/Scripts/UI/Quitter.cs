using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// basic script use in MVP to quit without the full UserInterface
public class Quitter : MonoBehaviour {
	// hide the cursor
	void Start()
	{
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update () {
		// if cancel pressed, quit the application
		if (Input.GetAxisRaw("Cancel") > 0f)
		{
			Application.Quit();
		}
	}
}
