using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameContainers {
	public GameObject fx;
}

public class Game : MonoBehaviour {
	
	public GameContainers containers = new GameContainers();

	private MapGenerator map;
	private List<Entity> players;
	private Entity player;
	
	void Awake () {
		InitMap();
		InitGrid();
		InitPlayers();
	}

	private void InitMap () {
		map = GetComponent<MapGenerator>();
		map.Generate();
	}

	private void InitGrid () {
		// initialice empty grid
		Grid.InitEmpty(map.width, map.height);

		// set grid walkability
		for (int y = 0; y < map.height - 0; y++) {
			for (int x = 0; x < map.width - 0; x++) {
				Vector3 startPoint = new Vector3(x, 10, y);
				Vector3 endPoint = new Vector3(x, 0, y);
				RaycastHit hit = Utilities.SetRay(startPoint, endPoint, 10);
				bool walkable = hit.point.y == 0;
				Grid.SetWalkable((float)x, (float)y, walkable);
			}
		}
	}

	private void InitPlayers () {
		players = new List<Entity>();

		Entity[] playerComponents = FindObjectsOfType<Entity>();
		foreach (Entity playerComponent in playerComponents) {
			players.Add(playerComponent);
		}

		SelectPlayer(players[0]);
	}

	public void SelectPlayer (Entity player) {
		if (player.moving) { return; }

		this.player = player;

		for (int i = 0; i < players.Count; i++) {
			players[i].Deselect();
		}

		player.Select();
	}

	public void SetPlayerPath (Vector3 pos) {
		player.SetPath(pos);
	}
}
