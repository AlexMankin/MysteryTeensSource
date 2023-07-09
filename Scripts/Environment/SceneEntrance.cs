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

public class SceneEntrance : MonoBehaviour {

    public SceneData fromScene;
    public int ID;
    public CardinalDirection FacingDirection;

    private static List<SceneEntrance> entranceList;
    private static Helpers.EventVoid e_RegisterEntrances;


    void Awake() {
        if (GetComponent<SpriteRenderer>() != null)
            Destroy(GetComponent<SpriteRenderer>());

        e_RegisterEntrances += onRegisterEntrances;
    }

    void OnDestroy() {
        e_RegisterEntrances -= onRegisterEntrances;
    }

    public static void ResetEntrances() {
        if (entranceList == null)
            entranceList = new List<SceneEntrance>();

        entranceList.Clear();
        if (e_RegisterEntrances != null)
            e_RegisterEntrances();
    }

    private void onRegisterEntrances() {
        entranceList.Add(this);
    }

    public static SceneEntrance FindEntrance(int fromSceneIndex, int goalID = 0) {
        if (entranceList.Count <= 0)
            return null;

        foreach (SceneEntrance E in entranceList) {
            if (E.fromScene?.unityIndex == fromSceneIndex && goalID == E.ID) {
                return E;
            }
        }

        return null;
    }

    public CardinalDirection OppositeDirection {
        get {
            if (FacingDirection == CardinalDirection.Left)
                return CardinalDirection.Right;
            if (FacingDirection == CardinalDirection.Right)
                return CardinalDirection.Left;
            if (FacingDirection == CardinalDirection.Up)
                return CardinalDirection.Down;
            if (FacingDirection == CardinalDirection.Down)
                return CardinalDirection.Up;

            return CardinalDirection._NULL;
        }
    }
    public Vector2 Position {
        get { return transform.position; }
    }

}
