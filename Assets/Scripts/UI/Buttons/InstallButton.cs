using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// upgrade install button script
public class InstallButton : MonoBehaviour {
	public InformationPanel informationPanel;

	// install upgrade and refresh information panel with latest info
	public void Install()
	{
		informationPanel.GetManager().InstallUpgrade(informationPanel.GetUpgrade());
		informationPanel.Refresh();
	}
}
