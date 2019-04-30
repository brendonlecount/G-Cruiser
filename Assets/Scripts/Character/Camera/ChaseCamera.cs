using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// class attached to chase camera node; keeps it turning with player, without banking or pitching
public class ChaseCamera : MonoBehaviour {
	public Controller controller;			// applicable controller object, for getting turn rotation

	
	// Update is called once per frame
	void Update ()
	{
		transform.rotation = controller.GetTurnRotation();		// apply turn rotation
	}
}
