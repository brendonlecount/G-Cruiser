using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

// camera modes
public enum CameraMode { First, Shoulder, Chase }

// class to handle user input related to camera movement, attaches to main camera node
// * up/down mouselook
// * switches between 1st and 3rd person cameras
// * updates main camera culling mask to hide/show force field based on view
// * handles looking over at start menu
// * monitors for camera recentering when in VR
public class CameraSwitcher : MonoBehaviour {
	public CameraMode cameraMode;			// 1st, shoulder, or chase camera?
	public GameObject firstPersonNode;		// attachment point for 1st person view
	public CameraPlacer shoulderNode;		// attachment point for shoulder view
	public CameraPlacer chaseNode;			// attachment point for chase view
	public MenuMode menuMode;				// current menumode
	public float clampAngleHead = 50f;		// min/max up/down look angles
	public float mouseSensitivity = 15f;	// mouse sensitivity
	public float startMenuLookAngle = 80f;	// left/right angle when looking over at start menu
	public float start2NoneRate = 45f;		// rate at which 
	public LayerMask mask1P;				// culling mask when in 1st person (hides force field)
	public LayerMask mask3P;				// culling mask when in 3rd person (shows force field)
	public Camera mainCamera;               // camera that culling mask is applied to
	public float lerpRate = 0.5f;

	float headRotationX;            // up/down rotation
	float headRotationXTarget = 0f;
	float headRotationY;			// left/right rotation (for looking towards start menu)

	// Use this for initialization
	void Start() {
		// set camera mode based on starting value
		switch (cameraMode)
		{
			case CameraMode.First:
				SwitchToFirstPerson();
				break;
			case CameraMode.Shoulder:
				SwitchToShoulder();
				break;
			case CameraMode.Chase:
				SwitchToChase();
				break;
			default:
				break;
		}
		// to do: recenter camera at start if VR is enabled
	}

	bool controlDown = false;		// makes sure camera switcher key only registers once per press
	bool recenterDown = false;		// makes sure VR recenter key only registers once per press

	// Update is called once per frame
	// monitors and applies user input, rotates camera away from start menu if applicable
	void LateUpdate() {
		if (menuMode == MenuMode.None)			// if racing...
		{
			RotateCamera();						// up/down mouselook
			CheckForViewChange();				// monitor camera switcher key
		}
		else if (menuMode == MenuMode.Start)	// else if in star menu
		{
			if (cameraMode != CameraMode.First)	// make sure you're in first person view
			{
				SwitchToFirstPerson();
			}
			headRotationX = 0f;					// zero out up/down mouselook
			headRotationY = startMenuLookAngle;	// look over at menu
			// apply rotation
			transform.localRotation = Quaternion.Euler(headRotationX, headRotationY, 0f);
		}
		else if (menuMode == MenuMode.Start2None)	// else if switching from start menu to racing mode...
		{
			// rotate camera back towards straight forward
			headRotationY -= GetStart2NoneRate() * Time.deltaTime;
			headRotationY = Mathf.Clamp(headRotationY, 0f, headRotationY);
			// apply rotation
			transform.localRotation = Quaternion.Euler(headRotationX, headRotationY, 0f);

			if (headRotationY == 0f)
			{
				// alert user interface script that head is centered (triggers switch to racing mode from Start2None mode)
				menuMode = MenuMode.None;
			}
		}

		// if VR is enabled, monitor and apply camera recentering
		if (GetIsVR())
		{
			if (Input.GetAxisRaw("Recenter Camera") > 0f)
			{
				if (!recenterDown)
				{
					recenterDown = true;
					UnityEngine.XR.InputTracking.Recenter();		// recenters camera
				}
				else
				{
					recenterDown = false;
				}
			}
		}
	}

	public float sineMult;			// multiplier for sinusoidal rotation rate of camera
	public float sineOffset;		// offset for sinusoidal rotation rate of camera

	// sinusoidal rate at which camera rotates back towards center from looking over at menu
	float GetStart2NoneRate()
	{
		float angleFraction = headRotationY / startMenuLookAngle;
		return start2NoneRate * (-Mathf.Cos(angleFraction * 2 * Mathf.PI) * 0.5f * sineMult + sineOffset + sineMult);
	}

	// monitor and apply up/down mouselook
	void RotateCamera()
	{
		if (cameraMode == CameraMode.First && !GetIsVR())	// disable up/down mouselook if in 3rd person or VR (VR has headtracking)
		{
			// Rotate head and camera based on Y mouse input
			headRotationXTarget = Mathf.Clamp(headRotationXTarget - mouseSensitivity * Input.GetAxis("Mouse Y"), -clampAngleHead, clampAngleHead);
			headRotationX = Mathf.Lerp(headRotationX, headRotationXTarget, 0.5f);
			transform.localRotation = Quaternion.Euler(new Vector3(headRotationX, 0f, 0f));

			// to do: recenter up/down mouselook if keyboard or joystick banking input is detected


		}
	}

	// monitor view change hotkey, and switch views if pressed
	void CheckForViewChange()
	{
		if (Input.GetAxis("Change View") > 0f)		// when switch view key is first pressed...
		{
			if (!controlDown)
			{
				controlDown = true;
				switch (cameraMode)		// switch to and apply next camera mode
				{
					case CameraMode.First:
						SwitchToShoulder();
						break;
					case CameraMode.Shoulder:
						SwitchToChase();
						break;
					case CameraMode.Chase:
						SwitchToFirstPerson();
						break;
					default:
						break;
				}
			}
		}
		else
		{
			controlDown = false;
		}
	}

	// switches to first person camera mode
	void SwitchToFirstPerson()
	{
		cameraMode = CameraMode.First;
		mainCamera.cullingMask = mask1P;
		transform.parent = firstPersonNode.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		headRotationX = 0f;
		headRotationXTarget = 0f;
		shoulderNode.SetCamActive(false);
		chaseNode.SetCamActive(false);
	}

	// switches to shoulder camera mode
	void SwitchToShoulder()
	{
		cameraMode = CameraMode.Shoulder;
		mainCamera.cullingMask = mask3P;
		transform.parent = shoulderNode.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		headRotationX = 0f;
		shoulderNode.SetCamActive(true);
		chaseNode.SetCamActive(false);
	}

	// switches to chase camera mode
	void SwitchToChase()
	{
		cameraMode = CameraMode.Chase;
		mainCamera.cullingMask = mask3P;
		transform.parent = chaseNode.transform;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		headRotationX = 0f;
		shoulderNode.SetCamActive(false);
		chaseNode.SetCamActive(true);
	}

	// true if VR is enabled and active
	bool GetIsVR()
	{
		return UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRSettings.isDeviceActive;
	}
}
