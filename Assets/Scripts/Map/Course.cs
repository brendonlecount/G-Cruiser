using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// class that contains and manages data related to a course (a collection of checkpoints on a map)
public class Course : MonoBehaviour {
	public TextAsset startingTopTimes;			// initial top times for the course, read only
	public float completionBonus;				// completion bonus for the course
	public float optimalTime;					// optimal time for the course (time bonus ramps up from here)
	public float timeBonusMax;					// maximum time bonus (bonus at 0)

	// to be used for writing new top times to a file
	string topTimesFile
	{
		get { return ""; }
	}
	


	Checkpoint[] checkpoints;		// list of checkpoints making up the course
	float[] topTimes;				// list of top times for the course
	string[] places;				// places

	// Use this for initialization
	void OnEnable () {
		// populate checkpoints array (replaces registration originally in checkpoint script)
		checkpoints = GetComponentsInChildren<Checkpoint>(true);		// include inactive checkpoints

		// initialize and populate top times array
		topTimes = new float[3];
		ReadTimes();

		// initialize and populate "places" array
		places = new string[3];
		places[0] = "(1st)";
		places[1] = "(2nd)";
		places[2] = "(3rd)";
	}

	// re-enables all checkpoints when a race initiated or reset
	public void ResetCourse()
	{
		for(int i = 0; i < checkpoints.Length; i++)			// for each checkpoint...
		{
			checkpoints[i].gameObject.SetActive(true);		// set checkpoint active again
		}
	}

	// read top times from file
	void ReadTimes()
	{
		string[] times;	// array of top times
		string[] separator = { System.Environment.NewLine };	// new line delimiter

		if (File.Exists(topTimesFile))		// if the writable top times file exists...
		{
			StreamReader reader = new StreamReader(topTimesFile);		// open it
			if (reader != null)
			{
				// populate the times string array from it
				times = reader.ReadToEnd().Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				times = null;
			}
		}
		else
		{
			// populate the times string array from the read-only startingTopTimes textAsset
			times = startingTopTimes.text.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
		}

		// parse the times string array into floats and store them in the topTimes array
		for (int i = 0; i < topTimes.Length && i < times.Length; i++)
		{
			topTimes[i] = float.Parse(times[i]);
		}
	}

	// output top times array to text file specified by topTimesFile
	void WriteTimes()
	{
		if (topTimesFile != "")		// if a file is specified
		{
			StreamWriter writer = new StreamWriter(topTimesFile);

			for (int i = 0; i < topTimes.Length; i++)
			{
				writer.WriteLine(topTimes[i].ToString());		// output top times line by line
			}

			writer.Close();
		}
	}

	// returns a place (first, second, third, etc.) for a given race time, if applicable, and
	// updates the topTimes array if a top time has been provided
	// called by Timer at the end of a race, when the win menu is shown
	public string GetPlace(float time)
	{
		for (int i = 0; i < topTimes.Length; i++)
		{
			if (time < topTimes[i])
			{
				for (int j = topTimes.Length - 1; j > i; j--)
				{
					topTimes[j] = topTimes[j - 1];
				}
				topTimes[i] = time;
				WriteTimes();
				return places[i];
			}
		}
		return "";
	}

	// public getter for total number of checkpoints
	public int GetCheckpointCount()
	{
		return checkpoints.Length;
	}

	// public getter for top times
	public float GetTime(int place)
	{
		return topTimes[place];
	}
}
