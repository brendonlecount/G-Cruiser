using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// course related buttons
public class CourseButton : MonoBehaviour {
	Map map;

	private void OnEnable()
	{
		map = FindObjectOfType<Map>();	// find the map script
	}

	// skip to next course
	public void NextCourse()
	{
		map.courseIndex += 1;
	}

	// skip to previous course
	public void PreviousCourse()
	{
		map.courseIndex -= 1;
	}
}
