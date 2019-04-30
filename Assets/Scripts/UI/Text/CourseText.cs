using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// updates current course label text
public class CourseText : MonoBehaviour {
	public Text text;		// output text

	Map map;				// map

	private void OnEnable()
	{
		map = FindObjectOfType<Map>();		// find map
	}

	// Update is called once per frame
	void Update()
	{
		text.text = map.course;		// output current course
	}
}
