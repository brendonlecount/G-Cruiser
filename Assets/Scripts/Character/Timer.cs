using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// class responsible for timing and scoring race, attached to player character
public class Timer : MonoBehaviour {
	public UpgradeManager upgradeManager;		// stores account balance

	public AudioSource checkpointSource;		// plays checkpoint noise

	// win menu text objects
	public Text timeText;						// total race time
	public Text completionBonusText;			// completion bonus
	public Text timeBonusText;					// time bonus
	public Text environmentBonusText;			// environment bonus
	public Text totalText;						// total reward

	UserInterface ui;							// displays win menu
	Map map;									// source of course and map information

	int checkpointCount = 0;					// number of checkpoints remaining
	bool timing = false;						// timer running?
	float time = 0f;							// current time

	// initialize; find UI and Map objects
	private void Start()
	{
		ui = FindObjectOfType<UserInterface>();
		map = FindObjectOfType<Map>();
	}

	// Update is called once per frame
	void Update () {
		if (timing)
		{
			time += Time.deltaTime;		// update time if timing
		}
	}

	// reset timer (restarts race) called by pause and blackout menu "reset race" option
	public void ResetTimer()
	{
		// reset timer
		timing = false;
		time = 0f;
		// reset map if present (re-enable checkpoints, reset checkpoint count, and stop ghost)
		if (map != null)
		{
			map.currentCourse.ResetCourse();
			checkpointCount = map.currentCourse.GetCheckpointCount();
			map.currentCourse.GetComponent<Ghost>().StopGhost();
		}
		else
		{
			checkpointCount = 0;
		}
	}

	// register checkpoint
	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Checkpoint"))		// if it's a checkpoint...
		{
			checkpointSource.Play();								// play sound effect
			other.transform.parent.gameObject.SetActive(false);		// hide checkpoint
			checkpointCount -= 1;									// decrement checkpoint count
			if (checkpointCount == 0)						// if it's the last checkpoint...
			{
				map.currentCourse.GetComponent<Ghost>().StopGhost();	// stop ghost
				timing = false;											// stop timing
				ApplyBonuses();											// calculate and apply bonuses
				if (ui != null)
				{
					ui.menuMode = MenuMode.Win;							// display win menu if UI is present
				}
			}
			else if (!timing)								// otherwise, if not timing yet...
			{	
				timing = true;								// start timing
				// start ghost playback
				map.currentCourse.GetComponent<Ghost>().StartGhost(other.transform.parent.GetSiblingIndex());
			}
		}
	}

	// public time getter function (for HUD time readout)
	public float GetTime()
	{
		return time;
	}

	// public checkpoint count getter function (for HUD readout)
	public int GetCheckpointCount()
	{
		return checkpointCount;
	}

	// calculate and apply bonuses, updating win menu text accordingly
	void ApplyBonuses()
	{
		float timeBonus = 0f;
		if (time < map.currentCourse.optimalTime)
		{
			timeBonus = map.currentCourse.timeBonusMax * (map.currentCourse.optimalTime - time) / map.currentCourse.optimalTime;
		}
		float totalBonus = (map.currentCourse.completionBonus + timeBonus) * map.currentEnvironment.environmentBonus;

		timeText.text = S2HMS(time) + map.currentCourse.GetPlace(time);
		completionBonusText.text = "Completion Bonus: $" + map.currentCourse.completionBonus.ToString("n0");
		timeBonusText.text = "Time Bonus: $" + timeBonus.ToString("n0");
		environmentBonusText.text = "Environment Bonus: " + map.currentEnvironment.environmentBonus.ToString("n2") + "x";
		totalText.text = "Total Bonus: $" + totalBonus.ToString("n0");

		upgradeManager.balance += totalBonus;
	}

	// converts seconds into minutes and seconds string
	public string S2HMS(float seconds)
	{
		int m = (int)(seconds / 60);
		float s = seconds - m * 60;
		return m.ToString("X2") + ":" + s.ToString("00.##");
	}

	// public getter function for top times of current map
	public float GetTime(int place)
	{
		return map.currentCourse.GetTime(place);
	}
}
