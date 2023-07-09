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
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GlobalMisc", menuName = "Mystery Teens/Global/Misc")]
public class GlobalMiscData : ScriptableObject {

    //VARIABLES
    public SceneData titleScene, firstScene, masterBedroomScene;
    public int targetFPS = 60;
    public Color ColorHealthFull, ColorHealthDamaged, ColorHealthCritical;
    public Scene exposedScene;
    public AnimationCurve hopCurve;
    public AnimationCurve uiCounterTabAnimation;
    public PersonData nullPersonData;
    public AllPersons defaultGhost;
    
    public GameObject ActorPrefab;

    [Header("Battle")]
    public GameObject BattleCursorPrefab;
    public GameObject DamageTextPrefab;
    
    public static float GoalAspectRatio => (float)WIN_RESOLUTION_X / (float)WIN_RESOLUTION_Y;
    public static float CurrentAspectRatio => (float)Screen.width / (float)Screen.height;
    //CONSTANTS
    public const int PPU = 1;
    public const int WIN_RESOLUTION_X = 960, WIN_RESOLUTION_Y = 540;

    public const string ENUM_NULL_STRING = "_NULL",
    FILE_EXT_ASSET = ".asset",
    FILE_EXT_SCENE = ".unity";
    //EVENTS

    //METHODS

    //PROPERTIES
}
