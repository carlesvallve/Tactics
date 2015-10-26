using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vision : MonoBehaviour {

	public static List<Player> LOS (Player player, List<Player> enemies, bool debug = false) {
		player.pathRenderer.ClearLines();

		List<Player> visibleEnemies = new List<Player>();

		for (int i = 0; i < enemies.Count; i++) {
			Player enemy = enemies[i];
			HideEnemy(enemy);

			if (CastVisionLines(player, enemy, debug) > 0) {
				ShowEnemy(enemy);
				visibleEnemies.Add(enemy);
			}
		}
		
		return visibleEnemies;
	}


	private static int CastVisionLines (Player player, Player enemy, bool debug) {
		// get player and enemy positions
		Vector3 centerPlayer = player.transform.localPosition + Vector3.up * 0.5f;
		Vector3 centerEnemy = enemy.transform.localPosition + Vector3.up * 0.5f;

		float distance = Vector3.Distance(centerPlayer, centerEnemy);
		if (distance > player.visionRange) {
			return 0;
		}

		// get perpendicular vector
		Vector3 forward = (centerPlayer - centerEnemy).normalized;
		Vector3 vec = Vector3.Cross(forward, Vector3.up).normalized;
		if (debug) {
			player.pathRenderer.DrawLine(centerPlayer - vec, centerPlayer + vec, Color.grey); 
			player.pathRenderer.DrawLine(centerEnemy - vec * 0.5f, centerEnemy + vec * 0.5f, Color.grey); 
		}
		
		// cast vision lines
		int i = 0;

		i += CastVisionLine(player, enemy, centerPlayer, centerEnemy, debug);
		i += CastVisionLine(player, enemy, centerPlayer, centerEnemy - vec * 0.5f, debug);
		i += CastVisionLine(player, enemy, centerPlayer, centerEnemy + vec * 0.5f, debug);

		i += CastVisionLine(player, enemy, centerPlayer - vec * 0.5f, centerEnemy, debug);
		i += CastVisionLine(player, enemy, centerPlayer + vec * 0.5f, centerEnemy, debug);

		i += CastVisionLine(player, enemy, centerPlayer - vec * 0.5f, centerEnemy - vec * 0.5f, debug);
		i += CastVisionLine(player, enemy, centerPlayer + vec * 0.5f, centerEnemy + vec * 0.5f, debug);

		i += CastVisionLine(player, enemy, centerPlayer - vec * 0.5f, centerEnemy + vec * 0.5f, debug);
		i += CastVisionLine(player, enemy, centerPlayer + vec * 0.5f, centerEnemy - vec * 0.5f, debug);

		i += CastVisionLine(player, enemy, centerPlayer - vec, centerEnemy - vec * 0.5f, debug);
		i += CastVisionLine(player, enemy, centerPlayer + vec, centerEnemy + vec * 0.5f, debug);

		//i += CastVisionLine(player, enemy, centerPlayer - vec, centerEnemy - vec, debug);
		//i += CastVisionLine(player, enemy, centerPlayer + vec, centerEnemy + vec, debug);

		// if at least one line is successfull, enemy will be visible
		return i;
	}


	private static int CastVisionLine (Player player, Player enemy, Vector3 startPos, Vector3 endPos, bool debug) {
		
		// if startPos is occupied by a cube, dont even cast this line
		RaycastHit hitUp = Utilities.SetRay(startPos + Vector3.up * 1.1f, startPos, 1.1f);
		if (hitUp.transform != null && hitUp.transform.gameObject.tag == "Cube") {
			//if (!Grid.GetWalkable(startPos.x, startPos.z)) {
			return 0;
		}
		
		float distance = Vector3.Distance(startPos, endPos);
		RaycastHit hit = Utilities.SetRay(startPos, endPos, distance);
				
		Color color = Color.green;
		if (hit.transform != null) {
			if (hit.transform.gameObject.tag == "Cube") {
				color = Color.red;
			}
		}

		if (debug) {
			player.pathRenderer.DrawLine(startPos, endPos, color); 
		}

		return color == Color.green ? 1 : 0;
	}


	private static void HideEnemy (Player enemy) {
		enemy.gameObject.SetActive(false);
	}


	private static void ShowEnemy (Player enemy) {
		enemy.gameObject.SetActive(true);
	}

}
