using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// thruster upgrade
public class Thruster : Upgrade {
	public float thrust;		// axial thrust

	// copy upgrade data
	public void Copy(Thruster original)
	{
		CopyUpgrade(original);
		thrust = original.thrust;
	}
}
