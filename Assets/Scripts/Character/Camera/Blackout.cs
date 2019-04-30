using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

// class responsible for simulating and applying G-LOC and collision based knockouts
// attached to main camera
public class Blackout : MonoBehaviour {
	public Controller controller;						// controller to get acceleration from
	public VignetteAndChromaticAberration vignette;		// vignette screen space effect
	public ColorCorrectionCurves color;					// hue/saturation screen space effect

	public float knockoutFactorVertical = 0.5f;			// reduces vertical acceleration (simulates knees bending)
	public float knockoutRate = 20f;					// rate at which screen goes black when knocked out

	public float accelerationFactor = 20f;				// scales Gs when modeling G-LOC (higher values, quicker G-LOC onset)
	public float baseAcceleration = 30f;				// offsets Gs when modeling G-LOC (no G-LOC below threshold value)

	public float blackoutRate = 0.25f;					// rate at which G-LOC takes effect
	public float blackoutFactorStartVisuals = 0.5f;		// threshold above which G-LOC visual show up

	UserInterface ui;									// UI object (for triggering blackout menu and monitoring pause state)

	float blackoutFactor = 0f;							// G-LOC/blackout intensity (0 to 1)

	bool knockedOut = false;							// knocked unconscious?

	private void Start()
	{
		ui = FindObjectOfType<UserInterface>();			// find user interface object
	}

	// Update is called once per frame
	void Update () {
		if (ui == null || ui.menuMode == MenuMode.None)			// if not paused...
		{
			// get acceleration vector
			Vector3 effectiveAcceleration = controller.GetAccelerationVector();
			// scale down vertical acceleration to simulate knees absorbing impact
			effectiveAcceleration.y *= knockoutFactorVertical;

			if (effectiveAcceleration.magnitude > controller.GetKnockoutAcceleration())	// if acceleration is above knockout threshold...
			{
				knockedOut = true;		// set knockedOut to true
			}
			else if (knockedOut)														// else if knocked out...
			{
				blackoutFactor += Time.deltaTime * knockoutRate;	// quickly ramp up blackoutFactor
			}
			else
			{
				// else...
				// calculate steady state blackout factor
				float blackoutFactorTarget = (controller.GetAccelerationY() * controller.GetGFactor() - baseAcceleration) / accelerationFactor;
				// update blackoutFactor based on steady state (target) blackout factor
				blackoutFactor += (blackoutFactorTarget - blackoutFactor) * Time.deltaTime * blackoutRate;
				// clamp blackout factor
				blackoutFactor = Mathf.Clamp(blackoutFactor, 0f, 2f);
			}

			// calculate visuals intensity from current blackout factor
			float appliedFactor = (blackoutFactor - blackoutFactorStartVisuals) / (1f - blackoutFactorStartVisuals);
			// apply visuals
			vignette.intensity = Mathf.Clamp(appliedFactor, 0.01f, 1f);
			color.saturation = Mathf.Clamp(1f - appliedFactor, 0.0f, 0.99f);

			if (blackoutFactor >= 1f && ui != null)		// if blacked out...
			{
				ui.menuMode = MenuMode.Blackout;		// trigger blackout menu
			}
		}
		else if (ui.menuMode == MenuMode.Start || ui.menuMode == MenuMode.Start2None)	// else if paused...
		{
			// reset appropriate factors
			knockedOut = false;
			blackoutFactor = 0f;
			vignette.intensity = 0.01f;			// don't set these to 0 (vignette) or 1 (saturation), or the effect  
			color.saturation = 0.99f;			// "goes to sleep" and it takes time for it to wake up
		}
	}
}
