using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// represents play mode (pause, sequential order, or random)
public enum PlayMode { Paused, Sequential, Random }

// class responsible for playing soundtrack, attached to main camera
public class MusicSwitcher : MonoBehaviour {
	public AudioClip[] songClips;				// list of song clips

	[Tooltip("Determines initial play mode.")]
	public PlayMode playMode = PlayMode.Sequential;

	[Tooltip("Displays playmode and song title.")]
	public Text display;

	[Tooltip("Duration text is displayed.")]
	public float displayTime = 2f;

	AudioSource player;						// source of audio
	int songIndex = -1;						// index of currently playing clip
	bool controlDown = false;				// allows "on key down" behavior

	// Use this for initialization
	void Start () {
		player = GetComponent<AudioSource>();		// find audiosource

		// initialize current clip based on starting play mode
		if (playMode == PlayMode.Paused)
		{
			songIndex = 0;
			player.clip = songClips[songIndex];
			player.Play();
			player.Pause();
		}
		else if (playMode == PlayMode.Sequential)
		{
			songIndex = 0;
			player.clip = songClips[songIndex];
			player.Play();
			displayTimer = displayTime;
			displayText = songClips[songIndex].name;
		}
		else if (playMode == PlayMode.Random)
		{
			songIndex = RandomIndex();
			player.clip = songClips[songIndex];
			player.Play();
			displayTimer = displayTime;
			displayText = songClips[songIndex].name;
		}
	}

	// Update is called once per frame
	void Update () {
		AdvanceSong();				// skip to next song if not puased and current has ended
		ManageInput();				// monitor and apply user input
		ManageDisplay();			// manage HUD display
	}

	float displayTimer = 0f;		// timer for temporarily displayed HUD information
	string displayText = "";		// displayed message

	// skips song, if not in pause mode
	void AdvanceSong()
	{
		if (!player.isPlaying)		// if end of current track has been reached...
		{
			if (playMode == PlayMode.Sequential)			// if playback mode is sequential...
			{
				player.Stop();    // sound stops upon losing focus, then restarts upon gaining focus, so make sure it's stopped
				songIndex = NextIndex();				// calculate next song index
				player.clip = songClips[songIndex];		// load associated clip
				player.Play();							// start playback
				displayTimer = displayTime;					// set timer to trigger HUD display
				displayText = songClips[songIndex].name;	// set HUD notification to song title
			}
			else if (playMode == PlayMode.Random)			// otherwise, if playback mode is random...
			{
				player.Stop();
				songIndex = RandomIndex();					// repeat above process with random index
				player.clip = songClips[songIndex];
				player.Play();
				displayTimer = displayTime;
				displayText = songClips[songIndex].name;
			}
		}
	}

	// monitor for user input and skip/pause/change playback mode appropriately
	void ManageInput()
	{
		if (Input.GetAxis("Pause Music") > 0f)		// if playback mode button pressed...
		{
			if (!controlDown)						// if this is the first that the button's been pressed...
			{
				// change playback mode and display new mode as indicated
				if (playMode == PlayMode.Paused)			// go to sequential if paused
				{
					playMode = PlayMode.Sequential;
					player.UnPause();
					displayTimer = displayTime;
					displayText = "Play Mode: Sequential";
				}
				else if (playMode == PlayMode.Sequential)	// go to random is sequential
				{
					playMode = PlayMode.Random;
					displayTimer = displayTime;
					displayText = "Play Mode: Random";
				}
				else if (playMode == PlayMode.Random)		// go to paused if random
				{
					playMode = PlayMode.Paused;
					player.Pause();
					displayTimer = displayTime;
					displayText = "Play Mode: Paused";
				}
			}
			controlDown = true;				// indicate that the keypress has been registered
		}
		else if (Input.GetAxis("Skip Music") > 0f)		// if skip forward button is pressed...
		{
			if (!controlDown)
			{
				if (playMode == PlayMode.Paused)		// start playing (sequential) if paused
				{
					playMode = PlayMode.Sequential;
					player.Stop();
					songIndex = NextIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}
				else if (playMode == PlayMode.Sequential)	// skip to next song if sequential
				{
					player.Stop();
					songIndex = NextIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}
				else if (playMode == PlayMode.Random)		// skip to random song if random
				{
					player.Stop();
					songIndex = RandomIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}

				// display new song title
				displayTimer = displayTime;
				displayText = songClips[songIndex].name;
			}
			controlDown = true;
		}
		else if (Input.GetAxis("Skip Music") < 0f)		// if skip backward button is pressed...
		{
			if (!controlDown)
			{
				if (playMode == PlayMode.Paused)				// start playing previous song (sequential) if paused
				{
					playMode = PlayMode.Sequential;
					player.Stop();
					songIndex = PreviousIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}
				else if (playMode == PlayMode.Sequential)		// skip to previous song if sequential
				{
					player.Stop();
					songIndex = PreviousIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}
				else if (playMode == PlayMode.Random)			// skip to random song if random
				{
					player.Stop();
					songIndex = RandomIndex();
					player.clip = songClips[songIndex];
					player.Play();
				}

				// dispay new song title on HUD
				displayTimer = displayTime;
				displayText = songClips[songIndex].name;
			}
			controlDown = true;
		}
		else
		{
			// reset controlDown if no keys are pressed
			controlDown = false;
		}
	}

	// if displayTimer is greater than zero, count down and display "displayText" until timer reaches 0
	void ManageDisplay()
	{
		if (displayTimer > 0)
		{
			displayTimer -= Time.deltaTime;
			display.text = displayText;
			display.gameObject.SetActive(true);
		}
		else
		{
			display.gameObject.SetActive(false);
		}

	}

	// calculate a random index between 0 and songClips.Length - 1
	int RandomIndex()
	{
		int randomIndex = songIndex;
		while(randomIndex == songIndex)
		{
			randomIndex = Random.Range(0, songClips.Length);
		}
		return randomIndex;
	}

	// calculate next index between 0 and songClips.Length - 1
	int NextIndex()
	{
		int nextIndex = songIndex + 1;
		if (nextIndex >= songClips.Length)
		{
			return 0;
		}
		else
		{
			return nextIndex;
		}
	}

	// calculate previous index between 0 and songClips.Length - 1
	int PreviousIndex()
	{
		int previousIndex = songIndex - 1;
		if (previousIndex < 0)
		{
			return songClips.Length - 1;
		}
		else
		{
			return previousIndex;
		}
	}
}
