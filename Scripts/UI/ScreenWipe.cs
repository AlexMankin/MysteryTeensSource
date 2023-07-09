using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWipe : MonoBehaviour {

    //VARIABLES
    public Actor screenActor, wipeActor;
    private static Timer wipeTimer;
    private static ScreenWipe instance;
    private static Vector3 t3;
    //CONSTANTS
    private const float OFF_POSITION_HORIZONTAL = 250f,
        DISTANCE_HORIZONTAL = 650f, 
        OFF_POSITION_VERTICAL = 150f, 
        DISTANCE_VERTICAL = 500f;

    private const float WIPE_DURATION = 0.45f;

    //EVENTS

    //METHODS

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        wipeTimer = new Timer();
    }
    void Start(){
        wipeActor.sprite.Alpha = 0f;
        screenActor.sprite.Alpha = 0f;
	}

    public static IEnumerator Wipe(CardinalDirection enterFromDirection, WipeType type = WipeType.Cloudy, bool reverseDirection = false, SpriteLayer layer = SpriteLayer.ScreenWipe, Color? screenColor = null) {
        if (enterFromDirection == CardinalDirection._NULL)
            enterFromDirection = CardinalDirection.Right;

        Sprite wipeSprite = SpriteFromType(type);
        Vector3 rotationVector = RotationFromDirection(enterFromDirection);
        WipePackage wipePositions = GetWipePositions(enterFromDirection, reverseDirection);

        instance.screenActor.sprite.Alpha = 0f;
        instance.screenActor.sprite.Adjuster.AdjustColor(screenColor.HasValue ? screenColor.Value : Color.black);
        instance.screenActor.sprite.Layer = layer;
        instance.wipeActor.SetAnimation(wipeSprite);
        instance.wipeActor.sprite.Alpha = 1f;
        instance.wipeActor.LocalPosition = wipePositions.startPosition;
        instance.wipeActor.transform.localEulerAngles = rotationVector;
        instance.wipeActor.sprite.Layer = layer;

        wipeTimer = new Timer();
        while (wipeTimer.Run(WIPE_DURATION, true)){
            t3 = Vector3.Lerp(wipePositions.startPosition, wipePositions.endPosition, wipeTimer.Percentage);
            instance.wipeActor.LocalPosition = t3;
            yield return null;
        }

        instance.wipeActor.LocalPosition = wipePositions.endPosition;
        instance.screenActor.sprite.Alpha = reverseDirection ? 0f : 1f;
        instance.wipeActor.sprite.Alpha = 0f;

        yield break;


    }

    public static IEnumerator UnWipe(CardinalDirection exitToDirection, WipeType type = WipeType.Cloudy, bool reverseDirection = true) {
        yield return Wipe(exitToDirection, type, reverseDirection);
    }

    static Sprite SpriteFromType(WipeType type) {
        if (type == WipeType.Woodland)
            return Global.Animations.wipeWood;

        if (type == WipeType.Cloudy)
            return Global.Animations.wipeCloudy;

        return Global.Animations.wipeScoffy;
    }

    static Vector3 RotationFromDirection(CardinalDirection direction) {
        Vector3 ret = Vector3.zero;
        
        if (direction == CardinalDirection.Up)
            ret.z = 90f;
        else if (direction == CardinalDirection.Left)
            ret.z = 180f;
        else if (direction == CardinalDirection.Down)
            ret.z = 270f;

        return ret;
    }

    static WipePackage GetWipePositions(CardinalDirection direction, bool startActive) {
        WipePackage ret = new WipePackage();
        t3 = Vector3.zero;
        t3.z = 10f;
        if (direction == CardinalDirection.Up) {
            t3.y = startActive ? OFF_POSITION_VERTICAL - DISTANCE_VERTICAL : OFF_POSITION_VERTICAL;
            ret.startPosition = t3;
            t3.y = startActive ? OFF_POSITION_VERTICAL : OFF_POSITION_VERTICAL - DISTANCE_VERTICAL;
            ret.endPosition = t3;
        }
        else if (direction == CardinalDirection.Down) {
            t3.y = startActive ? -OFF_POSITION_VERTICAL + DISTANCE_VERTICAL : -OFF_POSITION_VERTICAL;
            ret.startPosition = t3;
            t3.y = startActive ? -OFF_POSITION_VERTICAL : -OFF_POSITION_VERTICAL + DISTANCE_VERTICAL;
            ret.endPosition = t3;
        }
        else if (direction == CardinalDirection.Right) {
            t3.x = startActive ? OFF_POSITION_HORIZONTAL - DISTANCE_HORIZONTAL : OFF_POSITION_HORIZONTAL;
            ret.startPosition = t3;
            t3.x = startActive ? OFF_POSITION_HORIZONTAL : OFF_POSITION_HORIZONTAL - DISTANCE_HORIZONTAL;
            ret.endPosition = t3;
        }
        else if (direction == CardinalDirection.Left) {
            t3.x = startActive ? -OFF_POSITION_HORIZONTAL + DISTANCE_HORIZONTAL : -OFF_POSITION_HORIZONTAL;
            ret.startPosition = t3;
            t3.x = startActive ? -OFF_POSITION_HORIZONTAL : -OFF_POSITION_HORIZONTAL + DISTANCE_HORIZONTAL;
            ret.endPosition = t3;
        }

        return ret;
    }

    //PROPERTIES
    public static bool IsWiping => wipeTimer.IsRunning;
}

public struct WipePackage {
    public Vector3 startPosition;
    public Vector3 endPosition;
}

public enum WipeType {
    _NULL,
    Woodland = 0,
    Scoffy = 1,
    Cloudy = 2
}