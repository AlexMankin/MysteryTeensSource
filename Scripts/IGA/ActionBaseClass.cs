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
using System.Reflection; //To send Type to SaveData

public class ActionBaseClass : MonoBehaviour {

    public TextData Txt, Txt2, Txt3;
    public TextData[] AdditionalText;
    public FlagZeroBehavior FlagZeroFunction;
    public bool FirstFlagActivation;

    public bool collideToInspect;
    public InspectIconType inspectionIcon = InspectIconType.Question;
    protected bool prevCanDisplay = true;

    protected delegate void eventActionFlag(System.Type type, bool newFlagValue, int flagIndex);
    protected delegate void eventActionPhase(System.Type type, int newValue);
    protected static eventActionFlag e_FlagSet;
    protected static eventActionPhase e_PhaseSet;
    protected bool inspected;

    //CONSTANTS
    public const int NUM_FLAGS = 5;

    //FUNCTIONS
    protected virtual void Awake() {
        SceneLoader.E_LoadedBeforeFade += WakeBeforeFade;
        SceneLoader.E_LoadedAfterFade += WakeAfterFade;
        SceneLoader.E_ReadyToLoadAfterFade += BeforeSceneChange;
        IGA.E_ActionEnd += OnControlReturn;

        if (InitializeGame.IsInitialized && FirstFlagActivation && !GetFlag())
            actor.Off();

        if (InitializeGame.IsInitialized) {
            if (FlagZeroFunction == FlagZeroBehavior.OffSwitch && GetFlag(0)) {
                actor.Off();
            }
            else if (FlagZeroFunction == FlagZeroBehavior.OnSwitch && !GetFlag(0)) {
                actor.Off();
            }
        }

        e_FlagSet += onFlagChange;
        e_PhaseSet += onPhaseChange;
    }

    protected virtual void Start() {
        for (int i = 0; i < NUM_FLAGS; ++i) {
            e_FlagSet?.Invoke(this.GetType(), GetFlag(i), i);
        }
    }

    protected virtual void Update() {

        if (prevCanDisplay != CanDisplay) {
            foreach (Behaviour comp in gameObject.GetComponents<Behaviour>()) {
                comp.enabled = CanDisplay;
            }

            foreach (Transform child in transform) {
                child.gameObject.SetActive(CanDisplay);
            }
        }

        if (CanCollide) {
            foreach (Actor A in ActorRegistrar.AllActorsInScene) {
                if (!A.IgnoreCollision) {
                    if (collider.OverlapPoint(A.transform.position)) {
                        OnCollide(A);
                        if (A.IsPlayer) {
                            OnCollidePlayer();
                        }
                        if (A == Inspector.Instance?.GetComponent<Actor>()) {

                        }
                    }
                }
            }
        }

        this.enabled = true;
        prevCanDisplay = CanDisplay;
    }

    protected virtual void OnDestroy() {
        SceneLoader.E_LoadedBeforeFade -= WakeBeforeFade;
        SceneLoader.E_LoadedAfterFade -= WakeAfterFade;
        IGA.E_ActionEnd -= OnControlReturn;
        e_FlagSet -= onFlagChange;
        e_PhaseSet -= onPhaseChange;

        StopAllCoroutines();
    }

    public virtual void WakeBeforeFade() { }
    public virtual void WakeAfterFade() { }
    protected virtual void OnFlag0(bool newValue) { }
    protected virtual void OnFlag1(bool newValue) { }
    protected virtual void OnFlag2(bool newValue) { }
    protected virtual void OnFlag3(bool newValue) { }
    protected virtual void OnFlag4(bool newValue) { }
    protected virtual void OnPhaseChange(int newValue) { }
    public virtual void OnControlReturn() { }
    public virtual void OnCollide(Actor collidingActor) { }
    public virtual void OnCollidePlayer() { }
    public virtual void OnInspect() { }
    public virtual void OnMeow() { }
    public virtual void BeforeSceneChange() { }


    public void SetFlag(bool val = true, int flagIndex = 0, System.Type actorType = null) {
        if (flagIndex < 0 || flagIndex >= ActionVariablePackage.NUM_FLAGS) {
            Debug.LogWarning(IndexErrorString);
            return;
        }

        actorType = actorType == null ? this.GetType() : actorType;

        ActionVariablePackage avp = sv.Data.GetPackage(actorType);
        if (avp.Flags == null)
            avp.Flags = new bool[ActionVariablePackage.NUM_FLAGS];
        avp.Flags[flagIndex] = val;
        sv.Data.UpdatePackage(actorType, avp);

        e_FlagSet?.Invoke(this.GetType(), val, flagIndex);
    }
    public void SetFlag<T>(bool val = true, int flagIndex = 0) {
        SetFlag(val, flagIndex, typeof(T));
    }
    public void SetFlag(int flagIndex) {
        SetFlag(true, flagIndex);
    }
    public void SetFlag<T>(int flagIndex) {
        SetFlag(true, flagIndex, typeof(T));
    }

    public bool GetFlag(int flagIndex = 0, System.Type actorType = null) {
        if (flagIndex < 0 || flagIndex > ActionVariablePackage.NUM_FLAGS) {
            Debug.LogWarning(IndexErrorString);
            return false;
        }

        actorType = actorType == null ? this.GetType() : actorType;

        return sv.Data.GetPackage(actorType).Flags[flagIndex];
    }
    public bool GetFlag<T>(int flagIndex = 0) {
        return GetFlag(flagIndex, typeof(T));
    }

    /// <summary>
    /// Returns true if it's able to successfully flip the flag from false to true.
    /// </summary>
    public bool UseFlag(int flagIndex = 0, System.Type actorType = null) {
        actorType = actorType == null ? this.GetType() : actorType;

        if (GetFlag(flagIndex, actorType))
            return false;

        SetFlag(true, flagIndex, actorType);

        return true;
    }
    /// <summary>
    /// Type parameter allows you to set save data values for other actions (use responsibly!!!)
    /// </summary>
    public bool UseFlag<T>(int flagIndex = 0) {
        return UseFlag(flagIndex, typeof(T));
    }

    /// <summary>
    /// An integer value uniquely reserved in sava data for use by and/or associated with this action type.
    /// </summary>
    public void SetPhase(int val, System.Type actorType = null) {
        actorType = actorType == null ? this.GetType() : actorType;

        ActionVariablePackage avp = sv.Data.GetPackage(actorType);
        avp.Phase = val;
        sv.Data.UpdatePackage(actorType, avp);
    }
    /// <summary>
    /// Type parameter allows you to set save data values for other actions (use responsibly!!!)
    /// </summary>
    public void SetPhase<T>(int val) {
        SetPhase(val, typeof(T));
    }

    /// <summary>
    /// An integer value uniquely reserved in sava data for use by and/or associated with this action type.
    /// </summary>
    public int GetPhase(System.Type actorType = null) {
        actorType = actorType == null ? this.GetType() : actorType;

        return sv.Data.GetPackage(actorType).Phase;
    }
    /// <summary>
    /// Type parameter allows you to set save data values for other actions (use responsibly!!!)
    /// </summary>
    public int GetPhase<T>() {
        return GetPhase(typeof(T));
    }

    public delegate void FlagHandler(bool newValue);

    protected void onFlagChange(System.Type actionType, bool newValue, int flagNo) {
        if (actionType == this.GetType()) {
            if (flagNo == 0)
                OnFlag0(newValue);
            if (flagNo == 1)
                OnFlag1(newValue);
            if (flagNo == 2)
                OnFlag2(newValue);
            if (flagNo == 3)
                OnFlag3(newValue);
            if (flagNo == 4)
                OnFlag4(newValue);

        }
    }

    protected void onPhaseChange(System.Type actionType, int newValue) {
        if (actionType == this.GetType()) {
            OnPhaseChange(newValue);
        }
    }




    public static string IndexErrorString { get { return MethodBase.GetCurrentMethod().DeclaringType + ": Flag index out of range."; } }
    public virtual bool CanDisplay { get { return true; } }

    protected Actor actor => GetComponent<Actor>();
    protected Collider2D collider { get { return actor != null ? actor.collider : GetComponent<Collider2D>(); } }
    public virtual bool CanCollide {
        get {

            if (IGA.IsRunning)
                return false;

            if (collider == null)
                return false;

            if (SceneLoader.IsLoading)
                return false;

            if (!Player.CanControl)
                return false;

            if (ActorRegistrar.AllActorsInScene == null)
                return false;

            return true;
        }
    }

    public int Phase {
        get { return GetPhase(); }
        set { SetPhase(value); }
    }

    //Persons
    protected Actor Cat => ActorRegistrar.GetPerson(AllPersons.Cat);
    protected Actor Duke => ActorRegistrar.GetPerson(AllPersons.Duke);
    protected Actor Sunbeam => ActorRegistrar.GetPerson(AllPersons.Sunbeam);
    protected Actor Larold => ActorRegistrar.GetPerson(AllPersons.Larold);
    protected Actor Mugsy => ActorRegistrar.GetPerson(AllPersons.Mugsy);
    protected Actor Valerie => ActorRegistrar.GetPerson(AllPersons.Valerie);
    protected Actor Glimmer => ActorRegistrar.GetPerson(AllPersons.Glimmer);

}

public enum FlagZeroBehavior {
    Normal = 0,
    OnSwitch = 1,
    OffSwitch = 2
}

