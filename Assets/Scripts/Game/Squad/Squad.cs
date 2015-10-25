using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Squad : MonoBehaviour {
	private GameCamera cam;

	public GameObject playerPrefab;
	public List<Player> players;
	public Color color;

	private Player currentPlayer;
	private int currentPlayerNum;


	void Awake () {
		cam = Camera.main.GetComponent<GameCamera>();
	}


	public void Init (int maxPlayers, Vector3 pos, Color color) {
		this.color = color;
		
		CreatePlayers(maxPlayers, pos);
	}


	// =============================================
	// Player Creation
	// =============================================

	private void CreatePlayers (int maxPlayers, Vector3 pos) {
		players = new List<Player>();

		for (int i = 0; i < maxPlayers; i++) {
			Player player = CreatePlayer(i, pos);
			players.Add(player);
		}
	}


	private Player CreatePlayer (int num, Vector3 pos) {
		GameObject obj = (GameObject)Instantiate(playerPrefab);
		obj.transform.SetParent(transform);
		obj.name = "Player" + num;

		pos = GetPlayerPos(pos);

		Player player = obj.GetComponent<Player>();
		player.Init(num, pos, color);

		return player;
	}


	private Vector3 GetPlayerPos (Vector3 initialPos) {
		int radius = 1;
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
		cam.SetTarget(currentPlayer.gameObject);
	}


	public void SetPlayerPath (Vector3 pos) {
		if (currentPlayer == null) { return; }
		currentPlayer.SetPath(pos);
	}
}
