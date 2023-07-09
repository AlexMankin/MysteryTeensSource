using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class InitializeGame : MonoBehaviour {

	//VARIABLES
	private float vol;
    private bool resolutionAdjustedPrevFrame;
	private static bool isInitialized;
	private static int enteredFromScene;
   
	//CONSTANTS
	private const string ENTRY_SCENE_NAME = "0aLD";
	//EVENTS
	
	//METHODS
	
	void Start(){

		if (isInitialized) {
			Debug.LogWarning ("Game is trying to initialize again. Please avoid re-loading the ENTRY scene or make sure the Initialize Game script is not attached to anything in the scene.");
			Debug.LogWarning ("Attached object: " + gameObject.name);
			GameObject.Destroy (gameObject);
			return;
		}

        Application.targetFrameRate = Global.Misc.targetFPS;

		FB_Input.Initialize ();
		UserPrefs.Initialize ();
		sv.Initialize ();
		AudioDatabase.Initialize ();
		TextDatabase.Initialize ();	
		SceneCamera.Initialize ();
		FB_Debug.Initialize ();
        ActorRegistrar.Initialize();

        ZoomCamera.UpdateLetterbox();
        //test.SetAnimation (AnimJass.Walk);
        //StartCoroutine (crLoop ());


        isInitialized = true;

		if (enteredFromScene > 0) {
			SceneLoader.Load (enteredFromScene);
		} else {
			SceneLoader.Load (Global.Misc.titleScene);
		}

	}

	void Update(){
		//TODO: Move this somewhere else
        if (resolutionAdjustedPrevFrame) {
            //VERY OBNOXIOUS
            resolutionAdjustedPrevFrame = false;
            ZoomCamera.UpdateLetterbox();
        }

		if (FB_Input.ButtonDown(FB_Button.FullScreen)){

			if (Screen.fullScreen)
                Screen.SetResolution(GlobalMiscData.WIN_RESOLUTION_X / 2, GlobalMiscData.WIN_RESOLUTION_Y / 2, false);
            else if (Screen.width < GlobalMiscData.WIN_RESOLUTION_X)
                Screen.SetResolution(GlobalMiscData.WIN_RESOLUTION_X, GlobalMiscData.WIN_RESOLUTION_Y, false);
            else {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
            }

            resolutionAdjustedPrevFrame = true;
        }

        /*

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			if (Input.GetKey (KeyCode.L)) {
				//Load data
				sv.Load(0);
			} else {
				//Save data
				sv.Save(0);
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			if (Input.GetKey (KeyCode.L)) {
				//Load data
				sv.Load(1);
			} else {
				//Save data
				sv.Save(1);
			}
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (Input.GetKey (KeyCode.L)) {
				//Load data
				sv.Load(2);
			} else {
				//Save data
				sv.Save(2);
			}
		}
        

        
        if (Input.GetKeyDown(KeyCode.R)) {
            sv.Data.completed1cLB = !sv.Data.completed1cLB;
            Debug.Log("1cLB set to " + sv.Data.completed1cLB);
        }
            
        if (Input.GetKeyDown(KeyCode.T)) {
            sv.Data.completed2aBR = !sv.Data.completed2aBR;
            Debug.Log("2aBR set to " + sv.Data.completed2aBR);
        }
            
        if (Input.GetKeyDown(KeyCode.Y)) {
            sv.Data.completed2bGD = !sv.Data.completed2bGD;
            Debug.Log("2bGD set to " + sv.Data.completed2bGD);
        }
            
        if (Input.GetKeyDown(KeyCode.U)) {
            sv.Data.completed3aKT = !sv.Data.completed3aKT;
            Debug.Log("3aKT set to " + sv.Data.completed3aKT);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            sv.Data.completed3bGB = !sv.Data.completed3bGB;
            Debug.Log("3bGB set to " + sv.Data.completed3bGB);
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            sv.Data.completed3cGL = !sv.Data.completed3cGL;
            Debug.Log("3cGL set to " + sv.Data.completed3cGL);
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            sv.Data.completed3dBA = !sv.Data.completed3dBA;
            Debug.Log("3dBA set to " + sv.Data.completed3dBA);
        }

        */

    }

    private IEnumerator crLoop(){
		while (true) {
			yield return null;
		}
	}
		
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoadRuntimeMethod(){
		//If you launch outside of the entry scene, we need to jump there to initialize everything
		if (SceneManager.GetActiveScene ().name != ENTRY_SCENE_NAME) {
            enteredFromScene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene (0);
		}
	}

	public static bool IsInitialized{ get { return isInitialized; } }
}


