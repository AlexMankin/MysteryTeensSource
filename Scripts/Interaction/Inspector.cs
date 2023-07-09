using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector : MonoBehaviour {

    //VARIABLES
    public static Inspector Instance {get; private set; }
    public static ActionBaseClass HighlightedAction { get; private set; }
    private Actor actor => GetComponent<Actor>();

    private List<Collider2D> collisionsWithPlayer, collisionsWithInspectionBox;
    private ContactFilter2D filter;
    private BoxCollider2D box;

    private Vector2 t2;
    //CONSTANTS
    private const float DISPLACEMENT_FROM_PLAYER = 30f;
    private const float BOX_SIZE_SHORT = 16f, BOX_SIZE_LONG = 28f;
    //EVENTS

    //METHODS

    void Awake() {
        if (Instance != null) { 
            Destroy(gameObject);
            return;
        }

        Instance = this;
        collisionsWithPlayer = new List<Collider2D>();
        collisionsWithInspectionBox = new List<Collider2D>();
        filter = new ContactFilter2D();
        filter.NoFilter();

        box = actor.collider as BoxCollider2D;
        t2 = new Vector2();
    }

    private void Update() {
        HighlightedAction = null;

        if (!CanInspect)
            return;

        actor.Position = Player.PlayableActor.Position;
        Vector2 playerForward = Player.PlayableActor.ForwardVector;
        t2.x = Mathf.Abs(playerForward.x) > 0f ? BOX_SIZE_LONG : BOX_SIZE_SHORT;
        t2.y = Mathf.Abs(playerForward.y) > 0f ? BOX_SIZE_LONG : BOX_SIZE_SHORT;
        box.size = t2;

        t2.x = t2.x / 2f * playerForward.x;
        t2.y = t2.y / 2f * playerForward.y;
        box.offset = t2;

        //Look for touch collisions first.
        Player.PlayableActor?.collider?.OverlapCollider(filter,  collisionsWithPlayer);
        for (int i = 0; i < collisionsWithPlayer.Count; ++i) {
            ActionBaseClass action = collisionsWithPlayer[i].GetComponent<ActionBaseClass>();
            if (action == null)
                continue;

            if (action.collideToInspect && action.gameObject.activeSelf) {
                HighlightedAction = action;
                return;
            }
        }

        //Look for inspection box collisions
        actor.collider.OverlapCollider(filter, collisionsWithInspectionBox);
        for (int i = 0; i < collisionsWithInspectionBox.Count; ++i) {
            ActionBaseClass action = collisionsWithInspectionBox[i].GetComponent<ActionBaseClass>();
            if (action == null)
                continue;

            if (action.collideToInspect)
                continue;

            if (action.gameObject.activeSelf) {
                if (HighlightedAction == null) {
                    HighlightedAction = action;
                    continue;
                }

                //We have a contest! Select the object that's closer to the player from their forward direction.
                if (action.inspectionIcon != InspectIconType._NULL && HighlightedAction.inspectionIcon == InspectIconType._NULL) {
                    HighlightedAction = action;
                }
                else if (Vector2.Distance(Player.PlayableActor.Position, action.transform.position) >=
                    Vector2.Distance(Player.PlayableActor.Position, HighlightedAction.transform.position)) {
                    HighlightedAction = action;
                }
            }
        }
    }



	//PROPERTIES
    public static bool CanInspect {
        get {
            if (!Player.CanControl)
                return false;

            return true;
        }
    }
}
