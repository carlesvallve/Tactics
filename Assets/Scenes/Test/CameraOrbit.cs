using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour {

	// TODO: refactor camera to have a static target
	public Transform target;
	public Vector3 center = new Vector3(0, 0.35f, 0);
	public Vector3 angle = new Vector3(45, -45, 0);
	public float distance = 10;

	public float distanceMin = 1;
	public float distanceMax = 20;

	public float yMin = -20;
	public float yMax = 80;

	public float xSpeed = 25;
	public float ySpeed = 50;
	public float zSpeed = 15;

	public float x = 45;
	public float y = 45;

	public float lastX = 0;
	private Vector2 lastTouchPosition;

	// input controls
	public bool mouseControlsEnabled = true;
	public bool touchControlsEnabled = false;

	public bool rotating = false;

	// bigger numbers increase interpolation speed
	public float targetInterval = 5f;
	public float rotationInterval = 25f;

	#if !UNITY_EDITOR
		private float pinchLength = 0f;
		private float deltaLength = 0f;
	#endif


	void Start () {
		//angle increments
		x = angle.y;
		y = angle.x;

		lastX = x;
	}


	public void SetTarget (Transform target) {
		if (!target) target = new GameObject().transform;
		this.target = target;
	}


	void setDistance (float mouseZ) {
		float distanceTo = Mathf.Clamp(distance - mouseZ * zSpeed, distanceMin, distanceMax);
		distance = Mathf.Lerp(distance, distanceTo, Time.deltaTime * 10.0f);
	}


	void setRotation (float mouseX, float mouseY) {
		x += mouseX * xSpeed;
		y -= mouseY * ySpeed;
		y = ClampAngle(y, yMin, yMax);

		lastX = x;

		// update angle prop for debug purposes
		angle.x = y;
		angle.y = x;
	}


	void LateUpdate () {
		if (!target) { return; }

		// manage interactive control input
		setInputControls();

		float time = Time.deltaTime;
		float interval = rotating ? rotationInterval : targetInterval; //this.interval;

		//get new pos and rot from increments
		Quaternion rotation = Quaternion.Euler(y, x, 0);

		Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position + center; 

		if (interval != 0) {
			transform.position = Vector3.Slerp(transform.position, position, time * interval);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, time * interval);
		} else {
			transform.position = position;
			transform.rotation = rotation;
		}
	}


	private float ClampAngle(float angle , float min , float max ) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}


	private void setInputControls () {

		#if UNITY_EDITOR
			if (!mouseControlsEnabled) return;

			// -------------------------
			// control camera with mouse
			// -------------------------

			if (Input.GetButtonDown("Fire3")) { rotating = true; }
			if (Input.GetButtonUp("Fire3")) { rotating = false; }


			setDistance(Input.GetAxis("Mouse ScrollWheel"));
			

			if (rotating) {
				setRotation(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			} 

		#else
			if (!touchControlsEnabled) return;

			// --------------------------------------------------
			// control camera by pinching/rotating with 2 fingers
			// --------------------------------------------------

			if (Input.touchCount == 2) {
				rotating = true;

				// Zoom the camera while pinching with 2 fingers

				if (Input.GetTouch(1).phase == TouchPhase.Began) {
					pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				}

				if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved) {
					deltaLength = (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) - pinchLength) * 0.005f;
					pinchLength = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);

					// set camera distance
					setDistance(deltaLength);
				}
				
				// Rotate camera by moving with 2 fingers

				for (int i = 0; i < Input.touchCount; ++i) {
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began) { rotating = true; }
					if (touch.phase == TouchPhase.Ended) { rotating = false; }

					if (rotating) {
						setRotation(
							touch.deltaPosition.x * 0.02f,
							touch.deltaPosition.y * 0.02f
						);
					}
				}

			} else {
				rotating = false;
			}

		#endif
	}
}


