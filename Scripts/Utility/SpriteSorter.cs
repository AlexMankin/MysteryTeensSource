using UnityEngine;
using System.Collections;

public class SpriteSorter : MonoBehaviour {

	public bool SortMe = true;
    public int sortYOffset = 0;
	public Transform objectTransform;

	private SpriteRenderer render;
	private int layerNum;

	private Vector3 T3;

	// Use this for initialization
	void Start () {
		render = GetComponent<SpriteRenderer> ();
		if (!objectTransform)
			objectTransform = transform;
	}

	// Update is called once per frame
	void Update () {
		if (SortMe) {
			render.sortingOrder = -(int)(objectTransform.position.y) - sortYOffset;
		}
	}
}