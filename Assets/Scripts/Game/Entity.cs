using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {
	private Game game;
	private GameCamera cam;

	public int movement = 4;
	public float speed = 0.2f; // duration for walking one step
	public GameObject pathPrefab;

	
	private PathRenderer pathRenderer;
	private List<Vector2> path = new List<Vector2>();

	public bool moving { get; private set; }

	private GameObject body;
	private Material material;
	

	void Awake () {
		game = GameObject.Find("Game").GetComponent<Game>();
		cam = Camera.main.GetComponent<GameCamera>();
		body = transform.Find("Body").gameObject;
		material = body.GetComponent<Renderer>().material;
		CreatePathRenderer();
	}


	void Update () {
		SetBodyOutline();
	}


	private void SetBodyOutline () {
		float maxWidth = 0.0004f;
		float outlineWidth = maxWidth * 16f / cam.distance;

		if (outlineWidth > maxWidth) { outlineWidth = maxWidth; }
		//if (!selector.activeSelf) { outlineWidth = 0; }

		material.SetFloat("_Outline", outlineWidth); 
	}


	public void Deselect () {
		pathRenderer.DestroyPath();
		pathRenderer.DisplaySelector(false);
	}


	public void Select () {
		pathRenderer.DisplaySelector(true);
	}


	private void CreatePathRenderer () {
		GameObject obj = (GameObject)Instantiate(pathPrefab);
		obj.transform.SetParent(game.containers.fx.transform);
		pathRenderer  = obj.GetComponent<PathRenderer>();
		pathRenderer.Init(this);
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


	private IEnumerator FollowPathAnim () {
		moving = true;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, true);

		int step = 0;

		while (path.Count > 0) {
			Vector3 point = new Vector3(path[0].x, 0, path[0].y);
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


	private IEnumerator MoveToPos(Vector3 endPos, float duration) {
		Vector3 startPos = transform.localPosition;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.position = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
			yield return null;
		}

		transform.position = endPos;
	}
}
