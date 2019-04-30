using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// not used; input manager used instead (see GameplaySettings for mouse sensitivity)
public class InputSettings : MonoBehaviour {

	public KeyCode forward;
	public KeyCode reverse;
	public KeyCode music;
	public KeyCode musicForward;
	public KeyCode musicBack;
	public KeyCode pauseMenu;

	public float mouseSensitivity = 15f;

}
