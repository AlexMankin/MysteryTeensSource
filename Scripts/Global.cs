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

public class Global : MonoBehaviour {

    //VARIABLES
    public GlobalMiscData misc;
    public GlobalAnimationData anim;
    public GlobalSpriteData sprites;


    public GameObject prefabDamageText;
    private static Global instance;
    //CONSTANTS
    public const string FILE_EXT_ASSET = ".asset",
                        FILE_EXT_TXT = ".txt",
                        FILE_EXT_MFS = ".mfs";

    public const float DEFAULT_WALKING_SPEED = 140f;

    //EVENTS

    //METHODS

    void Awake() {
        instance = this;
    }

    public static IEnumerator TeenEnterDoor(Actor teen, Actor door, bool doorAlreadyOpen = false) {
        Vector2 goalPosition = door.Position;
        goalPosition.y += 10f;

        if (!doorAlreadyOpen) {
            door.SetAnimation(Global.Animations.doorOpen);
            FB_Audio.PlaySFX(SFX.Menu_ShoulderClick);
            yield return new WaitForSeconds(0.5f);
        }

        teen.mover.MoveToPositionInSeconds(goalPosition, 0.5f);
        yield return new WaitUntil(() => teen.Position.y >= door.Position.y);

        teen.sprite.Adjuster.AdjustAlpha(0f, 0.2f);
        yield return new WaitForSeconds(0.2f);
    }


    //PROPERTIES
    public static GlobalAnimationData Animations => instance.anim;
    public static GlobalSpriteData Sprites => instance.sprites;
    public static GlobalMiscData Misc => instance.misc;

    public static GameObject Prefab_DamageText => instance.prefabDamageText;
}
