using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages switching between fullscreen and windowed previews
public class FullscreenPreviewButton : MonoBehaviour {
	Map map;

	// Use this for initialization
	void Start () {
		map = FindObjectOfType<Map>();	// find map script
	}

	// go fullscreen when button pressed
	public void goFullscreen()
	{
		map.goFullscreen();
	}

	// Update is called once per frame
	private void Update()
	{
		if (map.isPreviewFullscreen())
		{
			// if fullscreen, go windowed when cancel is pressed
			if (Input.GetAxisRaw("Cancel") > 0f)
			{
				map.goWindowed();
			}
		}
	}
}
