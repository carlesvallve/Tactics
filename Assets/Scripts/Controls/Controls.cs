using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {

	public LayerMask layerMask;
	private Game game;
	private GameCamera cam;

	private bool mouseIsDown = false;
	private bool mouseHasMoved = false;
	private Vector3 mouseLastPos;
	private Vector3 mouseDelta;
	

	void Awake () {
		game = GetComponent<Game>();
		cam = Camera.main.GetComponent<GameCamera>();
	}


	void Update () {
		UpdateKeyControls();
		UpdateMouseControls();
	}


	private void UpdateKeyControls () {
		// game
		if (Input.GetKeyDown(KeyCode.Return)) {
			game.SelectNextSquad();
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			game.currentSquad.SelectNextPlayer();
		}

		// camera
		if (Input.GetKeyDown(KeyCode.A)) {
			cam.movement = new Vector3(-cam.offsetSpeed, cam.movement.y, cam.movement.z);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			cam.movement = new Vector3(cam.offsetSpeed, cam.movement.y, cam.movement.z);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			cam.movement = new Vector3(cam.movement.x, cam.movement.y, cam.offsetSpeed);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			cam.movement = new Vector3(cam.movement.x, cam.movement.y, -cam.offsetSpeed);
		}
		if (Input.GetKeyDown(KeyCode.Q)) { cam.RotateAroundTarget(-1); }
		if (Input.GetKeyDown(KeyCode.E)) { cam.RotateAroundTarget(1); }
	}


	private void UpdateMouseControls () {
		// on mouse down
		if (Input.GetButtonDown("Fire1")) {
			mouseIsDown = true;
			mouseHasMoved = false;
			mouseLastPos = Input.mousePosition;
		}

		// while mouse is down
		if (mouseIsDown) {
			Vector2 delta = (Input.mousePosition - mouseLastPos);
			mouseLastPos = Input.mousePosition;

			mouseDelta = cam.transform.TransformDirection(new Vector3(delta.x, 0, delta.y));
			if (mouseDelta.magnitude >= 1) {
				mouseHasMoved = true;
			}

			cam.movement -= mouseDelta.normalized * 0.075f;
		}

		// on mouse up
		if (Input.GetButtonUp("Fire1")) {
			mouseIsDown = false;

			if (PointerInteraction.IsPointerOverGameObject()) {
				return;
			}

			if (mouseHasMoved) { 
				return;
			}
			
			RaycastHit hit = GetHit(Input.mousePosition);
			if (!hit.transform) { return; }

			string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);

			switch (layerName) {
			case "Grid":
				TapOnGrid(hit);
				break;
			case "Player":
				TapOnPlayer(hit);
				break;
			}
		}
	}


	private RaycastHit GetHit(Vector3 pos) {
		// check colliders for all layers in LayerMask
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1000, layerMask)) {}
		return hit;
	}


	private void TapOnGrid (RaycastHit hit) {
		Vector3 pos = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z));
		if (pos.x < 0 || pos.z < 0 || pos.x > Grid.xsize - 1 || pos.z > Grid.ysize - 1) { return; }

		game.currentSquad.SetPlayerPath(pos);
	}


	private void TapOnPlayer (RaycastHit hit) {
		Player player = hit.transform.parent.GetComponent<Player>();
		if (player.squad == game.currentSquad) {
			game.currentSquad.SelectPlayer(player);
		} else {
			game.currentSquad.currentPlayer.Aim(player);
		}
	}
}
