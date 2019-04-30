using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class responsible for preventing 3rd person cameras from clipping through scene geometry
// attach to camera attachment node, rather than pivot node
public class CameraPlacer : MonoBehaviour {
	public LayerMask layerMask;				// geometry layer mask

	GameObject baseNode;					// camera pivot node
	Vector3 offsetDirection;				// camera offset direction
	float offsetDistance;					// camera offset distance
	bool active = false;					// camera active?


	// Use this for initialization
	void Start () {
		baseNode = transform.parent.gameObject;				// find base node
		Vector3 offset = transform.localPosition;			// get attachment node offset direction
		offsetDirection = Vector3.Normalize(offset);		// normalize offset direction
		offsetDistance = Vector3.Distance(Vector3.zero, offset);	// get attachment node offset distance
	}
	
	// LateUpdate is called once per frame, after world geometry has been updated
	void LateUpdate () {
		if (active)		// if camera is active...
		{
			RaycastHit hitInfo;
			// perform a raycast from camera pivot node towards camera attachment node
			if (Physics.Raycast(baseNode.transform.position, baseNode.transform.rotation * offsetDirection, out hitInfo, offsetDistance, layerMask.value))
			{
				// if it hits, place camera attachment node at hit point
				transform.localPosition = offsetDirection * hitInfo.distance;
			}
			else
			{
				// if it misses, place camera attachment node at original offset
				transform.localPosition = offsetDirection * offsetDistance;
			}
		}
	}

	// public setter to enable or disable raycasting (set by CameraSwitcher)
	public void SetCamActive(bool active)
	{
		this.active = active;
	}
}
