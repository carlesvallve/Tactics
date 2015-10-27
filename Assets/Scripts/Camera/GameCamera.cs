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


	private bool transitioning = false;
	private Vector3 normalModePosition;
	private Quaternion normalModeRotation;
	private float normalFov;


	void LateUpdate () {
		if (cameraMode == CameraMode.Aim) { 
			return; 
		}

		SetOffset();
		SetZoom();
		TrackTarget();

		normalModePosition = transform.localPosition;
		normalModeRotation = transform.rotation;
		normalFov = Camera.main.fieldOfView;
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
		if (transitioning) { return; }
		if (cameraMode != CameraMode.Normal) { return; }
		

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


	// =======================================================
	// Camera Mode Transitions
	// =======================================================

	public IEnumerator SetAimMode (Player player, Player enemy) {
		if (transitioning) { yield break; }

		transitioning = true;
		cameraMode = CameraMode.Aim;

		Vector3 lookAtPos = enemy.transform.localPosition + Vector3.up * 0.5f;
		player.body.transform.LookAt(
			new Vector3(lookAtPos.x, player.transform.localPosition.y + Utilities.GetMeshBounds(player.body).size.y / 2, lookAtPos.z)
		);

		Vector3 endPos = player.transform.localPosition + 
		player.body.transform.up * Random.Range(0.25f, 1.5f) - 
		player.body.transform.forward * Random.Range(1f, 3f) + 
		player.body.transform.right * Utilities.GetRandomSign();

		Vector3 direction = lookAtPos - transform.localPosition;
		Quaternion endRot = Quaternion.LookRotation(direction);
		
		float endFov = 40;

		float duration = 3f;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, (Time.time - startTime) / duration);

			direction = lookAtPos - transform.localPosition;
			endRot = Quaternion.LookRotation(direction);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, endRot, (Time.time - startTime) / duration * 2f);
			
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endFov, (Time.time - startTime) / duration);

			/*if (Vector3.Distance(transform.localPosition, endPos) <= 0.1f) {
				break;
			}*/
			
			yield return null;
		}

		transform.localPosition = endPos;
		transform.localRotation = endRot;
		Camera.main.fieldOfView = endFov;

		transitioning = false;
	}


	public IEnumerator SetNormalMode () {
		if (transitioning) { yield break; }
		if (cameraMode == CameraMode.Normal) { yield break; }

		transitioning = true;

		Vector3 endPos = normalModePosition;
		Quaternion endRot = normalModeRotation;
		float endFov = normalFov;

		float duration = 3f;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, (Time.time - startTime) / duration);
			transform.rotation = Quaternion.Slerp(transform.rotation, endRot, (Time.time - startTime) / duration * 2f);
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endFov, (Time.time - startTime) / duration);

			/*if (Vector3.Distance(transform.localPosition, endPos) <= 0.1f) {
				break;
			}*/

			yield return null;
		}

		transform.localPosition = endPos;
		transform.rotation = normalModeRotation;
		Camera.main.fieldOfView = endFov;

		cameraMode = CameraMode.Normal;
		transitioning = false;
	}
}
