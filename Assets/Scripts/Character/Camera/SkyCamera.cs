using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sky camera renders items in between the scene and the skybox, like the moon and clouds
// it rotates with the main camera, but its movement is scaled down
// to simulate a greater draw distance
// it has a very far clipping plane, but only renders one or two objects
public class SkyCamera : MonoBehaviour {
	public GameObject mainCamera;			// copy main camera rotation

	// Update is called once per frame
	void LateUpdate () {
		transform.localPosition = mainCamera.transform.position;		// copy position (scaled down)
		transform.localRotation = mainCamera.transform.rotation;		// copy rotation
	}
}
