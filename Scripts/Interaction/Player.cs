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

public class Player : MonoBehaviour {

    //VARIABLES
    public static Actor PlayableActor;
    private Actor actor => GetComponent<Actor>();

    public static bool isFrozen, isMeowing;
    private static Vector2 walkDirectionVector, prevWalkVector;
    private static float timeOfLastMovement;

    public static ActionBaseClass ActiveInspectedAction { get; private set; }
    private static ActionBaseClass ActionHighlightedOnMeow;

    private static Hotspot currentHotspot, candidateHotspot;
    private Vector2 raySource;
    private RaycastHit2D[] rayHits;
    private static int inspectableLayerInt;

    //CONSTANTS
    private const float INSPECT_DISTANCE = 2f;
    private const float DURATION_IDLE_UI = 2f;
    private const float PULL_VELOCITY = 5f;
    private const float SPRINT_SPEED_MULTIPLIER = 1.5f;

    //EVENTS
    public delegate void EventPlayer(Player player);
    public static EventPlayer E_PlayerChanged;

    //METHODS

    void Awake() {

        CanSprint = true;

        if (GetComponent<Actor>() == null) {
            Debug.LogWarning("Could not make " + gameObject.name + " player because no attached Actor was found.");
            return;
        }

        E_PlayerChanged?.Invoke(this);
        E_PlayerChanged += onPlayerChanged;

        PlayableActor = GetComponent<Actor>();
        Debug.Log("Changed actor to " + PlayableActor);
    }

    void OnDestroy() {
        E_PlayerChanged -= onPlayerChanged;
    }

    void Update() {

        if (!CanControl) {
            timeOfLastMovement = Time.time;
            ActiveInspectedAction = null;
            return;
        }

        if (walkDirectionVector != Vector2.zero) {
            timeOfLastMovement = Time.time;
        }

        PlayableActor.LockDirection = false;

        //Determine inspected object
        prevWalkVector = walkDirectionVector;
        walkDirectionVector = Vector2.zero;

        if (FB_Input.GetButton(FB_Button.Up)) {
            walkDirectionVector += Vector2.up;
        }
        if (FB_Input.GetButton(FB_Button.Right)) {
            walkDirectionVector += Vector2.right;
        }

        if (FB_Input.GetButton(FB_Button.Left)) {
            walkDirectionVector += Vector2.left;
        }

        if (FB_Input.GetButton(FB_Button.Down)) {
            walkDirectionVector += Vector2.down;
        }

        if (walkDirectionVector != Vector2.zero) {
            PlayableActor.mover.MoveThisFrame(walkDirectionVector, Speed);
        }

        if (FB_Input.ButtonDown(FB_Button.Confirm)) {
            Inspector.HighlightedAction?.OnInspect();
        }

        if (FB_Input.ButtonDown(FB_Button.Meow)) {
            if (CanMeow) {
                ActionHighlightedOnMeow = Inspector.HighlightedAction;
                StartCoroutine(meow());
            }
        }
    }

    IEnumerator meow() {
        bool currentLockAnimation = actor.LockAnimation;
        AnimData currentAnimation = actor.sprite.CurrentAnimation;

        SFX selectedMeow = SFX.Meow1;
        switch (Random.Range(1, 6)) {
            case 1:
                selectedMeow = SFX.Meow1;
                break;
            case 2:
                selectedMeow = SFX.Meow2;
                break;
            case 3:
                selectedMeow = SFX.Meow3;
                break;
            case 4:
                selectedMeow = SFX.Meow4;
                break;
            case 5:
                selectedMeow = SFX.Meow5;
                break;
        }

        FB_Audio.PlaySFX(selectedMeow);
        isMeowing = true;
        actor.SetAnimation(Global.Animations.cat.meow);
        yield return new WaitForSeconds(1f);

        isMeowing = false;
        actor.LockAnimation = false;

        if (currentLockAnimation)
            actor.SetAnimation(currentAnimation);
        else
            actor.LockAnimation = false;

        ActionHighlightedOnMeow?.OnMeow();
    }

    void onPlayerChanged(Player newPlayer) {
        Destroy(this);
    }

    //PROPERTIES
    public static bool CanControl {
        get {
            if (IGA.IsRunning)
                return false;

            if (PlayableActor == null)
                return false;

            if (SceneLoader.IsLoading)
                return false;

            if (isFrozen)
                return false;

            if (isMeowing)
                return false;

            return true;
        }
    }

    public static bool CanSprint { get; set; }

    public static bool CanMeow => !sv.Data.Sys_DisableMeow;

    public static bool CanSummonIdleUI {
        get {
            return Time.time - timeOfLastMovement >= DURATION_IDLE_UI;
        }
    }

    private float Speed {
        get {
            if (!CanSprint)
                return PlayableActor.speed;

            return FB_Input.GetButton(FB_Button.Sprint) ? PlayableActor.speed * SPRINT_SPEED_MULTIPLIER : PlayableActor.speed;
        }
    }
}
