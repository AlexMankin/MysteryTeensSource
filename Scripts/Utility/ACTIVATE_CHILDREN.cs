using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACTIVATE_CHILDREN : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < transform.childCount; ++i) {
			if (!transform.GetChild (i).gameObject.activeSelf)
				transform.GetChild (i).gameObject.SetActive (true);
		}
	}
}
