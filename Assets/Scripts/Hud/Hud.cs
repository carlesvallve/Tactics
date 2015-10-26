using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	public GameObject enemyIconPrefab;
	private List<GameObject> enemyIcons;

	private Game game;
	private Transform header;

	public void Init (Game game) {
		this.game = game;

		Transform hud = GameObject.Find("Hud").transform;
		header = hud.Find("Header");

		enemyIcons = new List<GameObject>();
	}


	public void ClearEnemyIcons () {
		for (int i = 0; i < enemyIcons.Count; i ++) {
			Destroy(enemyIcons[i]);
		}

		enemyIcons.Clear();
	}

	
	public void SetEnemyIcons (List<Player> enemies) {
		ClearEnemyIcons();

		for (int i = 0; i < enemies.Count; i ++) {
			Player enemy = enemies[i];

			GameObject obj = (GameObject)Instantiate(enemyIconPrefab);
			obj.transform.SetParent(header);
			obj.transform.localScale = new Vector3(1, 1, 1);
			obj.name = "EnemyIcon";

			enemyIcons.Add(obj);

			Image image = obj.GetComponent<Image>();
			image.color = enemy.color;

			Button button = obj.GetComponent<Button>();
			button.onClick.AddListener( delegate { SelectEnemyIcon(enemy); } );
		}

		RectTransform rect = header.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(enemyIcons.Count * 50, 50);
	}


	public void SelectEnemyIcon (Player enemy) {
		game.currentSquad.currentPlayer.SetAim(enemy);
	}
}
