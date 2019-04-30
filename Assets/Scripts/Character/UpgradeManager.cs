using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// class responsible for managing installed and purchased upgrades and account balance; attached to player and racers
public class UpgradeManager : MonoBehaviour {
	public Controller controller;				// associated controller
	public TextAsset startingUpgrades;			// list of starting upgrades, if any

	public float balance = 0f;					// starting balance (overridden by startingUpgrades if present)

	List<string> ownedUpgrades;					// list of owned upgrades

	UpgradeShop upgradeShop;					// database of potential upgrades

	// currently installed upgrades
	Chassis chassis;				
	ForceField forceField;
	GSuit gSuit;
	ImpulseConverter impulseConverter;
	Retrothruster retrothruster;
	Thruster thruster;

	string activeSaveFile = "";			// path and name of active save file, set by LoadFromSave


	// Use this for initialization
	void Start () {
		// initialize list
		ownedUpgrades = new List<string>();

		// find starting upgrades (used if no upgradeShop or saved data are present)
		chassis = GetComponentInChildren<Chassis>();
		forceField = GetComponentInChildren<ForceField>();
		gSuit = GetComponentInChildren<GSuit>();
		impulseConverter = GetComponentInChildren<ImpulseConverter>();
		retrothruster = GetComponentInChildren<Retrothruster>();
		thruster = GetComponentInChildren<Thruster>();

		// find upgradeShop if present
		upgradeShop = FindObjectOfType<UpgradeShop>();

		if (upgradeShop == null || startingUpgrades == null)		// if no upgrade shop or saved data are present
		{
			// add found upgrades to ownedUpgrades list
			PurchaseUpgrade(chassis, false, false);
			PurchaseUpgrade(forceField, false, false);
			PurchaseUpgrade(gSuit, false, false);
			PurchaseUpgrade(impulseConverter, false, false);
			PurchaseUpgrade(retrothruster, false, false);
			PurchaseUpgrade(thruster, false, false);

			// install found upgrades
			controller.InstallChassis(chassis);
			controller.InstallForceField(forceField);
			controller.InstallGSuit(gSuit);
			controller.InstallImpulseConverter(impulseConverter);
			controller.InstallRetrothruster(retrothruster);
			controller.InstallThruster(thruster);
		}
		else
		{
			// otherwise, load upgrades from upgradeStore based on saved data
			LoadFromAsset(startingUpgrades);
		}
	}

	// reload upgrade data from save file
	public void LoadFromSave(string saveFile)
	{
		StreamReader reader = new StreamReader(saveFile);
		if (reader != null)
		{
			string saveData = reader.ReadToEnd();
			reader.Close();
			activeSaveFile = saveFile;
			LoadFromText(reader.ReadToEnd());
		}
	}

	// reload upgrade data from text asset
	public void LoadFromAsset(TextAsset saveAsset)
	{
		LoadFromText(saveAsset.text);
	}

	// reload upgrade data from string (can be from text asset or save file)
	void LoadFromText(string saveText)
	{
		ownedUpgrades.Clear();										// reset owned upgrades

		string[] separator = { System.Environment.NewLine };		// new line delimeter for parsing
		// parse save data into array of lines
		string[] saveData = saveText.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);

		int dataIndex = 0;

		// skip to "Balance"
		while (dataIndex < saveData.Length && saveData[dataIndex] != "Balance")
		{
			dataIndex++;
		}
		// read balance
		dataIndex++;
		if (dataIndex < saveData.Length)
		{
			balance = float.Parse(saveData[dataIndex]);
		}

		// skip to "Installed"
		while (dataIndex < saveData.Length && saveData[dataIndex] != "Installed")
		{
			dataIndex++;
		}
		// read installed
		dataIndex++;
		while (dataIndex < saveData.Length && saveData[dataIndex] != "Purchased")
		{
			InstallUpgrade(upgradeShop.GetUpgrade(saveData[dataIndex++]), false);
		}

		// read purchased upgrades
		dataIndex++;
		while (dataIndex < saveData.Length)
		{
			PurchaseUpgrade(upgradeShop.GetUpgrade(saveData[dataIndex++]), false, false);
		}
	}

	// save balance and upgrade data to saveFile
	public void Save(string saveFile)
	{
		if (saveFile != "")
		{
			activeSaveFile = saveFile;
			Save();
		}
	}

	// save balance and upgrade data to activeSaveFile (if set)
	void Save()
	{
		if (activeSaveFile != "")
		{
			StreamWriter writer = new StreamWriter(activeSaveFile);
			if (writer != null)
			{
				writer.Write(GenerateSaveText());
				writer.Close();
			}

		}
	}

	// generate save text based on balance and upgrade data
	string GenerateSaveText()
	{
		string nl = System.Environment.NewLine;

		string saveData = "";

		// write balance data
		saveData += "Balance" + nl;
		saveData += balance + nl;

		// write installed upgrade data
		saveData += "Installed" + nl;
		saveData += chassis.name + nl;
		saveData += forceField.name + nl;
		saveData += gSuit.name + nl;
		saveData += impulseConverter.name + nl;
		saveData += retrothruster.name + nl;
		saveData += thruster.name + nl;

		// write purchased upgrade data
		saveData += "Purchases" + nl;
		for (int i = 0; i < ownedUpgrades.Count; i++)
		{
			saveData += ownedUpgrades[i] + nl;
		}

		return saveData;
	}


	// purchase an upgrade (add to ownedUpgrades and deduct cost from balance)
	// upgrade: upgrade to purchase
	// charge: deduct cost from balance?
	// save: save new upgrade data to file when done?
	public void PurchaseUpgrade(Upgrade upgrade, bool charge = true, bool save = true)
	{
		if (upgrade == null)
		{
			return;
		}

		if (!charge || balance >= upgrade.cost)
		{
			if (!ownedUpgrades.Contains(upgrade.name))
			{
				ownedUpgrades.Add(upgrade.name);
				if (charge)
				{
					balance -= upgrade.cost;
				}
			}
		}

		if (save)
		{
			Save();
		}
	}

	// is an upgrade owned? (in ownedUpgrades)
	public bool IsOwned(Upgrade upgrade)
	{
		if (upgrade == null)
		{
			return false;
		}
		return ownedUpgrades.Contains(upgrade.name);
	}

	// is an upgrade installed?
	public bool IsInstalled(Upgrade upgrade)
	{
		if (upgrade == null)
		{
			return false;
		}
		else if (chassis.name == upgrade.name || forceField.name == upgrade.name || gSuit.name == upgrade.name || impulseConverter.name == upgrade.name || retrothruster.name == upgrade.name || thruster.name == upgrade.name)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	// install upgrade (purchase if not owned, apply stats to controller, save if indicated)
	public void InstallUpgrade(Upgrade upgrade, bool save = true)
	{
		if (upgrade == null)
		{
			return;
		}

		PurchaseUpgrade(upgrade, false);			// purchase if unowned

		// is it a chassis? install as chassis if so
		Chassis chassis = upgrade.GetComponent<Chassis>();
		if (chassis != null)
		{
			this.chassis.Copy(chassis);
			controller.InstallChassis(this.chassis);
			return;
		}

		// repeat for each category of upgrade until category is found
		ForceField forceField = upgrade.GetComponent<ForceField>();
		if (forceField != null)
		{
			this.forceField.Copy(forceField);
			controller.InstallForceField(this.forceField);
			return;
		}

		GSuit gSuit = upgrade.GetComponent<GSuit>();
		if (gSuit != null)
		{
			this.gSuit.Copy(gSuit);
			controller.InstallGSuit(this.gSuit);
			return;
		}

		ImpulseConverter impulseConverter = upgrade.GetComponent<ImpulseConverter>();
		if (impulseConverter != null)
		{
			this.impulseConverter.Copy(impulseConverter);
			controller.InstallImpulseConverter(this.impulseConverter);
			return;
		}

		Retrothruster retrothruster = upgrade.GetComponent<Retrothruster>();
		if (retrothruster != null)
		{
			this.retrothruster.Copy(retrothruster);
			controller.InstallRetrothruster(this.retrothruster);
			return;
		}

		Thruster thruster = upgrade.GetComponent<Thruster>();
		if (thruster != null)
		{
			this.thruster.Copy(thruster);
			controller.InstallThruster(this.thruster);
			return;
		}

		// save new upgrade data if indicated
		if (save)
		{
			Save();
		}
	}
}
