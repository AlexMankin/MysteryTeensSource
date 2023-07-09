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

[System.Serializable]
[CreateAssetMenu(fileName = "GlobalAnimations", menuName = "Mystery Teens/Global/Animations")]
public class GlobalAnimationData : ScriptableObject {

    //VARIABLES
    public AnimData emoteEllipsis, emoteSurprise, emoteGrumble, emoteQuestion, emoteLove;
    public GlobalCatAnimations cat;
    public AnimData inspectQuestion, inspectConvo, inspectExclamation;
    public AnimData doorOpen, doorClosed;
    public AnimData sunnyPetCat;

    [Header("Screen Wipes")]
    public Sprite wipeWood;
    public Sprite wipeScoffy;
    public Sprite wipeCloudy;

    //CONSTANTS

    //EVENTS

    //METHODS


    //PROPERTIES
}
