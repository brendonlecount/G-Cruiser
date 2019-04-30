using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

// loads text from a text asset
public class TextLoader : MonoBehaviour {
	public TextAsset textFile;		// source text

	// Use this for initialization
	void Start () {
		if (textFile != null)
		{
			// set text to asset text
			GetComponent<Text>().text = textFile.text;
		}
	}
}
