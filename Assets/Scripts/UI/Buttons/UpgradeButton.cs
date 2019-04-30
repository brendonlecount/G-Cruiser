using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// upgrade button (preps information panel to display associated upgrade information)
public class UpgradeButton : MonoBehaviour {
	public Upgrade upgrade;			// associated upgrade
	public Color installedColor;	// color of installed upgrade text
	public Color defaultColor;		// color of uninstalled upgrade text

	public InformationPanel informationPanel;	// information panel, holds and displays upgrade information

	UpgradeManager manager;			// player's upgrade manager

	private void OnEnable()
	{
		if (manager == null)
		{
			// find manager if not yet set
			manager = GameObject.FindWithTag("Player Upgrades").GetComponent<UpgradeManager>();
		}

		Text text = gameObject.GetComponentInChildren<Text>();
		text.text = upgrade.name;			// set button text to upgrade name
		// format button based on owned and installed status
		if (manager.IsOwned(upgrade))
		{
			text.fontStyle = FontStyle.Bold;
		}
		else
		{
			text.fontStyle = FontStyle.Normal;
		}
		if (manager.IsInstalled(upgrade))
		{
			text.color = installedColor;
		}
		else
		{
			text.color = defaultColor;
		}
	}

	// when pressed, present upgrade to informationPanel and set previous menu for information panel back button
	public void UpgradeClicked()
	{
		informationPanel.SetUpgrade(upgrade);
		informationPanel.SetPreviousMenu(transform.parent.gameObject);
	}
}
