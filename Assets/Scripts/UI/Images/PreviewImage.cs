using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// starts/stops course preview video, attached to render-to-texture target in menu
public class PreviewImage : MonoBehaviour {
	Map map;

	// on enable, find the map and start preview playback
	private void OnEnable()
	{
		if (map == null)
		{
			map = FindObjectOfType<Map>();
		}
		map.playPreview();
	}

	// on disable, stop preview playback
	private void OnDisable()
	{
		map.stopPreview();
	}

}