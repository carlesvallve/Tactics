﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Humanoid : Entity {
	protected Game game;
	protected GameCamera cam;

	public int movement = 4;
	public float speed = 0.2f; // duration for walking one step
	public GameObject pathPrefab;

	public PathRenderer pathRenderer { get; private set; }
	protected List<Vector2> path = new List<Vector2>();

	public bool moving { get; private set; }

	protected GameObject body;
	protected Material material;

	public Squad squad { get; private set; }
	public int num { get; private set; }
	public Color color { get; private set; }

	public float visionRange = 7;


	public void Init (Squad squad, int num, Vector3 pos, Color color) {
		game = GameObject.Find("Game").GetComponent<Game>();
		cam = Camera.main.GetComponent<GameCamera>();
		body = transform.Find("Body").gameObject;
		material = body.GetComponent<Renderer>().material;

		this.squad = squad;
		this.num = num;
		this.color = color;

		transform.localPosition = pos;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, false);

		CreatePathRenderer();
		SetBodyOutline();
	}


	protected void SetBodyOutline () {
		float outlineWidth = 0.0002f;

		if (material.HasProperty("_Outline")) { material.SetFloat("_Outline", outlineWidth); }
		if (material.HasProperty("_OutlineColor")) { material.SetColor("_OutlineColor", color); } 
	}


	public void Deselect () {
		pathRenderer.DestroyPath();
		pathRenderer.DisplaySelector(false);
		pathRenderer.ClearLines();
	}


	public void Select () {
		pathRenderer.DisplaySelector(true);
	}


	protected void CreatePathRenderer () {
		GameObject obj = (GameObject)Instantiate(pathPrefab);
		obj.transform.SetParent(Game.containers.fx);
		obj.name = "Path " + name;
		pathRenderer  = obj.GetComponent<PathRenderer>();
		pathRenderer.Init(this, color);
	}


	public void SetPath (Vector3 pos) {
		if (moving) { return; }

		if (path != null && path.Count > 0) {
			Vector3 goal = new Vector3(path[path.Count -1].x, 0, path[path.Count -1].y);
			if (pos == goal) {
				FollowPath();
				return;
			}
		}

		path = Grid.astar.SearchPath(
			(int)transform.localPosition.x, 
			(int)transform.localPosition.z,
			(int)pos.x, 
			(int)pos.z
		);

		if (path == null || path.Count == 0) { 
			pathRenderer.DestroyPath();
			return;
		}

		path.RemoveAt(0);
		pathRenderer.CreatePath(path, movement);
	}


	public void FollowPath () {
		if (path == null) { return; }
		StartCoroutine(FollowPathAnim());
	}


	protected IEnumerator FollowPathAnim () {
		moving = true;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, true);

		int step = 0;

		while (path.Count > 0) {
			//Vector3 point = new Vector3(path[0].x, 0, path[0].y);
			Vector3 point = pathRenderer.dots[step].transform.localPosition; 
			yield return StartCoroutine(MoveToPos(point, speed));

			path.RemoveAt(0);
			pathRenderer.DestroyDot(step);
			step += 1;

			if (step == movement) {
				break;
			}
		}

		moving = false;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, false);

		path = null;
	}


	protected IEnumerator MoveToPos(Vector3 endPos, float duration) {
		Vector3 startPos = transform.localPosition;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
			cam.offset *= 0.95f;
			yield return null;
		}

		transform.position = endPos;
	}


	public void Aim (Player target) {
		print ("Aiming towards " + target);
	}

}
