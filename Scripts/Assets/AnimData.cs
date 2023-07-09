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

[CreateAssetMenu(fileName = "Anm_", menuName = "Mystery Teens/AnimData")]
[System.Serializable]
public class AnimData : ScriptableObject {

	//VARIABLES
	public string Name;
	public string SheetName;
	public AnimFrame[] FrameList;
	public bool Loop = true;
	//CONSTANTS
	
	//EVENTS
	
	//METHODS
	
	public AnimData(string n){
		name = n;
	}

	public override bool Equals(object obj){
		AnimData other = obj as AnimData;

		if (other == null)
			return false;

		return (string.Equals (Name, other.Name) && string.Equals (SheetName, other.SheetName));
	}

	public float Duration{
		get{
			float ret = 0f;

			for (int i = 0; i < FrameList.Length; ++i)
				ret += FrameList [i].Duration;

			return ret;
		}
	}



}

[System.Serializable]
public struct AnimFrame{
	public Sprite Sprite;
	public float Duration;
    public SFX SoundEffect;
	public bool LoopFromHere;
}
	
