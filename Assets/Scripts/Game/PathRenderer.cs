using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject dotPrefab;
	
	private List<GameObject> dots;
	private LineRenderer lineRenderer;
	

	void Awake () {
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(Color.red, Color.yellow);
        lineRenderer.SetWidth(0.05f, 0.05f);
        lineRenderer.enabled = false;
	}


	public void CreatePath (List<Vector2> path) {
		DestroyPath();

		lineRenderer.SetVertexCount(path.Count);
		dots = new List<GameObject>();

		for (int i = 0; i < path.Count; i++) {
			Vector3 point = new Vector3(path[i].x, 0.02f, path[i].y);

			GameObject dot = (GameObject)Instantiate(dotPrefab);
			dot.transform.localPosition = point;
			dot.transform.SetParent(transform, false);

			//float sc = (i == 0 || i == path.Count -1) ? 1.5f : 0.75f;
			float sc = (i == path.Count -1) ? 1.5f : 0.75f;
			dot.transform.localScale = new Vector3(sc, sc, sc);
			dots.Add(dot);

			lineRenderer.SetPosition(i, point);
		}
	}


	public void DestroyPath () {
		if (dots == null) { return; }
		
		lineRenderer.SetVertexCount(1);
		for (int i = 0; i < dots.Count; i++) {
			Destroy(dots[i]);
		}

		dots = null;
	}

}
