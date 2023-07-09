/* 
	Found this one online:
	https://forum.unity.com/threads/making-edge-colliders-and-polygon-colliders-snap-to-grid.429808/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonColliderSnapper : MonoBehaviour {

	public PolygonCollider2D polygon;

	// Use this for initialization
	void Start () {
		if (polygon == null)
			polygon = GetComponent<PolygonCollider2D>();
	}

	// Update is called once per frame
	void Update () {

	}

	public void Snap()
	{
		int counter = 0;
		for (int pathIndex = 0; pathIndex < polygon.pathCount; pathIndex++)
		{
			Vector2[] path = polygon.GetPath(pathIndex);
			for (long pointIndex = 0; pointIndex < path.Length; pointIndex++)
			{
				counter++;
				Vector2 point = path[pointIndex];
				path[pointIndex] = new Vector2(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
			}
			polygon.points = path;
			polygon.SetPath(pathIndex, path);
		}
		print("Snapped " + counter + " points.");
	}
}