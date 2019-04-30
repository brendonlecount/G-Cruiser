using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// the information panel displays information about an upgrade
public class InformationPanel : MonoBehaviour {
	// outputs for upgrade information
	public Text balance;
	public Text upgradeName;
	public Text description;
	public Text status;
	public Text cost;
	public Text mass;
	// buttons are shown and hidden contextually
	public Button installButton;
	public Button purchaseButton;

	UpgradeManager manager;		// for installing upgrades

	// set by calling menu (upgrade button)
	Upgrade upgrade;
	GameObject previousMenu;

	public void OnEnable()
	{
		if (manager == null)
		{
			manager = GameObject.FindWithTag("Player Upgrades").GetComponent<UpgradeManager>();
		}
		Refresh();
	}

	// looks at upgrade information and output its data to the information menu
	public void Refresh()
	{
		// output data
		balance.text = "$" + manager.balance.ToString("n0");
		upgradeName.text = upgrade.gameObject.name;
		description.text = upgrade.description;
		cost.text = "$" + upgrade.cost.ToString("n0");
		mass.text = upgrade.mass.ToString("n1") + " kg";
		// output status (owned/unowned/installed)
		if (manager.IsInstalled(upgrade))
		{
			status.text = "Installed";
		}
		else if (manager.IsOwned(upgrade))
		{
			status.text = "Owned";
		}
		else
		{
			status.text = "Unowned";
		}
		// contextually enable or disable install and purchase buttons based on upgrade status
		installButton.gameObject.SetActive(!manager.IsInstalled(upgrade) && manager.IsOwned(upgrade));
		purchaseButton.gameObject.SetActive(!manager.IsOwned(upgrade) && manager.balance >= upgrade.cost);
	}

	// lets UpgradeButton set the currently displayed upgrade
	public void SetUpgrade(Upgrade upgrade)
	{
		this.upgrade = upgrade;
	}

	// lets other buttons (purchase and install) get the currently active upgrade
	public Upgrade GetUpgrade()
	{
		return upgrade;
	}

	// used by back button, set by UpgradeButton
	public void SetPreviousMenu(GameObject menu)
	{
		previousMenu = menu;
	}

	// get previously active menu (used by back button)
	public GameObject GetPreviousMenu()
	{
		return previousMenu;
	}

	// used by buttons to get ahold of upgrade manager for upgrade purchase or install
	public UpgradeManager GetManager()
	{
		return manager;
	}
}
