using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// force field upgrade
public class ForceField : Upgrade {
	public float dragCoefficient;		// associated aerodynamic drag
	public float dragCoefficientBreak;	// drag when breaking
	public float damping;				// collision damping
	public GameObject model;			// prefab model

	// copy upgrade information
	public void Copy(ForceField original)
	{
		CopyUpgrade(original);
		dragCoefficient = original.dragCoefficient;
		dragCoefficientBreak = original.dragCoefficientBreak;
		damping = original.damping;
		model = original.model;
	}
}
