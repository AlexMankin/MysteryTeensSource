using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_Debug : MonoBehaviour {

	//VARIABLES
	public bool Enabled, MuteBGM;

	private static FB_Debug instance;
	//CONSTANTS
	
	//EVENTS
	
	//METHODS

	void Awake(){
		instance = this;
	}

	public static void Initialize(){
		if (!instance.Enabled)
			return;

		if (instance.MuteBGM)
			FB_Audio.VolumeSettingBGM = 0f;
	}

}
