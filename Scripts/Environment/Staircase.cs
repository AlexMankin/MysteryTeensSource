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

public class Staircase : MonoBehaviour {

    //VARIABLES
    public StaircaseAscendingDirection AscendingDirection;

    private float signStair, signMover;
    private static Vector2 T2;

    //PROPERTIES
    private Collider2D col => GetComponent<Collider2D>();

    //FUNCTIONS
    void LateUpdate() {

        foreach (Actor A in ActorRegistrar.AllActorsInScene) {
            if (A.mover != null && A.person != null) {
                if (!A.mover.ignoreStaircases && col.OverlapPoint(A.Position)) {
                    signStair = (AscendingDirection == StaircaseAscendingDirection.Right) ? 1 : -1;
                    signMover = Mathf.Sign(A.mover.PreviousAppliedDisplacement.x);
                    T2.x = 0f;
                    T2.y = signStair * signMover;
                    A.mover.MoveThisFrame(T2, Mathf.Abs(A.mover.PreviousAppliedDisplacement.x) * (1 / Time.deltaTime), false);
                }
            }
        }
    }

}

public enum StaircaseAscendingDirection {
    Right,
    Left
}