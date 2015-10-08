using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour {
	private Game game;
	public GameObject pathPrefab;

	private PathRenderer pathRenderer;
	private List<Vector2> path = new List<Vector2>();
	

	void Awake () {
		game = GameObject.Find("Game").GetComponent<Game>();

		CreatePathRenderer();
	}


	private void CreatePathRenderer () {
		GameObject obj = (GameObject)Instantiate(pathPrefab);
		obj.transform.SetParent(game.containers.fx.transform);
		pathRenderer  = obj.GetComponent<PathRenderer>();
	}


	public void SetPath (Vector3 pos) {
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

		pathRenderer.CreatePath(path);
	}


	public void FollowPath () {
		if (path == null) { return; }

		StartCoroutine(FollowPathAnim());
	}


	private IEnumerator FollowPathAnim () {
		//foreach (Vector3 point in path) {
		while (path.Count > 0) {
			transform.position = new Vector3(path[0].x, 0, path[0].y);

			yield return new WaitForSeconds(0.15f);

			path.RemoveAt(0);
			pathRenderer.CreatePath(path);
		}

		//pathRenderer.DestroyPath();
		path = null;
	}

}
