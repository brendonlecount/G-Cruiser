using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// class used to keep track of a map, or collection of courses and environments
// attached to the parent object of the courses and environments
public class Map : MonoBehaviour {
	public string startingEnvironment;			// beginning environment (uses object name)
	public string startingCourse;				// beginning course (uses object name)

	Course[] courses;							// array of courses associated with the map
	Environment[] environments;					// array of environments associated with the map
	ReflectionProbe[] probes;					// array of environment probes associated with the map
												// (after experimentation, having only 1 is most practical)

	GameObject previewCamera;					// camera used to render map previews
	GameObject previewCameraFS;					// camera used to render full screen map previews

	bool playingPreview = false;				// is a preview playing?
	bool fullscreenPreview = false;				// is it fullscreen?

	private void Start()
	{
		probes = GetComponentsInChildren<ReflectionProbe>();			// find the reflection probes
		environments = GetComponentsInChildren<Environment>(true);		// find the  environments
		environment = startingEnvironment;								// set environment to starting environment
		courses = GetComponentsInChildren<Course>(true);				// find the courses
		course = startingCourse;										// set the course to starting course
		previewCamera = GameObject.FindWithTag("Preview Camera");		// find the preview camera
		previewCameraFS = GameObject.FindWithTag("Preview Camera FS");	// find the fullscreen preview camera
		previewCameraFS.SetActive(false);								// disable the preview cameras
		previewCamera.SetActive(false);
	}

	// getter/setter for current environment index
	public int environmentIndex
	{
		get
		{
			return _environmentIndex;
		}

		// applies current environment when set
		set
		{
			if (value >= environments.Length)
			{
				_environmentIndex = 0;		// wrap to 0 if length is exceeded
			}
			else if (value < 0)
			{
				_environmentIndex = environments.Length - 1;	// wrap to end if 0 is exceeded
			}
			else
			{
				_environmentIndex = value;
			}
			// disable any active environment
			for (int i = 0; i < environments.Length; i++)
			{
				environments[i].EnableEnvironment(false);
			}
			// enable current environment
			environments[_environmentIndex].EnableEnvironment(true);
			// refresh reflection probes
			for (int i = 0; i < probes.Length; i++)
			{
				probes[i].RenderProbe();
			}
		}
	}
	int _environmentIndex = 0;

	// getter/setter for current environment name
	public string environment
	{
		get
		{
			return environments[_environmentIndex].name;
		}
		set
		{
			for (int i = 0; i < environments.Length; i++)
			{
				// if found, set environment index accordingly
				if (environments[i].name == value)
				{
					environmentIndex = i;
					break;
				}
			}
		}
	}

	// getter for current environment
	public Environment currentEnvironment
	{
		get
		{
			return environments[_environmentIndex];
		}
	}

	// getter/setter for current course index
	public int courseIndex
	{
		get
		{
			return _courseIndex;
		}

		set
		{
			// wrap course index if bounds exceeded
			if (value >= courses.Length)
			{
				_courseIndex = 0;
			}
			else if (value < 0)
			{
				_courseIndex = courses.Length - 1;
			}
			else
			{
				_courseIndex = value;
			}
			// enable active course, disable others
			for (int i = 0; i < courses.Length; i++)
			{
				courses[i].gameObject.SetActive(i == _courseIndex);
			}
		}
	}
	int _courseIndex = 0;

	// getter/setter for current course name
	public string course
	{
		get
		{
			return currentCourse.name;
		}

		set
		{
			for(int i = 0; i < courses.Length; i++)
			{
				// if found, update course index
				if (courses[i].name == value)
				{
					courseIndex = i;
					break;
				}
			}
		}
	}

	// getter for current course
	public Course currentCourse
	{
		get
		{
			return courses[_courseIndex];
		}
	}

	// switches to fullscreen preview
	public void goFullscreen()
	{
		if (playingPreview)
		{
			previewCameraFS.SetActive(true);
			fullscreenPreview = true;
		}
	}

	// switches to windowed preview
	public void goWindowed()
	{
		if (playingPreview)
		{
			previewCameraFS.SetActive(false);
			fullscreenPreview = false;
		}
	}

	// starts preview playback
	public void playPreview()
	{
		playingPreview = true;
		previewCamera.SetActive(true);
		currentCourse.GetComponent<PlayableDirector>().Play();
	}

	// stops preview playback
	public void stopPreview()
	{
		playingPreview = false;
		fullscreenPreview = false;
		currentCourse.GetComponent<PlayableDirector>().Stop();
		previewCameraFS.SetActive(false);
		previewCamera.SetActive(false);
	}

	// true if playing a preview
	public bool isPlayingPreview()
	{
		return playingPreview;
	}

	// true if playing a preview fullscreen
	public bool isPreviewFullscreen()
	{
		return fullscreenPreview;
	}
}
