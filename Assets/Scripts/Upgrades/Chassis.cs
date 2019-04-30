using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// chassis upgrade
public class Chassis : Upgrade {
	public GameObject model;	// prefab representing chassis

	// copy upgrade data (not really necessary)
	public void Copy(Chassis original)
	{
		CopyUpgrade(original);
		model = original.model;
	}
}
