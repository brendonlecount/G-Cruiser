using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

// class responsible for recording a ghost playthrough, attached to player (disabled during normal play)
public class Ghoster : MonoBehaviour {
	public bool enableRecording = false;		// set to true to enable ghost recording, false to disable it
	public string ghostFile;					// output file for ghost data
	public float interval = 1f;					// polling interval for ghost data, in seconds
	public Timer timer;							// Timer object (attached to player, used to get checkpoint count)
	public Text recordingText;					// displays "recording" message on HUD


	bool recording = false;					// true if recording, false if not
	StreamWriter writer;					// file output stream
	float frameTimer = 0f;					// 
	float frameTime = 0f;

	// Update is called once per frame
	void Update () {
		if (enableRecording)				// ignore if recording is disabled
		{
			if (triggerTimer > 0f)			
			{
				// decrement timer; start/stop recording trigger cannot trigger until timer reaches zero, to prevent double-triggering
				triggerTimer -= Time.deltaTime;
			}
			if (Input.GetKeyDown(KeyCode.G))	// check for input to start/stop recording
			{
				if (recording)
				{
					StopRecording();
				}
				else
				{
					StartRecording();
				}
			}
			else if (recording)					// otherwise, if recording...			
			{
				Record();					// record a new data point, and/or update frame timer
			}
		}
	}

	float triggerTimer = 0f;				// prevents double-activation
	// starts/stops recording if a "record" trigger is encountered
	void OnTriggerEnter(Collider other)
	{
		if (enableRecording)
		{
			if (triggerTimer <= 0f && other.CompareTag("Record Trigger"))
			{
				triggerTimer = 2f;      // prevents instant retriggering by additional colliders
				if (recording)
				{
					StopRecording();
				}
				else
				{
					StartRecording();
				}
			}
		}
	}

	// starts recording (opens the ghost file and shows "recording" on HUD)
	void StartRecording()
	{
		writer = new StreamWriter(ghostFile);
		recording = true;
		recordingText.enabled = true;
	}

	// stops recording (closes the ghost file and hides "recording" on HUD, resets timers)
	void StopRecording()
	{
		recording = false;
		writer.Close();
		recordingText.enabled = false;
		frameTimer = 0f;
		frameTime = 0f;
	}

	// records; increment frame time, and record a datapoint if a polling interval has been reached
	void Record()
	{
		frameTime += Time.deltaTime;		// increment frame time
		if (frameTime >= frameTimer)		// if a polling interval has been reached...
		{
			string line = frameTime.ToString() + '\t';				// frame time
			line += timer.GetCheckpointCount().ToString() + '\t';	// checkpoint count
			line += transform.position.x.ToString() + '\t';			// x position
			line += transform.position.y.ToString() + '\t';			// y position
			line += transform.position.z.ToString() + '\t';			// z position
			line += transform.rotation.w.ToString() + '\t';			// rotation quaternion w
			line += transform.rotation.x.ToString() + '\t';			// rotation quaternion x
			line += transform.rotation.y.ToString() + '\t';			// rotation quaternion y
			line += transform.rotation.z.ToString();				// rotation quaternion z

			writer.WriteLine(line);						// output 
			frameTimer = frameTime + interval;			// set time of next polling interval
		}
	}
}
