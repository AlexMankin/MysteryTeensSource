using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteImitator : MonoBehaviour {

	//VARIABLES
	public Actor copyingActor;
	public bool invertX, invertY;
	public FB_Sprite copyingSprite;
	public SpriteLayer layer;
	private FB_Sprite sprite;
	//CONSTANTS
	
	//EVENTS
	
	//METHODS
	
	void Awake(){
		sprite = gameObject.AddComponent<FB_Sprite> ();
		sprite.Layer = layer;
	}

	void Start(){
		if (copyingSprite == null) {
			if (copyingActor != null) {
				copyingSprite = copyingActor.sprite;
			}
		}
	}

	void Update(){
		if (copyingSprite != null) {
			sprite.SetAnimation (copyingSprite.CurrentAnimation);
			sprite.FlipX = invertX ? !copyingSprite.FlipX : copyingSprite.FlipX;
			sprite.FlipY = invertY ? !copyingSprite.FlipY : copyingSprite.FlipY;
		}
	}

}
