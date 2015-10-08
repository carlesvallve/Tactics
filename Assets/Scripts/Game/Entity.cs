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
		path = Grid.astar.SearchPath(
			(int)transform.localPosition.x, 
			(int)transform.localPosition.z,
			(int)pos.x, 
			(int)pos.z
		);

		pathRenderer.Render(path);
	}

}
