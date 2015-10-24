using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	// main parameters
	public Vector3 distance = new Vector3(-16f, 8f, -16f);

	// track parameters
	private GameObject target;
	public float trackSpeed = 5f;

	// rotation parameters
	public float rotationSpeed = 0.75f;
	private bool rotating = false;

	// zoom parameters
	public float zoomSpeed = 0.1f;

	// offset parameters
	private Vector3 movement;
	private Vector3 offset;
	public float offsetSpeed = 0.5f;
	public float offsetFriction = 0.85f;


	void LateUpdate () {
		SetOffset();
		SetZoom();
		SetRotation();
		TrackTarget();
	}


	public void SetTarget (GameObject obj) {
		target = obj;
		offset = Vector3.zero;
		TrackTarget();
	}


	public void TrackTarget () {
		if (rotating) { return; }

		Vector3 pos = Vector3.zero;
		if (target == null) {
			pos = new Vector3(distance.x + offset.x, distance.y * 1.4f, distance.z + offset.z);
		} else {
			pos = new Vector3(
				target.transform.localPosition.x + distance.x + offset.x, 
				target.transform.localPosition.y + distance.y * 1.4f, 
				target.transform.localPosition.z + distance.z + offset.z
			);
		}

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
		if (delta == 0) { return; }
		distance *= delta > 0 ? 1 + zoomSpeed : 1 - zoomSpeed;
	}


	private void SetRotation () {
		if (Input.GetKeyDown(KeyCode.Z)) { RotateAroundTarget(1); }
		if (Input.GetKeyDown(KeyCode.C)) { RotateAroundTarget(-1); }
	}


	private void RotateAroundTarget(int dir) {
		if (rotating) { return; }

		StartCoroutine(RotateAroundPoint(
			target.transform.localPosition, 
			new Vector3(0,1,0), 
			dir * 90, 
			rotationSpeed)
		);
	}


	private IEnumerator RotateAroundPoint(Vector3 point, Vector3 axis, float rotateAmount, float rotateTime) {
		rotating = true;

		float step = 0; //non-smoothed
		float rate = 1 / rotateTime; //amount to increase non-smooth step by
		float smoothStep = 0; //smooth step this time
		float lastStep = 0; //smooth step last time
		
		while(step < 1) { // until we're done
			step += Time.deltaTime * rate; //increase the step
			smoothStep = Mathf.SmoothStep(0, 1, step); //get the smooth step
			transform.RotateAround(point, axis, rotateAmount * (smoothStep - lastStep));
			lastStep = smoothStep; //store the smooth step

			yield return null;
		}

		//finish any left-over
		if(step > 1) {
			transform.RotateAround(point, axis,rotateAmount * (1 - lastStep));
		}

		// update distance vector
		distance = new Vector3(
			transform.localPosition.x - point.x,
			(transform.localPosition.y - point.y) / 1.4f,
			transform.localPosition.z - point.z
		);	

		rotating = false;
	}
}
