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

/// <summary>
/// SCENE EXIT
/// 
/// Place a Game Object containing this behaviour in the scene to declare a spawn position.
/// </summary>
[AddComponentMenu("Futurebound/Scene Exit")]
public class SceneExit : MonoBehaviour {

    public SceneData destinationScene;
    //public TransitionType Transition;
    public SFX SoundEffect;				/// <summary>
	public int ID;
    public CardinalDirection intoDirection;

    //private static SCENE activatedDestinationScene;
    //private static TransitionType activatedTransition;

    void Awake() {

        if (GetComponent<IGameAction>() == null)
            gameObject.AddComponent<Actions.a000_Exit>();
    }

    private void OnDrawGizmos() {
        if (Box == null)
            return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube((Vector2)transform.position + Box.offset, Box.bounds.size);

    }

    public IEnumerator LoadSceneAction() {
        if (destinationScene == null)
            yield break;

        FB_Audio.PlaySFX(SoundEffect);
        SceneLoader.Load(destinationScene, ID);
        yield break;
    }

    private BoxCollider2D Box => GetComponent<BoxCollider2D>();
}
