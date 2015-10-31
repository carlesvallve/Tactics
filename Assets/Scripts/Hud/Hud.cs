using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {
	public HudPrefabs prefabs = new HudPrefabs();

	private List<GameObject> enemyIcons;
	private List<Player> visibleEnemies;
	private int currentEnemyIcon;

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

	
	public void SetEnemyIcons (List<Player> visibleEnemies) {
		ClearEnemyIcons();

		this.visibleEnemies = visibleEnemies;
		currentEnemyIcon = visibleEnemies.Count - 1;

		for (int i = 0; i < visibleEnemies.Count; i ++) {
			Player enemy = visibleEnemies[i];

			GameObject obj = (GameObject)Instantiate(prefabs.enemyIcon);
			obj.transform.SetParent(header);
			obj.transform.localScale = new Vector3(1, 1, 1);
			obj.name = "EnemyIcon";

			enemyIcons.Add(obj);

			Image image = obj.GetComponent<Image>();
			image.color = enemy.color;

			Button button = obj.GetComponent<Button>();
			button.onClick.AddListener( delegate { SelectEnemyIcon(i, enemy); } );
		}

		RectTransform rect = header.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(enemyIcons.Count * 50, 50);
	}


	public void SelectNextEnemyIcon () {
		currentEnemyIcon += 1;
		if (currentEnemyIcon > visibleEnemies.Count -1) { currentEnemyIcon = 0; }
		SelectEnemyIcon(currentEnemyIcon, visibleEnemies[currentEnemyIcon]);
	}


	public void SelectEnemyIcon (int num, Player enemy) {
		currentEnemyIcon = num;
		game.currentSquad.currentPlayer.SetAim(enemy);
	}
}
