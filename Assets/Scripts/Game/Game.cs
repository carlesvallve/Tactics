using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameContainers {
	public Transform grid;
	public Transform fx;
}

/*

- create enemy squad
- each cell, run vision algorithm
- if enemy is spotted, 
	- display enemy icon in hud
	- reveal enemy and stop moving
	- move enemy to new point

- select enemy: set camera to shooting mode
- play aiming anim

- shoot enemy:
	- calculate shooting trajectory and hit outcome
	- play shooting anim
	- if enemy is hit, play hit anim and apply damage

- actions:
	- move
	- overwatch
	- aim
	- defend
*/

public class Game : MonoBehaviour {
	
	public static GameContainers containers;

	public GameContainers gameContainers = new GameContainers();
	public GameObject squadPrefab;
	
	private MapGenerator map;

	private List<Squad> squads;
	private int currentSquadNum;
	public Squad currentSquad { get; private set; }


	void Awake () {
		containers = gameContainers;

		InitMap();
		InitGrid();
		InitSquads();
	}


	void Update () {
		CalculateVision();
	}


	// =============================================
	// World
	// =============================================

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


	// =============================================
	// Squads
	// =============================================

	private void InitSquads () {
		squads = new List<Squad>();
		Vector3 center = new Vector3((Grid.xsize - 1) / 2, 0, (Grid.ysize - 1) / 2);
		
		squads.Add(CreateSquad(
			2, center, 1, GameSettings.colors.cyan)
		);
		squads.Add(CreateSquad(
			10, center, 7, GameSettings.colors.red)
		);

		SelectSquadByNum(0);
	}


	private Squad CreateSquad(int maxPlayers, Vector3 pos, int radius, Color color) {
		GameObject obj = (GameObject)Instantiate(squadPrefab);
		obj.transform.SetParent(containers.grid);
		obj.name = "Squad";

		Squad squad = obj.GetComponent<Squad>();
		squad.Init(maxPlayers, pos, radius, color);

		return squad;
	}


	public void SelectNextSquad () {
		currentSquadNum += 1;
		if (currentSquadNum > squads.Count - 1) {
			currentSquadNum = 0;
		} 

		SelectSquadByNum(currentSquadNum);
	}

	public void SelectSquadByNum (int currentSquadNum) {
		if (currentSquad != null) {
			currentSquad.currentPlayer.Deselect();
		}

		this.currentSquadNum = currentSquadNum;
		currentSquad = squads[currentSquadNum];

		for (int i = 0; i < currentSquad.players.Count; i++) {
			currentSquad.players[i].gameObject.SetActive(true);
		}

		currentSquad.SelectPlayerByNum(0);
	}

	// =============================================
	// Vision
	// =============================================

	private List<Player> CalculateVision () {
		Player player = currentSquad.currentPlayer;

		// get list of all enemies in range
		List<Player> enemies = new List<Player>();
		for (int i = 0; i < squads.Count; i++) {
			if (squads[i] == currentSquad) { continue; }
			for (int n = 0; n < squads[i].players.Count; n++) {
				Player enemy = squads[i].players[n];
				float distance = Vector3.Distance(player.transform.localPosition, enemy.transform.localPosition);
				if (distance <= player.visionRange) {
					enemies.Add(enemy);
					enemy.gameObject.SetActive(true);
				} else {
					enemy.gameObject.SetActive(false);
				}
			}
		}


		// cast 8 rays in a circle around player, to a circle around enemy
		// if any of the rays is near enough the enemy, enemy is visible

		for (int i = 0; i < enemies.Count; i++) {
			Player enemy = enemies[i];
			enemy.gameObject.SetActive(false);

			Vector3 centerPlayer = player.transform.localPosition + Vector3.up * 0.25f;
			Vector3 centerEnemy = enemy.transform.localPosition + Vector3.up * 0.25f;
			RaycastHit hit = Utilities.SetRay(centerPlayer, centerEnemy, player.visionRange);

			if (hit.transform) { print (hit.transform.parent); }

			if (hit.transform != null && hit.transform.parent == enemy.transform) { 
				Debug.DrawLine (centerPlayer, hit.point, Color.yellow);
				enemy.gameObject.SetActive(true);
				continue;
			} else {
				Debug.DrawLine (centerPlayer, centerEnemy, Color.magenta);
			}

			/*for (int n = 0; n < 8; n++) {
				Vector3 startPoint = GetPointOnCircle(centerPlayer, 0.6f, n * 360 / 8);
				Vector3 endPoint = GetPointOnCircle(centerEnemy, 0.6f, n * 360 / 8);
				hit = Utilities.SetRay(startPoint, endPoint, player.visionRange);
				
				if (hit.transform != null && hit.transform.parent != enemy.transform) { 
					Debug.DrawLine (startPoint, endPoint, Color.red);
				} else {
					Debug.DrawLine (startPoint, endPoint, Color.green);
					enemy.gameObject.SetActive(true);
				}
			}*/
		}

		return enemies;
	}


	private Vector3 GetPointOnCircle(Vector3 center, float radius, float angle) { 
		return new Vector3 (
			center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad),
			center.y,
			center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad)
		);
	}
}
