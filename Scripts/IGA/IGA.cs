//
//    `````               Code founded by Alex Mankin                       
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

public class IGA : MonoBehaviour {

    //VARIABLES
    private static Coroutine currentAction;
    private static IEnumerator nextAction;

    private static IGA instance;
    //CONSTANTS

    //EVENTS
    public static Helpers.EventVoid E_ActionBegin, E_ActionEnd;

    //METHODS

    void Awake() {
        instance = this;
    }

    public static void Run(IEnumerator newRoutine) {
        if (newRoutine == null) {
            return;
        }

        if (currentAction != null) {
            Debug.LogWarning("Cannot start IGA, " + newRoutine + ", while another is already running!");
            return;
        }

        if (E_ActionBegin != null)
            E_ActionBegin();

        currentAction = instance.StartCoroutine(crRun(newRoutine));
    }

    public static void Queue(IEnumerator newRoutine) {
        if (newRoutine == null) {
            return;
        }

        if (nextAction != null) {
            Debug.LogWarning("Cannot queue IGA " + newRoutine + ", another IGA is already queued!");
            return;
        }

        if (currentAction == null) {
            Run(newRoutine);
        }
        else {
            nextAction = newRoutine;
        }
    }

    private static IEnumerator crRun(IEnumerator currentRoutine) {
        yield return currentRoutine;
        endRoutine();
    }

    private static void endRoutine() {
        currentAction = null;
        if (nextAction != null) {
            IEnumerator nxt = nextAction;
            nextAction = null;
            Run(nxt);
        }
        else {
            if (E_ActionEnd != null)
                E_ActionEnd();
        }
    }

    public static bool IsRunning {
        get {
            return currentAction != null || nextAction != null;
        }
    }
}

public interface IGameAction {
    void OnControlReturn();
    void OnCollide(Actor otherActor);
    void OnInspect();
    bool CanDisplay { get; }
    bool CanCollide { get; }
}