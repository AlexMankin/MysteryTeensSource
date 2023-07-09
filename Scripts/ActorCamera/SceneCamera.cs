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

public class SceneCamera : MonoBehaviour {

    //VARIABLES
    public AnimationCurve PanCurve;
    public static bool LockX, LockY;

    private static GameObject manualPositionTargetObject;
    private static SceneCamera instance;
    private static Camera cam;
    private Vector3 position;
    private Vector3 target;

    private static Timer panTimer;
    private static bool targetChangedThisFrame, targetIsPlayer;
    private static bool oobLeft, oobRight, oobTop, oobBottom;
    private static GameObject targetObject;
    private static Vector2 positionBeforeApproachingTarget, desiredPosition;
    private static bool stopAfterReachingCurrentTarget;
    private static Vector2 T2;
    private static Vector3 T3;
    //CONSTANTS
    public const float Z_POS = -100f, ACTOR_Y_OFFSET = 32f;
    //EVENTS

    //METHODS

    void Awake() {
        instance = this;
        cam = GetComponent<Camera>();

        if (manualPositionTargetObject == null) {
            manualPositionTargetObject = new GameObject("CamTarget");
            GameObject.DontDestroyOnLoad(manualPositionTargetObject);
        }

        position = transform.position;
        gameObject.AddComponent<PixelStabilizer>().useOwnTransform = true;

        panTimer = new Timer();
        SceneLoader.E_LoadedBeforeFade += onSceneLoadBeforeFade;
        Player.E_PlayerChanged += onPlayerChanged;
    }

    void OnDestroy() {
        SceneLoader.E_LoadedBeforeFade -= onSceneLoadBeforeFade;
        Player.E_PlayerChanged -= onPlayerChanged;
    }

    void LateUpdate() {
        if (targetObject == null) {
            T2 = transform.position;
        }
        else {
            T2 = targetObject.transform.position;
            T2.y += ACTOR_Y_OFFSET;
        }

        if (LockX) T2.x = transform.position.x;
        if (LockY) T2.y = transform.position.y;

        desiredPosition = constrainToMap(T2);

        if (panTimer.IsRunning) {
            //Approach the target
            position.x = Mathf.Lerp(positionBeforeApproachingTarget.x, desiredPosition.x, PanCurve.Evaluate(panTimer.Percentage));
            position.y = Mathf.Lerp(positionBeforeApproachingTarget.y, desiredPosition.y, PanCurve.Evaluate(panTimer.Percentage));
        }
        else {
            //Jump to target
            position = desiredPosition;
            if (stopAfterReachingCurrentTarget) {
                StopOnCurrentTarget();
                stopAfterReachingCurrentTarget = false;
            }
        }

        position.z = Z_POS; //2D in Unity is obnoxious. Having the camera's Z-position at >= 0 prevents it from rendering anything.
        transform.position = position;

        targetChangedThisFrame = false;
    }

    public static void Initialize() {
        cam.orthographicSize = (GlobalMiscData.WIN_RESOLUTION_Y / 4f) / GlobalMiscData.PPU;
    }

    public static void SetTarget(GameObject newTargetObject, float t = 0f, bool stopAfterReaching = false) {
        positionBeforeApproachingTarget = instance.position; //cam.transform.position;
        targetObject = newTargetObject;
        targetChangedThisFrame = true;
        panTimer.Begin(t);
        stopAfterReachingCurrentTarget = stopAfterReaching;

        if (Player.PlayableActor != null) {
            targetIsPlayer = Player.PlayableActor.gameObject == newTargetObject;
        }
    }
    public static void SetTarget(Vector2 pos, float t = 0f, bool stopAfterReaching = false) {
        manualPositionTargetObject.transform.position = pos;
        SetTarget(manualPositionTargetObject, t, stopAfterReaching);
    }
    public static void SetTarget(MonoBehaviour obj, float t = 0f, bool stopAfterReaching = false) {
        if (obj == null) {
            Debug.LogWarning("Camera target object is null or inactive!");
            return;
        }

        SetTarget(obj.gameObject, t, stopAfterReaching);
    }

    public static void StopOnCurrentTarget() {
        SetTarget((GameObject)null);
    }

    void onSceneLoadBeforeFade() {
        if (targetChangedThisFrame) {
            //An IGA might set a target manually, in which case don't want to override that change
            return;
        }

        if (Player.PlayableActor != null) {
            SetTarget(Player.PlayableActor);
        }
    }

    void onPlayerChanged(Player P) {
        if (targetIsPlayer) {
            SetTarget(P);
        }
    }

    private Vector2 constrainToMap(Vector2 V) {
        if (!canConstrain)
            return V;

        oobLeft = false;
        oobRight = false;
        oobTop = false;
        oobBottom = false;

        Vector2 diff = V - (Vector2)position;

        if (BorderLeft + diff.x < 0f)
            oobLeft = true;
        if (BorderBottom + diff.y < 0f)
            oobBottom = true;
        if (BorderRight + diff.x > Map.Width)
            oobRight = true;
        if (BorderTop + diff.y > Map.Height)
            oobTop = true;

        if (Map.Width < WidthInUnits || Map.current.freezeHorizontal) {
            oobLeft = true;
            oobRight = true;
        }

        if (Map.Height < HeightInUnits || Map.current.freezeVertical) {
            oobTop = true;
            oobBottom = true;
        }

        if (oobLeft && oobRight)
            V.x = Mathf.Max(Map.Width / 2f, WidthInUnits / 2f);
        else if (oobLeft)
            V.x = WidthInUnits / 2f;
        else if (oobRight)
            V.x = Map.Width - (WidthInUnits / 2f);


        if (oobTop && oobBottom)
            V.y = Mathf.Max(Map.Height / 2f, HeightInUnits / 2f);
        else if (oobTop)
            V.y = Map.Height - (HeightInUnits / 2f);
        else if (oobBottom)
            V.y = HeightInUnits / 2f;

        return V;

    }

    public static float BorderLeft { get { return instance.position.x - (WidthInUnits / 2f); } }
    public static float BorderRight { get { return instance.position.x + (WidthInUnits / 2f); } }
    public static float BorderBottom { get { return instance.position.y - (HeightInUnits / 2f); } }
    public static float BorderTop { get { return instance.position.y + (HeightInUnits / 2f); } }

    public static float WidthInUnits { get { return cam.orthographicSize * 2f * GlobalMiscData.GoalAspectRatio; } }
    public static float HeightInUnits { get { return cam.orthographicSize * 2f; } }

    public static Camera Cam { get { return cam; } }
    public static bool TargetIsPlayer { get { return targetIsPlayer; } }
    public static bool IsMoving { get { return panTimer.IsRunning; } }
    private static bool canConstrain {
        get {
            return true;
        }
    }
}
