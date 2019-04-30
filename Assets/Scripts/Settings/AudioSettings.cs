using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType { Music, SFX }

// not used (sound mixer used instead)
public class AudioSettings : MonoBehaviour {
//	public VolumeSlider masterVolume;
//	public VolumeSlider sfxVolume;
//	public VolumeSlider musicVolume;

	AudioBase[] audioSources;

	public float volumeMusicDefault;
	public float volumeSFXDefault;

	public float volumeMaster
	{
		get
		{
			return _volumeMaster;
		}
		set
		{
			value = Mathf.Clamp(value, 0f, 1f);
			_volumeMaster = value;
		}
	}
	float _volumeMaster = 0.75f;

	public float volumeMusic
	{
		get
		{
			return _volumeMusic;
		}
		set
		{
			value = Mathf.Clamp(value, 0f, 1f);
			_volumeMusic = value;
			UpdateVolumes();
		}
	}
	float _volumeMusic = 0.75f;

	public float volumeSFX
	{
		get
		{
			return _volumeSFX;
		}
		set
		{
			value = Mathf.Clamp(value, 0f, 1f);
			_volumeSFX = value;
			UpdateVolumes();
		}
	}
	float _volumeSFX = 0.75f;

	// Use this for initialization
	void Start () {
		//		audioSources = GameObject.FindGameObjectsWithTag("Audio Source");
		audioSources = FindObjectsOfType<AudioBase>();
		volumeMusic = volumeMusicDefault;
		volumeSFX = volumeSFXDefault;
	}

	void UpdateVolumes()
	{
		for(int i = 0; i < audioSources.Length; i++)
		{
//			audioSources[i].UpdateVolume(this);
		}
	}
}
