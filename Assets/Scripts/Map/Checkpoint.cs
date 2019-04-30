using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// empty script that allows checkpoints to be distinguished from other inanimate course geometry
public class Checkpoint : MonoBehaviour {
	// Use this for initialization
	// checkpoint registration now handled by Course script
//	void Start () {
//		GameObject.Find("Player").GetComponent<Timer>().RegisterCheckpoint();
//		GameObject.FindGameObjectWithTag("Body").GetComponent<Timer>().RegisterCheckpoint(transform.parent.gameObject);
//	}
}
