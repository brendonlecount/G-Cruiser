using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// retrothruster upgrade
public class Retrothruster : Upgrade {
	public float thrust;		// axial thrust (reverse)

	// copy upgrade data
	public void Copy(Retrothruster original)
	{
		CopyUpgrade(original);
		thrust = original.thrust;
	}
}
