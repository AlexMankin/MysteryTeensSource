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

public class DeployableAnimation : MonoBehaviour {


	//VARIABLES
	private static FB_Sprite[] spriteList;
	private static Mover[] MoverList;
	private static DeployableAnimation instance;

	private static Vector3 T3;

	//CONSTANTS
	private const int NUM_DEPLOYABLE_OBJECTS = 64;
	private const float Z_POSITON = 0f;

	void Awake(){
		instance = this;

		SceneLoader.E_LoadedBeforeFade += StopAll;
	}

	public static void Initialize(){
		generateObjects ();
	}

	void OnDestroy(){
		SceneLoader.E_LoadedBeforeFade -= StopAll;
	}

	public static int PlayAnimation(Vector2 pos, AnimData anim, SpriteLayer L = SpriteLayer.AboveSprite, Sprite spr = null){
		if (anim == null && spr == null) {
			return -1;
		}

		for (int i = 0; i < spriteList.Length; ++i) {
			if (!spriteList [i].IsPlaying) {
				T3.x = pos.x;
				T3.y = pos.y;
				T3.z = Z_POSITON;

				spriteList [i].transform.position = T3;
				if (anim != null)
					spriteList [i].SetAnimation (anim);
				else
					spriteList [i].SetAnimation (spr);
				spriteList [i].Layer = L;
				return i;
			}
		}

		Debug.LogWarning ("Could not play animation " + anim.SheetName + "." + anim.Name + ".  Consider increasing deployable capacity.");
		return -1;
	}
	public static int PlayAnimation(Vector2 pos, Sprite spr, SpriteLayer L = SpriteLayer.AboveSprite){
		return PlayAnimation (pos, null, L, spr);
	}

	public static bool IsPlaying(int index){
		return spriteList [index].IsPlaying;
	}

	public static FB_Sprite GetSprite(int index){
		return spriteList [index];
	}

	public static Mover GetMover(int index){
		if (index < 0 || index > spriteList.Length) {
			Debug.LogWarning ("Index " + index + " out of range for Deployable Sprite list.");
			return null;
		}

		if (!spriteList [index].IsVisible) {
			Debug.LogWarning ("Sprite " + index + " not active.");
			return null;
		}
		
		return MoverList [index];
	}

	public static void StopAnimation(int index){
		if (index >= 0) {
			spriteList [index].Disable ();
			MoverList [index].Stop ();
		}
	}

	public static void StopAll(){
		for (int i = 0; i < spriteList.Length; ++i) {
			spriteList [i].Disable ();
			MoverList [i].Stop ();
		}
	}

	private static void generateObjects(){
		spriteList = new FB_Sprite[NUM_DEPLOYABLE_OBJECTS];
		MoverList = new Mover[NUM_DEPLOYABLE_OBJECTS];
		GameObject O;

		for (int i = 0; i < spriteList.Length; ++i) {
			O = new GameObject ();
			O.name = "Deployable Animation " + i;
            O.AddComponent<Actor>();
			O.transform.parent = instance.transform;
			spriteList[i] = O.AddComponent<FB_Sprite> ();
			MoverList [i] = O.AddComponent<Mover> ();
		}
	}
}
