using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages purchaseable upgrades
public class UpgradeShop : MonoBehaviour {
	Upgrade[] upgrades;		// list of all upgrades , populated from gameobject's children

	// turns a string name into an upgrade
	public Upgrade GetUpgrade(string upgradeName)
	{
		// initialize upgrades if it hasn't been already
		if (upgrades == null)
		{
			upgrades = GetComponentsInChildren<Upgrade>();	// find Upgrade components in children
		}
		// for each upgrade in upgrades...
		for(int i = 0; i < upgrades.Length; i++)
		{
			if (upgrades[i].name == upgradeName)
			{
				// if the name matches, return the upgrade
				return upgrades[i];
			}
		}
		return null;	// otherwise return null
	}
}
