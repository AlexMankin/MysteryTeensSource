using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	//VARIABLES
	public BGM defaultBGM;
	public bool freezeHorizontal, freezeVertical;

	public static Map current;
    public static SpriteRenderer sprite => current?.GetComponent<SpriteRenderer>();

    public static float Width => sprite != null ? sprite.bounds.max.x : 0f;
    public static float Height => sprite != null ? sprite.bounds.max.y : 0f;

    //CONSTANTS

    //EVENTS

    //METHODS
    void Awake(){
		current = this;
	}

    private void OnDestroy() {
        current = null;
    }
}
