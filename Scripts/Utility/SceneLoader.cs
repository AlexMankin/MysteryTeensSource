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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    //VARIABLES
    [SerializeField] private Image faderImg;
    private static bool isLoading;
    private static SceneLoader instance;
    private static ColorAdjustment fader;
    private static int previousSceneIndex, currentSceneIndex;
    private static Vector2 T2;
    //CONSTANTS
    public const float DURATION_FADE = 0.5f;
    //EVENTS
    public static Helpers.EventVoid E_ReadyToLoadAfterFade, E_LoadedBeforeFade, E_LoadedAfterFade;

    //METHODS

    void Awake() {
        instance = this;
        fader = new ColorAdjustment(faderImg, this);
        fader.AdjustAlpha(0f);
        fader.AdjustColor(Color.black);
        sv.Data.currentSceneIndex = 0;
    }

    public static void Load (int sceneIndex, Vector2 atPosition, Vector2 facing) {
        instance.StartCoroutine(crLoadScene(sceneIndex, 0, atPosition, facing));
    }

    public static void Load (int sceneIndex, int entranceID = 0) {
        instance.StartCoroutine(crLoadScene(sceneIndex, entranceID, new Vector2(-1f, -1f), Vector2.zero));
    }

    public static void Load(SceneData S, int entranceID = 0) {
        if (S == null)
            return;

        instance.StartCoroutine(crLoadScene(S.unityIndex, entranceID, new Vector2(-1f, -1f), Vector2.zero));
    }
    public static void Load(SceneData S, Vector2 atPosition, Vector2 facing) {
        if (S == null)
            return;

        instance.StartCoroutine(crLoadScene(S.unityIndex, 0, atPosition, facing));
    }

    public static IEnumerator RefreshScene() {
        Load(currentSceneIndex, Player.PlayableActor.Position, Player.PlayableActor.ForwardVector);
        yield return null;
        yield return new WaitUntil(() => !ScreenWipe.IsWiping);

    }

    private static IEnumerator crLoadScene(int destinationSceneIndex, int entranceID, Vector2 atPosition, Vector2 facing) {

        isLoading = true;
       
        //Player control is lost, screen begins fading
        fader.AdjustAlpha(1f, DURATION_FADE);
        yield return new WaitUntil(() => !fader.IsFading);

        SceneCamera.LockX = false;
        SceneCamera.LockY = false;

        E_ReadyToLoadAfterFade?.Invoke();

        yield return null;

        if (!sv.Data.Sys_LoadingSaveData 
        && destinationSceneIndex != Global.Misc.titleScene.unityIndex 
        && destinationSceneIndex != Global.Misc.masterBedroomScene.unityIndex) {
            previousSceneIndex = sv.Data.currentSceneIndex;
            sv.Data.previousSceneIndex = sv.Data.currentSceneIndex;
            sv.Data.currentSceneIndex = destinationSceneIndex;
            sv.Data.Sys_PlayerPositionX = -10f;
            sv.Save(0, false);
        }
        else {
            previousSceneIndex = sv.Data.previousSceneIndex;
        }

        SceneManager.LoadScene(destinationSceneIndex);

        currentSceneIndex = destinationSceneIndex;

        yield return null;
        //New scene is loaded, screen is still black
        ActorRegistrar.CallForRegistration();
        SceneEntrance.ResetEntrances();

        SceneEntrance spawnAtEntrance = null;

        yield return null;

        if (sv.Data.Sys_LoadingSaveData && sv.Data.Sys_PlayerPositionX >= 0f) {
            if (Player.PlayableActor != null) {
                T2.x = sv.Data.Sys_PlayerPositionX;
                T2.y = sv.Data.Sys_PlayerPositionY;
                Player.PlayableActor.transform.position = T2;
            }
        }
        else {
            if (atPosition.x > 0f && facing != Vector2.zero) {
                if (Player.PlayableActor != null) {
                    T2.x = atPosition.x;
                    T2.y = atPosition.y;
                    Player.PlayableActor.transform.position = T2;

                    T2.x = facing.x;
                    T2.y = facing.y;
                    Player.PlayableActor.forward = T2.ToCardinal();
                }
            }

            Debug.Log(sv.Data.previousSceneIndex);

            spawnAtEntrance = SceneEntrance.FindEntrance(sv.Data.previousSceneIndex, entranceID);
            if (spawnAtEntrance != null && Player.PlayableActor != null) {
                Player.PlayableActor.transform.position = spawnAtEntrance.Position;

                if (spawnAtEntrance.FacingDirection != CardinalDirection._NULL) {
                    Player.PlayableActor.forward = spawnAtEntrance.FacingDirection;
                }
            }
        }

        sv.Data.Sys_LoadingSaveData = false;
        Ghost.Register();

        E_LoadedBeforeFade?.Invoke();

        if (!FB_Audio.LockBGM && Map.current != null)
            FB_Audio.PlayBGM(Map.current.defaultBGM);

        yield return null;

        yield return ScreenWipe.UnWipe(spawnAtEntrance ? spawnAtEntrance.OppositeDirection : CardinalDirection.Left, WipeType.Cloudy);
        yield return new WaitUntil(() => !ScreenWipe.IsWiping);

        isLoading = false;

        //Screen is no longer black, player control is about to return
        E_LoadedAfterFade?.Invoke();
    }

    public static int PreviousScene => previousSceneIndex;
    public static int CurrentScene => currentSceneIndex;
    public static bool IsLoading {
        get {
            return isLoading;
        }
    }
}
