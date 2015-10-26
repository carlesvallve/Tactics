using UnityEngine;
using System.Collections;

public enum CameraMode {
	Normal = 1,
	Aim = 2,
}

public class GameCamera : MonoBehaviour {

	// main parameters
	public Vector3 distance = new Vector3(-16f, 16f, -16f);

	// track parameters
	private GameObject target;
	public float trackSpeed = 5f;

	// rotation parameters
	public float rotationSpeed = 0.75f;
	private bool rotating = false;

	// zoom parameters
	public float zoomSpeed = 0.1f;
	private int zoomMode = -1;

	// offset parameters
	public Vector3 movement { get; set; }
	public Vector3 offset { get; set; }
	public float offsetSpeed = 0.5f;
	public float offsetFriction = 0.85f;

	private CameraMode cameraMode = CameraMode.Normal;


	void LateUpdate () {
		if (cameraMode == CameraMode.Aim) { 
			return; 
		}

		SetOffset();
		SetZoom();
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


	// =======================================================
	// Camera Pan
	// =======================================================

	private void SetOffset () {
		movement *= offsetFriction;
		offset += movement;
	}

	// =======================================================
	// Camera Zoom
	// =======================================================

	public void ToggleZoom () {
		zoomMode = -zoomMode;
		if (zoomMode > 0) {
			distance *= 1.6f;
		} else {
			distance /= 1.6f;
		}
	}

	private void SetZoom () {
		float delta = -Input.GetAxis("Mouse ScrollWheel");
		if (delta == 0) { return; }
		distance *= delta > 0 ? 1 + zoomSpeed : 1 - zoomSpeed;
	}


	// =======================================================
	// Camera Rotation
	// =======================================================

	public void RotateAroundTarget(int dir) {
		if (rotating) { return; }

		StartCoroutine(RotateAroundPoint(
			target.transform.localPosition + new Vector3(offset.x, 0, offset.z), 
			new Vector3(0,1,0), 
			-dir * 90, 
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


	public IEnumerator SetAim (Player player, Player enemy) {
		cameraMode = CameraMode.Aim;

		float duration = 1f;

		Vector3 lookAtPos = enemy.transform.localPosition + Vector3.up * 0.5f;
		player.body.transform.LookAt(
			new Vector3(lookAtPos.x, player.transform.localPosition.y, lookAtPos.z)
		);

		Vector3 endPos = player.transform.localPosition + 
		player.body.transform.up * 0.25f - 
		player.body.transform.forward * 2.5f + 
		player.body.transform.right * Random.Range(-1, 2);
		
		float endFov = 30;

		Quaternion startRotation = transform.rotation;

		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, (Time.time - startTime) / duration);

			Vector3 relativePos = lookAtPos - transform.position;
        	Quaternion rotation = Quaternion.LookRotation(relativePos);
    		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, (Time.time - startTime) / duration); //Time.deltaTime * 8);
			
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endFov, (Time.time - startTime) / duration);
			yield return null;
		}

 		transform.localPosition = endPos;
 		transform.LookAt(lookAtPos);

 		//cameraMode = CameraMode.Normal;
	}
}
