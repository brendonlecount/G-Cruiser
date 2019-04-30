using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class responsible for driving Controller based on user input
public class InputController : MonoBehaviour {
	public Controller controller;			// controller object to drive
	public float mouseSensitivity;			// mouse sensitivity
	public bool acceptInput = true;			// used to enable or disable controls

	// Update is called once per frame
	void Update () {
		if (acceptInput)
		{
			// get accelerate or breaking input from Accelerate axis (keys or joystick)
			if (Input.GetAxisRaw("Accelerate") > 0f)	// accelerate if Accelerate axis is positive
			{
				controller.SetDriveMode(DriveMode.Accelerate);		// apply to controller
			}
			else if (Input.GetAxisRaw("Accelerate") < 0f)	// decelerate if Accelerate axis is negative
			{
				controller.SetDriveMode(DriveMode.Decelerate);		// apply to controller
			}
			else
			{
				controller.SetDriveMode(DriveMode.Neutral);			// apply neutral if axis is zero
			}

			// get lean based on mouse x axis, or value of Horizontal axis (keyboard or joystick) if not zero 
			if (Input.GetAxis("Horizontal") == 0f)	// if Horizontal axis is zero...
			{
				float mouseX = -1f * Input.GetAxis("Mouse X") * mouseSensitivity;           // calculate mouse movement
				controller.SetZRotation(controller.GetZRotation() + mouseX);				// apply new bank rotation
			}
			else
			{
				// Horizontal axis is non-zero; base new bank angle on it
				controller.SetZRotation(Input.GetAxis("Horizontal") * controller.GetClampAngleBoard());
			}
		}
		else
		{
			controller.SetDriveMode(DriveMode.Neutral);		// reset acceleration to zero if input is disabled
		}
	}
}
