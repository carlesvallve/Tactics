﻿using UnityEngine;
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
		squads.Add(CreateSquad(8, Vector3.zero, GameSettings.colors.magenta));
		SelectSquad(0);
	}


	private Squad CreateSquad(int maxPlayers, Vector3 pos, Color color) {

		GameObject obj = (GameObject)Instantiate(squadPrefab);
		obj.transform.SetParent(containers.grid);
		obj.name = "Squad";

		Squad squad = obj.GetComponent<Squad>();
		squad.Init(maxPlayers, pos, color);

		return squad;
	}


	private void SelectNextSquad () {
		currentSquadNum += 1;
		if (currentSquadNum > squads.Count - 1) {
			currentSquadNum = 0;
		} 

		SelectSquad(currentSquadNum);
	}

	private void SelectSquad (int currentSquadNum) {
		this.currentSquadNum = currentSquadNum;
		currentSquad = squads[currentSquadNum];
	}
}
