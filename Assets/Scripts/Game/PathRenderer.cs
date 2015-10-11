using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject tilePrefab;
	public GameObject dotPrefab;
	
	private Entity entity;

	//private List<GameObject> area;
	private GameObject area;
	private List<GameObject> dots;
	private LineRenderer lineRenderer;


	void Awake () {
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(Color.red, Color.yellow);
        lineRenderer.SetWidth(0.05f, 0.05f);
        lineRenderer.enabled = false;
	}

	public void Init (Entity entity) {
		this.entity = entity;
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


	public void CreateArea (int radius) {
		DestroyArea();

		area = new GameObject("Area");
		area.transform.SetParent(transform, false);


		for (int y = -radius * 2; y <= radius * 2; y++) {
			for (int x = -radius * 2; x <= radius * 2; x++) {
				
				Vector3 pos = new Vector3(
					entity.transform.localPosition.x + x, 
					0, 
					entity.transform.localPosition.z + y
				);

				if (pos.x < 0 || pos.z < 0 || pos.x > Grid.xsize - 1 || pos.z > Grid.ysize - 1) { 
					continue; 
				}

				GameObject obj = (GameObject)Instantiate(tilePrefab);
				obj.transform.localPosition = pos;
				obj.transform.SetParent(area.transform, false);

				Color color = Mathf.Abs(x) <= radius && Mathf.Abs(y) <= radius? Color.cyan : Color.yellow;
				obj.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = color;
			}
		}
	}


	public void DestroyArea () {
		if (area == null) { return; }
		Destroy(area);
		area = null;
	}



}
