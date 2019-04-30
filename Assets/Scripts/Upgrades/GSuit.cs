using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// g-suit upgrade
public class GSuit : Upgrade {
	public float gFactor = 1f;		// ratio of g's felt (effects G-LOC)
	public float knockoutAcceleration = 1000f;	// acceleration above which player is knocked out

	// copy data
	public void Copy(GSuit original)
	{
		CopyUpgrade(original);
		gFactor = original.gFactor;
		knockoutAcceleration = original.knockoutAcceleration;
	}
}
