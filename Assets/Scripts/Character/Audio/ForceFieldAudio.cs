using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class responsible for modulating and applying force field sound effect
// differs from AudioModulator in that it has an on/off based on whether a collision is detected
// also, only volume is modulated, not pitch
public class ForceFieldAudio : AudioBase {
	public Controller controller;				// source of driving data
	public bool useSpeed;						// base modulation on speed or acceleration?

	// bounding parameters for volume
	public float volumeMin;
	public float volumeMax;

	// bounding parameters for speed (if used)
	public float speedMin;
	public float speedMax;

	// bounding parameters for acceleration (if used)
	public float accelerationMin;
	public float accelerationMax;

	public bool clamp = false;					// clamp volume between max and min values?


	AudioSource source;							// source to be modulated

	// calculated interpolation parameters
	float slope;								 
	float intercept;

	// Use this for initialization
	void Start() {
		source = GetComponent<AudioSource>();		// find source

		// calculate slope and intercept data, based either on speed or acceleration (to taste)
		if (useSpeed)
		{
			slope = (volumeMax - volumeMin) / (speedMax - speedMin);
			intercept = volumeMin - slope * speedMin;
		}
		else
		{
			slope = (volumeMax - volumeMin) / (accelerationMax - accelerationMin);
			intercept = volumeMin - slope * accelerationMin;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (useSpeed)				// if modulation is to be based on speed...
		{
			// get speed from controller (will be zero if not colliding)
			float forceFieldStrength = controller.GetSoundDriver(SoundDriverCode.ForceField);
			// apply interpolation to volume based on speed
			if (forceFieldStrength > 0)
			{
				if (clamp)
				{
					source.volume = Mathf.Clamp((forceFieldStrength * slope + intercept), volumeMin, volumeMax);
				}
				else
				{
					source.volume = Mathf.Clamp((forceFieldStrength * slope + intercept), 0f, 0.9f);
				}
			}
			else
			{
				// if strength is zero, mute
				source.volume = 0f;
			}
		}
		else
		{
			// else, base it on acceleration
			if (controller.isColliding)				// if a collision is detected...
			{
				// apply interpolation to volume based on acceleration
				if (clamp)
				{
					source.volume = Mathf.Clamp((controller.GetAcceleration() * slope + intercept), volumeMin, volumeMax);
				}
				else
				{
					source.volume = Mathf.Clamp((controller.GetAcceleration() * slope + intercept), 0f, 0.9f);
				}
			}
			else
			{
				// otherwise, mute
				source.volume = 0f;
			}
		}
	}
}
