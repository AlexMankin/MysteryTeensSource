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

public class Ghost : MonoBehaviour {

    public PersonData person;
    public CardinalDirection forward;

    //VARIABLES
    private static Dictionary<AllPersons, Ghost[]> referencesInScene;

    //CONSTANTS
    const int REF_CAPACITY = 32;

    //EVENTS
    private static Helpers.EventVoid e_RegisterPositionsInScene;

    //METHODS

    void Awake() {
        e_RegisterPositionsInScene += onRegisterPositions;
        GameObject.Destroy(spriteRenderer);
    }

    private void OnDestroy() {
        e_RegisterPositionsInScene -= onRegisterPositions;
    }


    public void OnValidate() {
        if (person == null) {
            return;
        }

        if (person.animations.idleDown == null)
            return;

        AnimData anim = person.animations.idleDown;

        spriteRenderer.sprite = anim.FrameList[0].Sprite;
        spriteRenderer.sortingLayerName = SpriteLayer.UI.ToString();

        spriteRenderer.hideFlags = HideFlags.HideInInspector;

    }

    private void OnDrawGizmos() {
        ColorAdjustment ca = new ColorAdjustment(spriteRenderer, this);
        ca.AdjustAlpha(0.4f);
        transform.localScale = Vector3.one * 0.5f;
        if (person != null) ca.AdjustColor(person.colorMid);
    }
    private void OnDrawGizmosSelected() {
        if (person != null)
            Gizmos.color = person.colorMid;

        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + SceneCamera.ACTOR_Y_OFFSET, 0f),
            new Vector3(960 / GlobalMiscData.PPU / 2f, 540 / GlobalMiscData.PPU / 2f, 0f));

        ColorAdjustment ca = new ColorAdjustment(spriteRenderer, this);
        ca.AdjustAlpha(0.6f);
        transform.localScale = Vector3.one;
    }
    public static void Register() {
        foreach (AllPersons A in System.Enum.GetValues(typeof(AllPersons))) {
            for (int i = 0; i < List[A].Length; ++i)
                List[A][i] = null;
        }
        e_RegisterPositionsInScene?.Invoke();
    }

    private void onRegisterPositions() {
        if (person == null)
            return;

        int index = Helpers.NumberFromGameObjectName(gameObject.name);
        if (index < 0 || index >= REF_CAPACITY)
            index = 0;

        referencesInScene[person.personName][index] = this;
    }

    public static Ghost Get(AllPersons A, int index = 0) {
        if (index < 0 || index >= REF_CAPACITY) {
            Debug.LogWarning("Index for reference position " + A + index + " out of range!");
            return null;
        }

        if (referencesInScene[A][index] == null) {
            Debug.LogWarning("No reference position found for " + A + index);
            return null;
        }

        return referencesInScene[A][index];
    }

    public static Vector2 GetPosition(AllPersons A, int index = 0) {
        if (Get(A, index) != null)
            return Get(A, index).Position;

        return Vector2.zero;
    }
    public static Vector2 GetPosition(Actor A, int index = 0) {
        if (A.person == null)
            return Vector2.zero;

        return GetPosition(A.person.personName, index);
    }

    //PROPERTIES
    SpriteRenderer spriteRenderer {
        get {
            SpriteRenderer ret = GetComponent<SpriteRenderer>();
            if (ret == null)
                ret = gameObject.AddComponent<SpriteRenderer>();

            return ret;
        }
    }

    public Vector2 Position => transform.position;
    public static Dictionary<AllPersons, Ghost[]> List {
        get {
            if (referencesInScene == null) {
                referencesInScene = new Dictionary<AllPersons, Ghost[]>();
                foreach (AllPersons A in System.Enum.GetValues(typeof(AllPersons))) {
                    referencesInScene[A] = new Ghost[REF_CAPACITY];
                }
            }

            return referencesInScene;
        }
    }
}
