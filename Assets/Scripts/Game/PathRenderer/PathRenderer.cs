using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject selectorPrefab;
	public GameObject dotPrefab;
	private List<PathDot> dots;

	private Entity entity;
	private GameObject selector;

	private List<Cube> cubeShields = new List<Cube>();

	public void Init (Entity entity) {
		this.entity = entity;
		CreateSelector();
	}


	public void CreateSelector () {
		selector = (GameObject)Instantiate(selectorPrefab);
		selector.transform.SetParent(entity.transform);
		selector.transform.localPosition = Vector3.zero;
		selector.transform.localScale = new Vector3(1, 1, 1);

		SpriteRenderer sprite = selector.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		sprite.material.SetColor("_OutlineColor", Color.cyan);
	}


	public void DisplaySelector (bool value) {
		selector.SetActive(value);
	}


	public void CreatePath (List<Vector2> path, int movement) {
		DestroyPath();

		movement -= 1;

		dots = new List<PathDot>();

		
		int goalNum = 0;
		Color goalColor = Color.grey;

		for (int i = 0; i < path.Count; i++) {
			// get pos
			Vector3 pos = new Vector3(path[i].x, 0.02f, path[i].y);

			// get color
			Color color = Color.grey;
			if (i <= movement) { color = Color.yellow; }
			if (i <= movement / 2) { color = Color.cyan; }

			// get scale
			float sc = ((i == path.Count - 1 && i <= movement) || i == movement) ? 0.1f : 0.05f; // 1.5f : 0.75f; // 
			Vector3 scale = new Vector3(sc, sc, sc);

			// get goal
			if ((i == path.Count - 1 && i <= movement) || i == movement) {
				goalNum = i;
				goalColor = color;
			}

			// create path dot
			GameObject obj = (GameObject)Instantiate(dotPrefab);
			obj.transform.SetParent(transform, false);

			PathDot dot = obj.GetComponent<PathDot>();
			dot.Init(i, pos, scale, color);

			dots.Add(dot);
		}

		// set shields at goal dot
		Vector3 goalPos = dots[goalNum].transform.localPosition;
		SetShieldsAtPos(goalPos, goalColor);
	}


	public void DestroyPath () {
		if (dots == null) { return; }
		
		for (int i = 0; i < dots.Count; i++) { 
			DestroyDot(i); 
		}
		dots = null;

		for (int i = 0; i < cubeShields.Count; i++) { 
			cubeShields[i].ResetShields(); 
		}
		cubeShields.Clear();
	}


	public void DestroyDot(int i) {
		if (dots[i] == null) { return; }

		Destroy(dots[i].gameObject);
	}


	private void SetShieldsAtPos (Vector3 pos, Color color) {
		SetShieldInDirection(pos, new Vector3(0, 0, -1), color);
		SetShieldInDirection(pos, new Vector3(0, 0, 1), color);
		SetShieldInDirection(pos, new Vector3(1, 0, 0), color);
		SetShieldInDirection(pos, new Vector3(-1, 0, 0), color);
	}


	private void SetShieldInDirection(Vector3 pos, Vector3 dir, Color color) {
		Ray ray = new Ray(pos, dir);
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast(ray, out hit, 1f)) {
			if (hit.transform.gameObject.tag == "Cube") {
				Cube cube = hit.transform.parent.GetComponent<Cube>();
				if (cube != null) {
					cube.DisplayShield(dir, color);
					cubeShields.Add(cube);
				}
			}
		} 

		Debug.DrawLine(pos, pos + dir, Color.green, 2, false);
	}
}
