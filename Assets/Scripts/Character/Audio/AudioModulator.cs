using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class used to modulate audio source pitch and volume based on stat returned from controller,
// determined by sound driver code (e.g. velocity for wind pitch and volume)
public class AudioModulator : AudioBase {
	public Controller controller;				// source of driving stat
	public SoundDriverCode code;				// code for driving stat
	// bounding volumes
	public float volumeMin = 0f;				
	public float volumeMax = 0f;
	public float levelVolumeMin = 0f;
	public float levelVolumeMax = 1f;
	public bool clampVolume = false;			// clamp volume or extrapolate?

	// bounding pitches
	public float pitchMin = 0.6f;
	public float pitchMax = 2f;
	public float levelPitchMin = 0f;
	public float levelPitchMax = 1f;
	public bool clampPitch = false;				// clamp pitch or extrapolate?

	AudioSource source;							// audio source to be controlled

	// calculated interpolation data for pitch and volume
	float volumeSlope;
	float volumeIntercept;
	float pitchSlope;
	float pitchIntercept;


	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		
		// calculate interpolation data for pitch and volume based on y=mx + b
		volumeSlope = (volumeMax - volumeMin) / (levelVolumeMax - levelVolumeMin);
		volumeIntercept = volumeMin - volumeSlope * levelVolumeMin;
		pitchSlope = (pitchMax - pitchMin) / (levelPitchMax - levelPitchMin);
		pitchIntercept = pitchMin - pitchSlope * levelPitchMin;
	}
	
	// Update is called once per frame
	void Update () {
		float driver = controller.GetSoundDriver(code);		// get driving stat from controller

		if (clampVolume)		// if volume should be clamped between volumeMin and volumeMax...
		{
			// calculate and apply clamped volume based on driver value, slope, and intercept
			source.volume = Mathf.Clamp((driver * volumeSlope + volumeIntercept), volumeMin, volumeMax);
		}
		else
		{
			// else, calculate unclamped value (volumes close to 1 produce clipping, so limit to 0.9)
			source.volume = Mathf.Clamp((driver * volumeSlope + volumeIntercept), 0f, 0.9f);
		}

		// repeat for pitch
		if (clampPitch)
		{
			source.pitch = Mathf.Clamp(driver * pitchSlope + pitchIntercept, pitchMin, pitchMax);
		}
		else
		{
			source.pitch = Mathf.Clamp(driver * pitchSlope + pitchIntercept, 0.6f, 2.9f);
		}
	}
}
