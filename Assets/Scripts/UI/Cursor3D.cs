using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.VR;

// draws a 3D cursor on the screen (not at screen depth)
// in VR, cursor is drawn at center of screen (gaze cursor)
// not in VR, cursor is drawn at mouse position if it is within menu bounds
public class Cursor3D : MonoBehaviour {
	public GameObject cursorModel;		// prefab of cursor to instantiate
	public LayerMask mask;				// layer mask used for cursor plane raycast
	public Camera cam;					// camera used for screenspace calculations
	public string cameraTag;			// tag used to find camera if not assigned

	private GameObject cursor3D;		// instance of cursor
	EventSystem eventSystem;			// event system to use

	GraphicRaycaster gr;				// graphis raycaster
	bool hidden = false;				// hide the cursor?

	private void Start()
	{
		eventSystem = FindObjectOfType<EventSystem>();		// get the event system
		gr = GetComponent<GraphicRaycaster>();				// get the graphics raycaster
		if (cam == null)
		{
			// find the camera via cameraTag if not set
			cam = GameObject.FindWithTag(cameraTag).GetComponent<Camera>();
		}
	}

	private void OnEnable()
	{
		// instantiate the cursor model
		cursor3D = GameObject.Instantiate(cursorModel);
		if (GetIsVR())
		{
			// if in VR, freeze and hide the default cursor
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void OnDisable()
	{
		// destroy the cursor model
		GameObject.Destroy(cursor3D);
	}


	// LateUpdate is called once per frame
	void LateUpdate () {
		if (!hidden)
		{
			// position the cursor
			PositionCursor();
			// if in VR, manage UI element currently targeted by the gaze cursor
			if (GetIsVR())
			{
				InteractWithMenu();
			}
		}
	}

	// hide the cursor
	public void Hide(bool hidden = true)
	{
		this.hidden = hidden;
		if (hidden)
		{
			cursor3D.SetActive(false);
		}
	}

	Button activeButton;	// currently active button (determined by gaze cursor)

	// manage gaze cursor
	void InteractWithMenu()
	{
		if (eventSystem != null)	// if there is an event system in the scene...
		{
			PointerEventData ped = new PointerEventData(eventSystem);	// get its associated pointer event data
			ped.position = new Vector2((float)cam.pixelWidth * 0.5f, (float)cam.pixelHeight * 0.5f);	// set its position
			List<RaycastResult> results = new List<RaycastResult>();        // note: results[i].worldPosition doesn't work
			gr.Raycast(ped, results);		// perform the graphics raycast
			if (results.Count > 0)			// if anything was hit...
			{
				Button button = null;
				for (int i = 0; i < results.Count; i++)
				{
					// set button to the first button found in results
					button = results[i].gameObject.GetComponent<Button>();
					if (button != null)
					{
						break;
					}
				}
				if (activeButton != button)		// if a new button was found...
				{
					// update the active button and make it selected
					activeButton = button;
					if (activeButton != null)
					{
						activeButton.Select(); 
					}
				}
			}
			else
			{
				//			Debug.Log("Nothing hit" + ped.position);
			}
		}
	}

	// positions the cursor model, hiding it and enabling the default cursor if you are outside the menu bounds
	void PositionCursor()
	{
		Ray ray;		// direction of raycast
		if (GetIsVR())
		{
			// set ray to axis of camera if in VR
			ray = new Ray(cam.transform.position, cam.transform.rotation * Vector3.forward);
		}
		else
		{
			// set ray to screenpoint of default cursor if not in VR
			ray = cam.ScreenPointToRay(Input.mousePosition);
		}

		// perform raycast
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, mask.value))
		{
			// if cursor plane was hit, show cursor and put it at the hit point
			cursor3D.SetActive(true);
			Cursor.visible = false;
			cursor3D.transform.position = hitInfo.point;
		}
		else
		{
			// hide 3D cursor and show default cursor
			cursor3D.SetActive(false);
			Cursor.visible = true;
		}
	}
	
	// true if VR is enabled and active
	bool GetIsVR()
	{
		return UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRSettings.isDeviceActive;
	}
}
