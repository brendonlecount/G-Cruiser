using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// impulse converter upgrade
public class ImpulseConverter : Upgrade {
	public float gMax;		// maximum g's pulled during a turn

	// copy upgrade information
	public void Copy(ImpulseConverter original)
	{
		CopyUpgrade(original);
		gMax = original.gMax;
	}
}
