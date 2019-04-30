using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// button for purchasing upgrades
public class PurchaseButton : MonoBehaviour {
	public InformationPanel informationPanel;

	// purchase upgrade and refresh information panel with latest info
	public void Purchase()
	{
		informationPanel.GetManager().PurchaseUpgrade(informationPanel.GetUpgrade());
		informationPanel.Refresh();
	}
}
