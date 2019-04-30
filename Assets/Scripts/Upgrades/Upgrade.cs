using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// upgrade base class
// upgrades are used by Controllers to determine things like
// drag, acceleration, and various model components
public class Upgrade : MonoBehaviour {
	[TextArea(5,10)]
	public string description;		// make the description a big text field
	public float cost;				// cost of the upgrade
	public float mass;				// mass of the upgrade

	// copy the upgrade (not really necessary, done when installing an upgrade)
	public void CopyUpgrade(Upgrade original)
	{
		name = original.name;
		description = original.description;
		cost = original.cost;
		mass = original.mass;
	}
}
