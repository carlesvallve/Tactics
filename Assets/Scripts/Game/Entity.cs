using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {

	private PathRenderer pathRenderer;
	private List<Vector2> path = new List<Vector2>();
	

	void Awake () {
		pathRenderer = transform.Find("Path").GetComponent<PathRenderer>();
	}


	public void SetPath (Vector3 pos) {
		path = Grid.astar.SearchPath(
			(int)transform.localPosition.x, 
			(int)transform.localPosition.z,
			(int)pos.x, 
			(int)pos.z
		);

		pathRenderer.Render(path);
	}
}
