using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Squad : MonoBehaviour {

	public SquadPrefabs prefabs = new SquadPrefabs();
	public List<Player> players;
	public Color color;

	public Player currentPlayer;
	private int currentPlayerNum;


	public void Init (int maxPlayers, Vector3 pos, int radius, Color color) {
		this.color = color;
		
		CreatePlayers(maxPlayers, pos, radius);
	}


	// =============================================
	// Player Creation
	// =============================================

	private void SetPlayerHandlers (Player player) {
		// update vision
		player.OnVisionUpdated += () => {
			Game.instance.UpdatePlayerVision(player);
		};
	}


	private void CreatePlayers (int maxPlayers, Vector3 pos, int radius) {
		players = new List<Player>();

		for (int i = 0; i < maxPlayers; i++) {
			Player player = CreatePlayer(i, pos, radius);
			players.Add(player);
			SetPlayerHandlers(player);
		}
	}


	private Player CreatePlayer (int num, Vector3 pos, int radius) {
		GameObject obj = (GameObject)Instantiate(prefabs.player);
		obj.transform.SetParent(transform);
		obj.name = "Player" + num;

		pos = GetPlayerPos(pos, radius);

		Player player = obj.GetComponent<Player>();
		player.Init(this, num, pos, color);

		return player;
	}


	private Vector3 GetPlayerPos (Vector3 initialPos, int radius) {
		Vector3 pos = Vector3.zero;

		bool ok = false;
		int c = 0;
		int c2 = 0;

		while (!ok) {
			pos = GetRandomPos(initialPos, radius);
			ok = true;

			RaycastHit hit = Utilities.SetRay(pos + Vector3.up * 10, pos, 10);

			if (hit.transform.gameObject.tag == "Player") {
				ok = false;
			} else {
				pos = new Vector3(pos.x, hit.point.y, pos.z);
			}

			
			if (!ok) {
				c += 1;
				if (c == 100) {
					radius += 1;
					c = 0;
				}

				c2 += 1;
				if (c2 == 1000) { 
					break; 
				}
			}
		}

		return pos;
	}

	private Vector3 GetRandomPos (Vector3 initialPos, int radius) {
		Vector3 pos = new Vector3(
			initialPos.x + Random.Range(-radius, radius + 1),
			0,
			initialPos.z + Random.Range(-radius, radius + 1)
		);

		if (pos.x < 0) { pos.x = 0; }
		if (pos.z < 0) { pos.z = 0; }
		if (pos.x > Grid.xsize - 1) { pos.x = Grid.xsize - 1; }
		if (pos.z > Grid.ysize - 1) { pos.z = Grid.ysize - 1; }

		return pos;
	}


	// =============================================
	// Player Interaction
	// =============================================

	public void SelectNextPlayer () {
		if (currentPlayer != null && currentPlayer.moving) { return; }

		currentPlayerNum += 1;
		if (currentPlayerNum > players.Count - 1) {
			currentPlayerNum = 0;
		}

		SelectPlayerByNum(currentPlayerNum);
	}


	public void SelectPlayerByNum (int num) {
		SelectPlayer(players[num]);
	}
	

	public void SelectPlayer (Player player) {
		if (currentPlayer != null && currentPlayer.moving) { return; }

		currentPlayerNum = player.num;
		currentPlayer = player;

		for (int i = 0; i < players.Count; i++) {
			players[i].Deselect();
		}

		currentPlayer.Select();
		GameCamera.instance.SetTarget(currentPlayer.gameObject);
	}


	public void SetPlayerPath (Vector3 pos) {
		if (currentPlayer == null) { return; }
		currentPlayer.SetPath(pos);
	}
}
