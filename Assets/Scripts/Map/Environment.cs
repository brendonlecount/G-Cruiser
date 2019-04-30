using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class that manages data pertaining to an environmental mode (day, night, fog, etc.)
public class Environment : MonoBehaviour {
	public float environmentBonus;				// bonus for completing a race under these conditions, if any
	public float environmentIntensity;			// ambient intensity
	public Material skybox;						// skybox
	public bool fog;							// fog?
	public FogMode fogMode;						// fog mode
	public Color fogColor;						// fog color
	public float fogDensity;					// fog density
	public GameObject[] props;					// any props that should be enabled, e.g. moon, headlight

	// enables (applies) the environment (or cleans up after it if "enable" is false)
	public void EnableEnvironment(bool enable)
	{
		if (enable)	// apply the environment
		{
			EnableProps(true);
			ApplyEnvironmentSettings();
		}
		else
		{
			// clear the environment
			EnableProps(false);
		}
	}

	// enables/disables any props associated with the environment
	void EnableProps(bool enable)
	{
		for(int i = 0; i < props.Length; i++)
		{
			if (props[i] != null)
			{
				props[i].SetActive(enable);
			}
		}
	}

	// applies environment settings associated with the environment (skybox, ambient intensity, fog)
	void ApplyEnvironmentSettings()
	{
		RenderSettings.skybox = skybox;							// skybox to use
		RenderSettings.ambientIntensity = environmentIntensity;	// brightness of skybox
		if (fog)												// fog settings
		{
			RenderSettings.fog = true;
			RenderSettings.fogMode = fogMode;
			RenderSettings.fogColor = fogColor;
			RenderSettings.fogDensity = fogDensity;
		}
		else
		{
			RenderSettings.fog = false;
		}
		DynamicGI.UpdateEnvironment();							// applies changes made to render settings
	}
}
