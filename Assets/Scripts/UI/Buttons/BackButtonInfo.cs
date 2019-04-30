using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// back button for information panel
public class BackButtonInfo : MonoBehaviour {
	public InformationPanel infoPanel;

	public void Back()
	{
		// check info panel script to see what the previous menu was,
		// and enable it when "Back" is called by menu button
		infoPanel.GetPreviousMenu().SetActive(true);
	}
}
