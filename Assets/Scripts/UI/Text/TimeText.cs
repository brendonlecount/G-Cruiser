using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// displays top time of associated place in top times menu
public class TimeText : MonoBehaviour {
	public int place;		// 1st, 2nd, 3rd...

	Timer timer;			// timer script

	private void OnEnable()
	{
		// find the timer if not yet set
		if (timer == null)
		{
			timer = FindObjectOfType<Timer>();
		}
		// output associated top time
		GetComponent<Text>().text = timer.S2HMS(timer.GetTime(place));		
	}
}
