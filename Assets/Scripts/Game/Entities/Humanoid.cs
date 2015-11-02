using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Humanoid : Entity {

	public int movement = 4;
	public float speed = 0.2f; // duration for walking one step
	public GameObject pathPrefab;

	public PathRenderer pathRenderer { get; private set; }
	protected List<Vector2> path = new List<Vector2>();

	public bool moving { get; private set; }

	public GameObject body { get; private set; }
	//protected Material material;
	protected Renderer[] renderers;

	protected Animation anim;

	public Squad squad { get; private set; }
	public int num { get; private set; }
	public Color color { get; private set; }

	public float visionRange = 7;

	// events
	public delegate void OnVisionUpdatedHandler ();
	public event OnVisionUpdatedHandler OnVisionUpdated;


	public void Init (Squad squad, int num, Vector3 pos, Color color) {
		body = transform.Find("soldierNoScript").gameObject;
		anim = body.GetComponent<Animation>();
		//material = body.GetComponent<Renderer>().material;
		renderers = body.GetComponentsInChildren<Renderer>();

		this.squad = squad;
		this.num = num;
		this.color = color;

		transform.localPosition = pos;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, false);

		CreatePathRenderer();
		SetBodyOutline();

		path = new List<Vector2>();
		pathRenderer.SetShieldsAtPos(transform.localPosition, color);
		SetCover();
	}


	protected void SetBodyOutline () {
		float outlineWidth = 0.0002f;
		foreach (Renderer renderer in renderers) {
			Material material = renderer.material;
			if (material.HasProperty("_Outline")) { material.SetFloat("_Outline", outlineWidth); }
			if (material.HasProperty("_OutlineColor")) { material.SetColor("_OutlineColor", color); } 
		}
	}

	// =============================================
	// Selection
	// =============================================

	public void Deselect () {
		path.Clear();
		pathRenderer.DestroyPath();
		pathRenderer.DisplaySelector(false);
		pathRenderer.ClearLines();
		pathRenderer.ClearShields();
	}


	public void Select () {
		pathRenderer.DisplaySelector(true);

		if (OnVisionUpdated != null) {
			OnVisionUpdated.Invoke();
		}

		if (GameCamera.instance.mode == CameraMode.Aiming) {
			Hud.instance.SelectNextEnemyIcon();
		}
	}


	// =============================================
	// Path
	// =============================================

	protected void CreatePathRenderer () {
		GameObject obj = (GameObject)Instantiate(pathPrefab);
		obj.transform.SetParent(Game.instance.containers.fx);
		obj.name = "Path " + name;
		pathRenderer  = obj.GetComponent<PathRenderer>();
		pathRenderer.Init(this, color);
	}


	public void SetPath (Vector3 pos) {
		if (moving) { return; }

		if (path.Count > 0) {
			Vector3 goal = new Vector3(path[path.Count -1].x, 0, path[path.Count -1].y);
			if (pos == goal) {
				FollowPath();
				return;
			}
		}

		path = Grid.astar.SearchPath(
			(int)transform.localPosition.x, 
			(int)transform.localPosition.z,
			(int)pos.x, 
			(int)pos.z
		);

		if (path.Count == 0) { 
			pathRenderer.DestroyPath();
			return;
		}

		path.RemoveAt(0);
		pathRenderer.CreatePath(path, movement);
	}


	// =============================================
	// Move
	// =============================================

	public void FollowPath () {
		if (path.Count == 0) { return; }
		StartCoroutine(FollowPathAnim());
	}


	protected IEnumerator FollowPathAnim () {
		// abandon cover
		//yield return StartCoroutine(MoveToCover(Vector3.zero));

		anim.CrossFade("soldierSprint", 0.2f);

		moving = true;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, true);

		int step = 0;

		while (path.Count > 0) {
			Vector3 point = pathRenderer.dots[step].transform.localPosition; 
			yield return StartCoroutine(MoveToPos(point, speed));

			path.RemoveAt(0);
			pathRenderer.DestroyDot(step);
			step += 1;

			if (step == movement) {
				break;
			}
		}

		moving = false;
		Grid.SetWalkable(transform.localPosition.x, transform.localPosition.z, false);

		anim.CrossFade("soldierIdleRelaxed", 0.2f);

		// check for cover and move body towards it
		//SetCover ();
	}


	protected IEnumerator MoveToPos(Vector3 endPos, float duration) {
		// look at next pos direction
		StartCoroutine(TurnToLookAt( new Vector3(endPos.x, transform.localPosition.y, endPos.z), 0.1f));

		Vector3 startPos = transform.localPosition;
		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			transform.localPosition = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
			GameCamera.instance.offset *= 0.95f;
			yield return null;
		}

		transform.localPosition = endPos;

		if (OnVisionUpdated != null) {
			OnVisionUpdated.Invoke();
		}
	}


	protected IEnumerator TurnToLookAt(Vector3 lookAtPos, float duration) {
		float t = 0;

		Vector3 direction = lookAtPos - transform.localPosition;
		Quaternion startRot = body.transform.rotation;
		Quaternion endRot = Quaternion.LookRotation(direction);

		while(t < 1) {
			body.transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

			t += Time.deltaTime / duration;
			yield return null;
		}

		body.transform.localRotation = endRot;
	}


	// =============================================
	// Cover
	// =============================================

	protected void SetCover () {
		if (body.transform.localPosition != new Vector3(0, body.transform.localPosition.y, 0)) {
			return;
		}

		Vector3 vec = Vector3.zero;
		float d = 0.25f;

		for (int i = 0; i < pathRenderer.cubeShields.Count; i++) {
			Cube cube = pathRenderer.cubeShields[i];

			if (cube.shieldTop.activeSelf) {
				vec += new Vector3(0, 0, -d);
			}

			if (cube.shieldBottom.activeSelf) {
				vec += new Vector3(0, 0, d);
			}

			if (cube.shieldLeft.activeSelf) {
				vec += new Vector3(d, 0, 0);
			}

			if (cube.shieldRight.activeSelf) {
				vec += new Vector3(-d, 0, 0);
			}
		}

		pathRenderer.ClearShields();
		StartCoroutine(MoveToCover(vec));
	}


	private IEnumerator MoveToCover (Vector3 vec, float duration = 0.1f) {
		// get start and end positions
		Vector3 startPos = body.transform.localPosition;
		Vector3 endPos = new Vector3(
			vec.x, 
			body.transform.localPosition.y, 
			vec.z
		);

		// escape if we are aleady at end position
		if (body.transform.localPosition == endPos) {
			yield break;
		}

		moving = true;

		float startTime = Time.time;

		while(Time.time < startTime + duration) {
			body.transform.localPosition = Vector3.Lerp(startPos, endPos, (Time.time - startTime) / duration);
			pathRenderer.selector.transform.localPosition = new Vector3(body.transform.localPosition.x, pathRenderer.selector.transform.localPosition.y, body.transform.localPosition.z);
			yield return null;
		}

		body.transform.localPosition = endPos;
		pathRenderer.selector.transform.localPosition = new Vector3(body.transform.localPosition.x, pathRenderer.selector.transform.localPosition.y, body.transform.localPosition.z);
	
		moving = false;
	}


	// =============================================
	// Aim
	// =============================================

	public void StartAiming (Player target) {
		/*
		- click on a visible enemy or enemy hud icon -> OK
		- move player body to a position where he has a clean line of vision
		- interpolate camera into aiming mode (player -> enemy)
		- display aiming hud over enemy (aim circle, cover level, aim percent)
		- click next icon or press tab to switch between targets
		*/

		// get positions
		Vector3 lookFromPos = transform.localPosition + Vector3.up * 0.5f;
		Vector3 lookAtPos = target.transform.localPosition + Vector3.up * 0.5f;

		anim.CrossFade("soldierIdle", 0.2f);

		// turn player towards target
		StartCoroutine(TurnToLookAt( new Vector3(lookAtPos.x, transform.localPosition.y, lookAtPos.z), 0.25f));

		// set camera to aiming mode
		GameCamera.instance.SetAimingMode(lookFromPos, lookAtPos);
	}


	public void StopAiming () {
		anim.CrossFade("soldierIdleRelaxed", 0.2f);
		GameCamera.instance.SetNormalMode();
	}

}
