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
using System;

public static class ActorRegistrar {

    //VARIABLES
    private static List<Actor> allActorsInScene;

    //CONSTANTS

    //EVENTS
    public static Helpers.EventVoid E_CallForRegistration;

    //METHODS
    public static void Initialize() {
        SceneLoader.E_ReadyToLoadAfterFade += onChangeScene;
        allActorsInScene = new List<Actor>();
    }

    public static void CallForRegistration() {
        if (!InitializeGame.IsInitialized)
            return;

        allActorsInScene.Clear();

        E_CallForRegistration?.Invoke();

        AssignPlayer(sv.Data.CurrentPlayer);
    }

    public static void Register(Actor A, PersonData D) {
        allActorsInScene.Add(A);

        if (D == null)
            return;

        Enum.TryParse(D.name, out AllPersons actorEnum);

        if (actorEnum == AllPersons._NULL) {
            Debug.LogWarning("Could not find Enum value for actor in scene: " + D.name + ".  Run Futurebound > Refresh Actors and try again!");
            return;
        }
    }

    public static Actor GetPerson(AllPersons A) {
        if (A == AllPersons._NULL)
            return null;

        foreach (Actor AS in allActorsInScene) {
            if (AS.person != null) {
                if (A == AS.person.personName)
                    return AS;
            }
        }

        return null;

    }

    public static void AssignPlayer(AllPersons A) {
        if (A == AllPersons._NULL)
            return;

        if (SceneLoader.CurrentScene == null)
            return;

        if (SceneLoader.CurrentScene == 0)
            return;

        Actor actorInScene = GetPerson(A);

        if (actorInScene == null) {
            Debug.LogWarning("Could not assign player to " + A + " because no actor could be found in the scene.  \nCurrent Scene: " + sv.Data.currentSceneIndex);
            return;
        }

        actorInScene.IsPlayer = true;
    }

    static void onChangeScene() {
        allActorsInScene.Clear();
    }

    //PROPERTIES
    public static List<Actor> AllActorsInScene { get { return allActorsInScene; } }
}
