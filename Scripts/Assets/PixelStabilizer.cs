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

public class PixelStabilizer : MonoBehaviour {

	Vector3 t3;
    Actor actor => GetComponent<Actor>();
	public bool useOwnTransform = false;
	public bool DisableWhenMoving = true;
	private Vector2 previousPosition;

	void Awake(){
		previousPosition = transform.position;
	}

	// Update is called once per frame
	void LateUpdate () {
        //TODO: Obsolete
        bool isMoving = actor != null ? actor.mover.IsMoving : !previousPosition.Equals(transform.position);
        if (actor != null) {
            if (actor.mover.IsMoving && DisableWhenMoving) {
                //Do not stabilize if object is in motion
                return;
            }
        }

        t3.x = Mathf.Round((useOwnTransform ? transform : transform.parent).localPosition.x);
        t3.y = Mathf.Round((useOwnTransform ? transform : transform.parent).localPosition.y);
        t3.z = (useOwnTransform ? transform : transform.parent).localPosition.z;
        transform.localPosition = t3;

		previousPosition = transform.localPosition;
	}
}
