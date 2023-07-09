using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAnimator : MonoBehaviour {

    //VARIABLES
    public bool LockAnimation, LockDirection;

    bool xFacing, yFacing;
    //CONSTANTS

    //EVENTS

    //METHODS

    void Awake() {
        if (actor == null) {
            Debug.LogWarning("No actor attached to animator.", gameObject);
            GameObject.Destroy(this);
            return;
        }
    }

    void Update() {
        

        if (actor.person == null || actor.mover == null || actor.sprite == null)
            return;

        if (actor.mover.Forward.x != 0f)
            xFacing = actor.mover.Forward.x > 0f;

        yFacing = actor.mover.Forward.y > 0f;

        AnimData goalAnimation = null;

        if (actor.person.animations.idleDown != null) {
            if (actor.mover.IsMoving) {
                if (yFacing)
                    goalAnimation = actor.person.animations.walkUp;
                else
                    goalAnimation = actor.person.animations.walkDown;
            }
            else {
                if (yFacing)
                    goalAnimation = actor.person.animations.idleUp;
                else
                    goalAnimation = actor.person.animations.idleDown;
            }

            if (!LockAnimation)
                actor.sprite.SetAnimation(goalAnimation);
        }

        if (!LockDirection)
            actor.sprite.FlipX = xFacing;
    }

    //PROPERTIES
    public Actor actor => GetComponent<Actor>();
}

[System.Serializable]
public struct PersonAnimationPackage {
    public AnimData walkDown, walkUp, idleDown, idleUp;
}