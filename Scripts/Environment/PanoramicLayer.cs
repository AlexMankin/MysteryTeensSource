using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramicLayer : MonoBehaviour {

	//VARIABLES
	public bool ScrollHorizontal, ScrollVertical;

	private float layerScale, xInit, yInit, xPos, yPos;

	private Vector3 T3;

	//CONSTANTS
	private const float BASE_Z_POS = 0f,
						Z_INCR = 100f;

	//METHODS
	void Awake(){
		layerScale = Mathf.Pow (2, (BASE_Z_POS - transform.position.z) / Z_INCR);
		xInit = transform.position.x;
		yInit = transform.position.y;
	}

	void LateUpdate(){
		T3 = SceneCamera.Cam.transform.position;

		float F = SceneCamera.BorderLeft;

		if (ScrollHorizontal) {
			xPos = F + xInit - (F * layerScale);
		}

		F = SceneCamera.BorderBottom;
		if (ScrollVertical) {
			yPos = F + yInit - (F * layerScale);
		}

		T3 = transform.position;
		if (ScrollHorizontal) T3.x = xPos;
		if (ScrollVertical) T3.y = yPos;
		transform.position = T3;
	}
}
