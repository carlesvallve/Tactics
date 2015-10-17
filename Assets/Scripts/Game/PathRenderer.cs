using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject dotPrefab;
	private List<GameObject> dots;
	private LineRenderer lineRenderer;


	


	public void CreatePath (List<Vector2> path, int movement) {
		DestroyPath();

		movement -= 1;

		dots = new List<GameObject>();

		for (int i = 0; i < path.Count; i++) {
			Vector3 point = new Vector3(path[i].x, 0.02f, path[i].y);

			GameObject dot = (GameObject)Instantiate(dotPrefab);
			dot.transform.localPosition = point;
			dot.transform.SetParent(transform, false);

			Color color = Color.grey;
			if (i <= movement) { color = Color.yellow; }
			if (i <= movement / 2) { color = Color.cyan; }

			SpriteRenderer sprite = dot.transform.Find("Sprite").GetComponent<SpriteRenderer>();
			sprite.color = color;

			float sc = ((i == path.Count -1 && i <= movement) || i == movement) ? 1.5f : 0.75f;
			dot.transform.localScale = new Vector3(sc, sc, sc);
			dots.Add(dot);
		}
	}


	public void DestroyDot(int i) {
		Destroy(dots[i]);
	}
	

	public void DestroyPath () {
		if (dots == null) { return; }
		
		for (int i = 0; i < dots.Count; i++) {
			Destroy(dots[i]);
		}

		dots = null;
	}
}
