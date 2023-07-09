using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectIcon : MonoBehaviour {

    //VARIABLES
    public static InspectIcon Instance { get; private set; }
    private Transform specialActorParent;
    private Actor actor => GetComponent<Actor>();
	//CONSTANTS
	
	//EVENTS
	
	//METHODS
	
	void Awake(){
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SceneLoader.E_ReadyToLoadAfterFade += onBeforeSceneChange;
        Player.E_PlayerChanged += onPlayerChange;
        specialActorParent = transform.parent;
    }

    private void OnDestroy() {
        SceneLoader.E_ReadyToLoadAfterFade -= onBeforeSceneChange;
        Player.E_PlayerChanged -= onPlayerChange;
    }

    private void Update() {
        if (Inspector.HighlightedAction == null) {
            actor.SetAnimation(null);
            return;
        }

        if (Inspector.HighlightedAction.inspectionIcon != InspectIconType._NULL) {
            if (Inspector.HighlightedAction.inspectionIcon == InspectIconType.Question)
                actor.SetAnimation(Global.Animations.inspectQuestion);

            if (Inspector.HighlightedAction.inspectionIcon == InspectIconType.Exclamation)
                actor.SetAnimation(Global.Animations.inspectExclamation);

            if (Inspector.HighlightedAction.inspectionIcon == InspectIconType.Convo)
                actor.SetAnimation(Global.Animations.inspectConvo);

            return;
        }
    }

    void onBeforeSceneChange() {
        returnToParentObject();
    }

    void returnToParentObject() {
        transform.SetParent(specialActorParent);
        transform.localPosition = Vector2.zero;
        actor.SetAnimation(null);
    }
    void onPlayerChange(Player newPlayer) {
        if (newPlayer == null) {
            returnToParentObject();
            return;
        }

        transform.SetParent(newPlayer.transform);
        transform.localPosition = new Vector2(0f, 40f);
    }

    //PROPERTIES
}

public enum InspectIconType {
    _NULL = 0,
    Question,
    Convo,
    Exclamation,
    NoIcon
}