using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// class responsible for arrow key and controller navigation of menu buttons
// to be attached to each menu panel
public class MenuNavigator : MonoBehaviour
{
	Button[] buttons;		// list of buttons making on panel

	int activeButton;		// index of currently selected button

	private void OnEnable()
	{
		if (buttons == null)	// if it hasn't been done already...
		{
			// find out how many buttons there are
			Button[] temp = GetComponentsInChildren<Button>(true);
			// initialize buttons array to that number
			buttons = new Button[temp.Length];
			int buttonIndex = 0;
			// for each child of the canvas, in order...
			for (int i = 0; i < transform.childCount; i++)
			{
				// if it's a button, add it to buttons and increment button count
				Button nextChild = transform.GetChild(i).GetComponent<Button>();
				if (nextChild != null)
				{
					buttons[buttonIndex] = nextChild;
					buttonIndex++;
				}
			}
		}

		controlDown = true;		// assume selection key is still being pressed after menu switch
		activeButton = -1;
	}

	bool controlDown;			// ensures key presses are registered only once

	// manage inpute (up/down button navigation)
	private void Update()
	{
		if (Input.GetAxisRaw("Menu Navigation") > 0f)	// if up first pressed...
		{
			// go to previous button
			if (!controlDown)
			{
				controlDown = true;
				PreviousButton();
			}
		}
		else if (Input.GetAxisRaw("Menu Navigation") < 0f)	// if down first pressed
		{
			// go to next button
			if (!controlDown)
			{
				controlDown = true;
				NextButton();
			}
		}
		else
		{
			controlDown = false;	// reset controlDown
		}
	}

	// determine index of next button, and select it
	void NextButton()
	{
		if (activeButton < 0 || activeButton >= buttons.Length)
		{
			activeButton = NextActiveButton(0);
		}
		else
		{
			activeButton = NextActiveButton(activeButton + 1);
		}
		if (activeButton >= 0)
		{
			buttons[activeButton].Select();
		}
	}

	// determine index of previous button, and select it
	void PreviousButton()
	{
		if (activeButton < 0 || activeButton == 0)
		{
			activeButton = PreviousActiveButton(buttons.Length - 1);
		}
		else
		{
			activeButton = PreviousActiveButton(activeButton - 1);
		}
		if (activeButton >= 0)
		{
			buttons[activeButton].Select();
		}
	}

	// messy functions that loop through the buttons array trying to find an active button
	int NextActiveButton(int startPosition)
	{
		if (startPosition < 0)
		{
			startPosition = buttons.Length - 1;
		}
		else if (startPosition >= buttons.Length)
		{
			startPosition = 0;
		}
		for (int i = startPosition; i < buttons.Length; i++)
		{
			if (buttons[i].gameObject.activeInHierarchy)
			{
				return i;
			}
		}
		for (int i = 0; i < startPosition; i++)
		{
			if (buttons[i].gameObject.activeInHierarchy)
			{
				return i;
			}
		}
		return -1;
	}

	int PreviousActiveButton(int startPosition)
	{
		if (startPosition < 0)
		{
			startPosition = buttons.Length - 1;
		}
		else if (startPosition >= buttons.Length)
		{
			startPosition = 0;
		}
		for (int i = startPosition; i >= 0; i--)
		{
			if (buttons[i].gameObject.activeInHierarchy)
			{
				return i;
			}
		}
		for (int i = buttons.Length - 1; i > startPosition; i--)
		{
			if (buttons[i].gameObject.activeInHierarchy)
			{
				return i;
			}
		}
		return -1;
	}
}