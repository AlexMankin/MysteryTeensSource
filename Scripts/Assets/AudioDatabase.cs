//
//    `````              Code founded by Alex Mankin                       
// .:///////:-.                               -.    
//./////////////:-`          `....``    ```.-://:-` 
//`/////////////////:.``  .:///////////////////+yy/-
//  `..---:://////////////////////////////////::::.`
//           `-://////////////////////////////`     
//                ```````.//////::::--..-////.      
//                      -//////-       `--``-/`     
//                      -//:.`         .     ./.    
//                        -::.                ``  
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioDatabase : MonoBehaviour {

	//VARIABLES
	private static Dictionary<BGM, AudioClip> bgmList;
	private static Dictionary<SFX, AudioClip> sfxList;


	//CONSTANTS
	private const string DIR_BGM = "BGM/", DIR_SFX = "SFX/";
	//EVENTS
	
	//METHODS

	public static void Initialize(){
		if (IsInitialized)
			return;
		
		string[] names;
		bgmList = new Dictionary<BGM, AudioClip> ();
		AudioClip clip;

		names = Enum.GetNames (typeof(BGM));
		foreach (string S in names) {
			if (!S.Equals ("_NULL")) {
				clip = Resources.Load (DIR_BGM + S) as AudioClip;
				if (clip == null) {
					Debug.LogWarning ("Could not load BGM + " + S + ".  Please run Mystery Teens > Refresh Asset Database");
				} else {
					bgmList.Add ((BGM)Enum.Parse (typeof(BGM), S), clip);
				}
			}
		}

		names = Enum.GetNames (typeof(SFX));
		sfxList = new Dictionary<SFX, AudioClip> ();
		foreach (string S in names) {
			if (!S.Equals ("_NULL")) {
				clip = Resources.Load (DIR_SFX + S) as AudioClip;
				if (clip == null) {
					Debug.LogWarning ("Could not load SFX + " + S + ".  Please run Mystery Teens > Refresh Asset Database");
				} else {
					sfxList.Add ((SFX)Enum.Parse (typeof(SFX), S), clip);
				}
			}
		}
	}

	public static AudioClip GetBGM(BGM id){
		return bgmList[id];
	}

	public static AudioClip GetSFX(SFX id){
		return sfxList[id];
	}

	public static bool IsInitialized{get{return bgmList != null && sfxList != null;}}
}
