using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// load/save button for start menu (saves submenu)
// to-do: implement a save button
// to-do: load/save other racer data
public class SaveButton : MonoBehaviour {
	public TextAsset saveAsset;

	UpgradeManager manager;

	// Use this for initialization
	void Start () {
		// find the player's upgrade manager
		manager = GameObject.FindWithTag("Player Upgrades").GetComponent<UpgradeManager>();
	}

	// when pressed, load upgrade data from the associated save asset
	public void LoadFromAsset()
	{
		manager.LoadFromAsset(saveAsset);
	}
}
