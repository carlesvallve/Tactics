﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject selectorPrefab;
	public GameObject dotPrefab;
	private List<PathDot> dots;

	private Entity entity;
	private GameObject selector;

	public void Init (Entity entity) {
		this.entity = entity;
		CreateSelector();
	}


	public void CreateSelector () {
		selector = (GameObject)Instantiate(selectorPrefab);
		selector.transform.SetParent(entity.transform);
		selector.transform.localPosition = Vector3.zero;
		selector.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

		SpriteRenderer sprite = selector.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		sprite.color = Color.cyan;
	}


	public void DisplaySelector (bool value) {
		selector.SetActive(value);
	}


	public void CreatePath (List<Vector2> path, int movement) {
		DestroyPath();

		movement -= 1;

		dots = new List<PathDot>();

		for (int i = 0; i < path.Count; i++) {
			// get pos
			Vector3 pos = new Vector3(path[i].x, 0.02f, path[i].y);

			// get color
			Color color = Color.grey;
			if (i <= movement) { color = Color.yellow; }
			if (i <= movement / 2) { color = Color.cyan; }

			// get scale
			float sc = ((i == path.Count - 1 && i <= movement) || i == movement) ? 1.5f : 0.75f;
			Vector3 scale = new Vector3(sc, sc, sc);

			// create path dot
			GameObject obj = (GameObject)Instantiate(dotPrefab);
			obj.transform.SetParent(transform, false);

			PathDot dot = obj.GetComponent<PathDot>();
			dot.Init(i, pos, scale, color);

			dots.Add(dot);
		}
	}


	public void DestroyPath () {
		if (dots == null) { return; }
		
		for (int i = 0; i < dots.Count; i++) {
			DestroyDot(i);
		}

		dots = null;
	}


	public void DestroyDot(int i) {
		if (dots[i] == null) { return; }

		Destroy(dots[i].gameObject);
	}

}
