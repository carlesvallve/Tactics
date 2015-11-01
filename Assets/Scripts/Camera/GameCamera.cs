using UnityEngine;
using System.Collections;

public enum CameraMode {
	Normal = 1,
	Aiming = 2,
	Rotating = 3
}

public class GameCamera : MonoBehaviour {
	public static GameCamera instance;

	// camera mode
	public CameraMode mode = CameraMode.Normal;

	// main parameters
	public Vector3 distance = new Vector3(-16f, 16f, -16f);

	// track parameters
	private GameObject target;
	public float trackSpeed = 5f;

	// rotation parameters
	public float rotationSpeed = 0.75f;

	// zoom parameters
	public float zoomSpeed = 0.1f;
	private int zoomMode = -1;

	// offset parameters
	public Vector3 movement { get; set; }
	public Vector3 offset { get; set; }
	public float offsetSpeed = 0.5f;
	public float offsetFriction = 0.85f;

	// normal pos, rot, fov
	private Vector3 normalModePosition;
	private Quaternion normalModeRotation;
	private float normalFov;

	void Awake () {
		instance = this;
	}

	void LateUpdate () {
		if (mode == CameraMode.Aiming) { 
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
		if (mode != CameraMode.Normal) { return; }

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
		if (mode != CameraMode.Normal) { return; }
		
		StartCoroutine(RotateAroundPoint(
			target.transform.localPosition + new Vector3(offset.x, 0, offset.z), 
			new Vector3(0,1,0), 
			-dir * 90, 
			rotationSpeed)
		);
	}


	private IEnumerator RotateAroundPoint(Vector3 point, Vector3 axis, float rotateAmount, float rotateTime) {
		mode = CameraMode.Rotating;

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

		mode = CameraMode.Normal;
	}


	// =======================================================
	// Camera Mode Transitions
	// =======================================================

	// Aiming Mode

	public void SetAimingMode (Vector3 lookFromPos, Vector3 lookAtPos) {
		if (mode == CameraMode.Rotating) {  return; }

		StopAllCoroutines();
		StartCoroutine(SetAimingModeCoroutine(lookFromPos, lookAtPos));
	}

	public IEnumerator SetAimingModeCoroutine (Vector3 lookFromPos, Vector3 lookAtPos) {
		mode = CameraMode.Aiming;

		// get directions
		Vector3 forward = (lookAtPos - lookFromPos).normalized;
		Vector3 right = Vector3.Cross(forward, Vector3.up).normalized;

		// get end pos
		Vector3 endPos = lookFromPos + 
		Vector3.up * Random.Range(0.25f, 1.5f) - 
		forward * Random.Range(1f, 3f) + 
		right * Utilities.GetRandomSign();

		// get end rot
		Quaternion endRot = Quaternion.LookRotation(forward);
		
		// get end fov
		float endFov = 35;

		float duration = 1.5f;
		float t = 0;

		while(t <= 1f) {
			t += Time.deltaTime / duration;
			float step =  Mathf.SmoothStep(0, 1, t);

			// position
			transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, step / 2);

			// rotation
			forward = (lookAtPos - transform.localPosition).normalized;
			endRot = Quaternion.LookRotation(forward);
			transform.localRotation = Quaternion.Lerp(transform.localRotation, endRot, step);
			
			// fov
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endFov, step / 3);

			yield return null;
		}

		transform.localPosition = endPos;
		transform.localRotation = endRot;
		Camera.main.fieldOfView = endFov;
	}


	// Normal Mode

	public void SetNormalMode () {
		if (mode != CameraMode.Aiming) { return; }

		StopAllCoroutines();
		StartCoroutine(SetNormalModeCoroutine());
	}

	public IEnumerator SetNormalModeCoroutine () {
		
		Vector3 endPos = normalModePosition;
		Quaternion endRot = normalModeRotation;
		float endFov = normalFov;

		float duration = 1.5f;
		float t = 0;

		while(t <= 1f) {
			t += Time.deltaTime / duration;
			float step =  Mathf.SmoothStep(0, 1, t);

			transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, step / 2);
			transform.localRotation = Quaternion.Slerp(transform.rotation, endRot, step);
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, endFov, step / 2);

			yield return null;
		}

		transform.localPosition = endPos;
		transform.localRotation = endRot;
		Camera.main.fieldOfView = endFov;

		mode = CameraMode.Normal;
	}
}
