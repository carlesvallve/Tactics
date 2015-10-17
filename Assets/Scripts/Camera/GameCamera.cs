using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public float distance = 16f;
	
	// track parameters
	private GameObject target;
	private float trackSpeed = 5f;
	
	// offset parameters
	private Vector3 movement;
	private Vector3 offset;
	private float offsetSpeed = 0.5f;
	private float offsetFriction = 0.85f;

	
	void Update () {
		SetOffset();
		SetZoom();
		TrackTarget();
	}


	public void SetTarget (GameObject obj) {
		target = obj;
		offset = Vector3.zero;
	}


	public void TrackTarget () {
		if (target == null) {
			return;
		}

		Vector3 pos = new Vector3(
			target.transform.localPosition.x - distance + offset.x, 
			distance * 1.4f, 
			target.transform.localPosition.z - distance + offset.z
		);

		transform.localPosition = Vector3.Lerp(transform.localPosition, pos, Time.deltaTime * trackSpeed);
	}


	private void SetOffset () {
		if (Input.GetKeyDown(KeyCode.A)) {
			movement = new Vector3(-offsetSpeed, movement.y, movement.z);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			movement = new Vector3(offsetSpeed, movement.y, movement.z);
		}
		if (Input.GetKeyDown(KeyCode.W)) {
			movement = new Vector3(movement.x, movement.y, offsetSpeed);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			movement = new Vector3(movement.x, movement.y, -offsetSpeed);
		}

		movement *= offsetFriction;
		offset += movement;
	}


	private void SetZoom () {
		float delta = -Input.GetAxis("Mouse ScrollWheel");
		distance += delta * 10f;
	}
}
