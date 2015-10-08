using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	private float speed = 0.2f;
	private float friction = 0.9f;

	private Vector3 movement;


	void Update () {
		if (Input.GetKey(KeyCode.A)) {
			movement = new Vector3(-speed, movement.y, movement.z);
		}
		if (Input.GetKey(KeyCode.D)) {
			movement = new Vector3(speed, movement.y, movement.z);
		}
		if (Input.GetKey(KeyCode.W)) {
			movement = new Vector3(movement.x, movement.y, speed);
		}
		if (Input.GetKey(KeyCode.S)) {
			movement = new Vector3(movement.x, movement.y, -speed);
		}

		movement *= friction;

		transform.Translate(movement, Space.World);
	}
}
