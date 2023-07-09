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
// This is the third Mover iteration and I finally don't hate it.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    //VARIABLES
    public bool ignoreStaircases;
    private List<MoverForce> sustainedForces, forces;
    private Vector2 displacementVector, netDisplacementVector, netForwardVector;

    private Vector2 prevNetDisplacementVector, prevNetForwardvector, prevPosition;
    private Vector2 currentForward;

    private Actor actor => GetComponent<Actor>();

    private Timer hopTimer;
    private float currentForceMagnitude;
    private Vector2 T2;
    //CONSTANTS

    //EVENTS

    //METHODS

    void Awake() {
        forces = new List<MoverForce>();
        sustainedForces = new List<MoverForce>();

        if (actor == null) {
            Debug.LogWarningFormat("Mover {0} has no actor attached!", gameObject.name);
            Destroy(this);
        }
    }

    public void MoveThisFrame(Vector2 direction, float speed, bool influenceForward = true) {

        MoverForce newForce = new MoverForce {
            targetVelocity = direction.normalized * speed,
            initialVelocity = direction.normalized * speed,
            killAfterApplying = true,
            influenceActorForward = influenceForward,
        };

        newForce.stopWhenCrossing = new Vector2(99999f, 99999f);
        AddForce(newForce);
    }

    public void MoveWithVelocityUntilCrossing(Vector2 direction, Vector2 untilCrossingPosition, float speed = Global.DEFAULT_WALKING_SPEED, float forceAccelerationTime = 0f, bool influenceForward = true) {
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            targetVelocity = direction * speed,
            initialVelocity = forceAccelerationTime > 0f ? Vector2.zero : direction * speed,
            accelerationTime = forceAccelerationTime,
            stopWhenCrossing = untilCrossingPosition,
        };

        AddForce(newForce);
    }

    public void MoveWithVelocityByDistance(Vector2 direction, float distance, float speed = Global.DEFAULT_WALKING_SPEED, float forceAccelerationTime = 0f, bool influenceForward = true) {
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            goalDistance = distance,
            initialVelocity = forceAccelerationTime > 0f ? Vector2.zero : direction.normalized * speed,
            targetVelocity = direction.normalized * speed,
            accelerationTime = forceAccelerationTime,
        };

        newForce.stopWhenCrossing = new Vector2(99999f, 99999f);

        AddForce(newForce);
    }
    public void MoveByDistanceInSeconds(Vector2 direction, float distance, float forceDuration, bool influenceForward = true) {
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            targetVelocity = direction.normalized * (distance / forceDuration),
            duration = forceDuration,
        };

        newForce.stopWhenCrossing = new Vector2(99999f, 99999f);

        AddForce(newForce);
    }

    public void MoveToPositionInSeconds(Vector2 position, float forceDuration, bool influenceForward = true) {
        if (position == (Vector2)transform.position)
            return;

        MoverForce newForce = new MoverForce {
            targetVelocity = (position - (Vector2)transform.position) / forceDuration,
            influenceActorForward = influenceForward,
            duration = forceDuration,
            stopWhenCrossing = position,
        };

        AddForce(newForce);
    }

    public void MoveWithVelocityToPosition(Vector2 position, float speed = Global.DEFAULT_WALKING_SPEED, float forceAccelerationTime = 0f, bool influenceForward = true) {
        if (position == (Vector2)transform.position)
            return;

        Vector2 calculatedVelocity = (position - (Vector2)transform.position).normalized * speed;
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            stopWhenCrossing = position,
            targetVelocity = calculatedVelocity,
            initialVelocity = forceAccelerationTime > 0f ? Vector2.zero : calculatedVelocity,
            accelerationTime = forceAccelerationTime
        };

        AddForce(newForce);
    }

    public void MoveWithVelocityForSeconds(Vector2 direction, float forceDuration, float speed = Global.DEFAULT_WALKING_SPEED, float forceAccelerationTime = 0f, bool influenceForward = true) {
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            targetVelocity = direction * speed,
            duration = forceDuration,
            accelerationTime = forceAccelerationTime,
        };

        newForce.stopWhenCrossing = new Vector2(99999f, 99999f);

        AddForce(newForce);
    }

    public void MoveWithVelocityUntilCondition(Vector2 direction, Helpers.BoolDelegate stopCondition, float speed = Global.DEFAULT_WALKING_SPEED, float forceAccelerationTime = 0f, float initialVelocity = -1f, bool influenceForward = true) {
        MoverForce newForce = new MoverForce {
            influenceActorForward = influenceForward,
            targetVelocity = direction.normalized * speed,
            accelerationTime = forceAccelerationTime,
            customStopCondition = stopCondition,
        };

        newForce.stopWhenCrossing = new Vector2(99999f, 99999f);

        AddForce(newForce);
    }

    public void HopSprite(float height, float t) {
        StartCoroutine(CrHopSprite(height, t));
    }
    IEnumerator CrHopSprite(float height, float t) {
        hopTimer = new Timer();
        GameObject spriteObject;
        if (actor != null) {
            spriteObject = actor.sprite.RenderObject;
        }
        else if (GetComponent<FB_Sprite>() != null) {
            spriteObject = GetComponent<FB_Sprite>().renderer.gameObject;
        }
        else {
            spriteObject = gameObject;
        }

        while (hopTimer.Run(t)) {
            T2 = spriteObject.transform.localPosition;
            T2.y = Global.Misc.hopCurve.Evaluate(hopTimer.Percentage) * height;
            spriteObject.transform.localPosition = T2;
            yield return null;
        }

        T2 = spriteObject.transform.localPosition;
        T2.y = 0f;
        spriteObject.transform.localPosition = T2;
    }

    public void Stop() {
        forces.Clear();
    }
    public void SlowToStopInSeconds(Vector2 direction, float duration) {

    }

    public void SlowToStopAtPosition(Vector2 position, float duration) {

    }

    private void AddForce(MoverForce newForce) {
        newForce.positionWhenFirstApplied = transform.position;
        newForce.timeWhenFirstApplied = Time.time;

        forces.Add(newForce);
    }

    private void FixedUpdate() {
        if (actor.rigidBody == null)
            return;

        prevPosition = actor.Position;
        prevNetDisplacementVector = netDisplacementVector;
        prevNetForwardvector = netForwardVector;
        netDisplacementVector = Vector2.zero;
        netForwardVector = Vector2.zero;

        foreach (MoverForce F in forces) {
            ApplyForce(F);
        }

        CleanForces();

        actor.rigidBody.MovePosition(actor.Position + netDisplacementVector);

        if (netForwardVector == Vector2.zero) netForwardVector = currentForward;
        currentForward = netForwardVector.normalized;
    }

    void ApplyForce(MoverForce F) {
        bool sustainThisForce = F.killAfterApplying == false;
        Vector2 appliedVelocity = F.targetVelocity;

        if (F.TimeSinceFirstApplied < F.accelerationTime) {
            //Still accelerating to target speed, reduce applied velocity accordingly
            appliedVelocity = F.initialVelocity + ((F.targetVelocity - F.initialVelocity) * (F.TimeSinceFirstApplied / F.accelerationTime));
        }

        displacementVector = appliedVelocity * Time.fixedDeltaTime;
        Vector2 tentativePosition = actor.Position + (displacementVector);

        if (F.goalDistance > 0f
        && F.ElapsedDistance(actor.Position) + displacementVector.magnitude > F.goalDistance) {
            displacementVector = displacementVector.normalized * F.DistanceRemaining(actor.Position);
            sustainThisForce = false;
        }

        if (F.TimeExpired) {
            if (F.goalDistance > 0f) {
                displacementVector = displacementVector.normalized * F.DistanceRemaining(actor.Position);
            }
            else {
                displacementVector = displacementVector.normalized * F.TimeRemaining;
            }
            sustainThisForce = false;
        }

        if (F.stopWhenCrossing != null) {
            float signX = Mathf.Sign(tentativePosition.x - F.stopWhenCrossing.x);
            float signY = Mathf.Sign(tentativePosition.y - F.stopWhenCrossing.y);

            if (signX != F.SignX || signY != F.SignY || (signX == 0f && signY == 0f)) {
                sustainThisForce = false;

                T2 = displacementVector;
                if (signX != F.SignX) {
                    T2.x = F.stopWhenCrossing.x - actor.Position.x;
                }

                if (signY != F.SignY) {
                    T2.y = F.stopWhenCrossing.y - actor.Position.y;
                }
                displacementVector = T2;
            }
        }

        if (F.customStopCondition != null ? F.customStopCondition.Invoke() : false) {
            sustainThisForce = false;
        }

        //Apply final displacement vector
        netDisplacementVector += displacementVector;

        if (F.influenceActorForward) {
            netForwardVector += displacementVector;
        }

        if (sustainThisForce)
            sustainedForces.Add(F);
    }

    void CleanForces() {
        forces.Clear();
        for (int i = 0; i < sustainedForces.Count; ++i) {
            forces.Add(sustainedForces[i]);
        }
        sustainedForces.Clear();
    }

    //PROPERTIES
    public Vector2 Forward {
        get { return netForwardVector.normalized; }
        set { currentForward = value;
        }
    }

    public Vector2 ResistedVelocity {
        get {
            return Vector2.zero;
        }
    }

    public Vector2 PreviousDisplacement => prevNetDisplacementVector;
    public Vector2 PreviousForward => prevNetForwardvector;
    public Vector2 PreviousAppliedDisplacement => actor.Position - prevPosition;
    public bool IsMoving => netDisplacementVector != Vector2.zero || forces.Count > 0;
    public struct MoverForce {
        public Vector2 targetVelocity;
        public Vector2 initialVelocity;
        public Vector2 positionWhenFirstApplied;

        public float accelerationTime;
        public float goalDistance;
        public bool killAfterApplying;
        public bool influenceActorForward;
        public float timeWhenFirstApplied;
        public float duration;
        public Vector2 stopWhenCrossing;
        public Helpers.BoolDelegate customStopCondition;

        public float ElapsedDistance(Vector2 currentPosition) {
            return (currentPosition - positionWhenFirstApplied).magnitude;
        }

        public float DistanceRemaining(Vector2 currentPosition) {
            return goalDistance - ElapsedDistance(currentPosition);
        }

        public float Speed => targetVelocity.magnitude;
        public float TimeSinceFirstApplied => Time.time - timeWhenFirstApplied;
        public float TimeRemaining => duration - TimeSinceFirstApplied;
        public float SignX => Mathf.Sign(positionWhenFirstApplied.x - stopWhenCrossing.x);
        public float SignY => Mathf.Sign(positionWhenFirstApplied.y - stopWhenCrossing.y);
        public bool TimeExpired {
            get {
                if (duration <= 0f)
                    return false;

                return TimeSinceFirstApplied >= duration;
            }
        }

    }
}


