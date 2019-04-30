using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

// handles volumes, managing main audio mixer
public class VolumePanel : MonoBehaviour {
	// main audio mixer
	public AudioMixer masterMixer;

	// outputs for volume in menu
	public Text sfxVolume;
	public Text musicVolume;
	public Text masterVolume;

	// bounds and increment for volume adjustments
	public float volumeMax = 0f;
	public float volumeMin = -80f;
	public float volumeIncrement = 5f;

	// on enable, read volumes from masterMixer and output them to the menu
	private void OnEnable()
	{
		float outFloat;

		masterMixer.GetFloat("volMaster", out outFloat);
		masterVolume.text = outFloat.ToString("N0");

		masterMixer.GetFloat("volMusic", out outFloat);
		musicVolume.text = outFloat.ToString("N0");

		masterMixer.GetFloat("volSFX", out outFloat);
		sfxVolume.text = outFloat.ToString("N0");
	}

	// increase or decrease associated volume
	public void IncreaseSFX()
	{
		SetVolume("volSFX", volumeIncrement, sfxVolume);
	}

	public void DecreaseSFX()
	{
		SetVolume("volSFX", -volumeIncrement, sfxVolume);
	}

	public void IncreaseMusic()
	{
		SetVolume("volMusic", volumeIncrement, musicVolume);
	}

	public void DecreaseMusic()
	{
		SetVolume("volMusic", -volumeIncrement, musicVolume);
	}

	public void IncreaseMaster()
	{
		SetVolume("volMaster", volumeIncrement, masterVolume);
	}

	public void DecreaseMaster()
	{
		SetVolume("volMaster", -volumeIncrement, masterVolume);
	}

	// apply a volume change to the specified audio group
	void SetVolume(string type, float increment, Text output)
	{
		float volume;
		masterMixer.GetFloat(type, out volume);		// get the current volume from the mixer

		// update the mixer with  the new volume
		masterMixer.SetFloat(type, Mathf.Clamp(volume + increment, volumeMin, volumeMax));

		// write the new volume to the screen
		output.text = Mathf.Clamp(volume + increment, volumeMin, volumeMax).ToString("N0");
	}
}
