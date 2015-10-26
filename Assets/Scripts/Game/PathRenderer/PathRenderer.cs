using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathRenderer : MonoBehaviour {

	public GameObject selectorPrefab;
	public GameObject dotPrefab;
	public GameObject linePrefab;
	
	public  GameObject selector { get; private set; }
	public List<PathDot> dots;
	public List<Cube> cubeShields = new List<Cube>();

	private Humanoid humanoid;
	private Color color;

	private List<LineRenderer> lines = new List<LineRenderer>();


	public void Init (Humanoid humanoid, Color color) {
		this.humanoid = humanoid;
		this.color = color;

		CreateSelector();
	}


	// =============================================
	// Selector
	// =============================================

	public void CreateSelector () {
		selector = (GameObject)Instantiate(selectorPrefab);
		selector.transform.SetParent(humanoid.transform);
		selector.transform.localPosition = Vector3.zero;
		selector.transform.localScale = new Vector3(1, 1, 1);

		SpriteRenderer sprite = selector.transform.Find("Sprite").GetComponent<SpriteRenderer>();
		sprite.material.SetColor("_OutlineColor", color);

		selector.SetActive(false);
	}


	public void DisplaySelector (bool value) {
		selector.SetActive(value);
	}


	// =============================================
	// Path
	// =============================================

	public void CreatePath (List<Vector2> path, int movement) {
		DestroyPath();

		movement -= 1;

		dots = new List<PathDot>();

		
		int goalNum = 0;
		Color goalColor = GameSettings.colors.grey;

		for (int i = 0; i < path.Count; i++) {
			// get pos
			Vector3 pos = new Vector3(path[i].x, 0.02f, path[i].y);

			// get color
			Color color = Color.grey;
			if (i <= movement) { color = GameSettings.colors.yellow; }
			if (i <= movement / 2) { color = this.color; } // GameSettings.colors.cyan

			// get scale
			float sc = ((i == path.Count - 1 && i <= movement) || i == movement) ? 0.1f : 0.05f;
			Vector3 scale = new Vector3(sc, sc, sc);

			// get goal
			if ((i == path.Count - 1 && i <= movement) || i == movement) {
				goalNum = i;
				goalColor = color;
			}

			// create path dot
			GameObject obj = (GameObject)Instantiate(dotPrefab);
			obj.transform.SetParent(transform, false);

			PathDot dot = obj.GetComponent<PathDot>();
			dot.Init(i, pos, scale, color);

			dots.Add(dot);
		}

		// set shields at goal dot
		Vector3 goalPos = dots[goalNum].transform.localPosition;
		SetShieldsAtPos(goalPos, goalColor);
	}


	public void DestroyPath () {
		if (dots == null) { return; }
		
		for (int i = 0; i < dots.Count; i++) { 
			DestroyDot(i); 
		}
		dots = null;

		ClearShields();
	}


	public void DestroyDot(int i) {
		if (dots[i] == null) { return; }

		Destroy(dots[i].gameObject);
	}


	// =============================================
	// Shields
	// =============================================

	public void ClearShields () {
		for (int i = 0; i < cubeShields.Count; i++) {
			cubeShields[i].ResetShields(); 
		}

		cubeShields.Clear();
	}


	public void SetShieldsAtPos (Vector3 pos, Color color) {
		//pos = new Vector3(pos.x, goalPos.y, pos.z);
		SetShieldInDirection(pos, new Vector3(0, 0, -1), color);
		SetShieldInDirection(pos, new Vector3(0, 0, 1), color);
		SetShieldInDirection(pos, new Vector3(1, 0, 0), color);
		SetShieldInDirection(pos, new Vector3(-1, 0, 0), color);
	}


	private void SetShieldInDirection(Vector3 pos, Vector3 dir, Color color) {
		Ray ray = new Ray(pos, dir);
		RaycastHit hit = new RaycastHit();

		if (Physics.Raycast(ray, out hit, 1f)) {
			if (hit.transform.gameObject.tag == "Cube") {
				Cube cube = hit.transform.parent.GetComponent<Cube>();
				if (cube != null) {
					cube.DisplayShield(dir, color);
					cubeShields.Add(cube);
				}
			}
		} 

		Debug.DrawLine(pos, pos + dir, Color.green, 2, false);
	}


	// =============================================
	// Debug Lines
	// =============================================

	public void ClearLines () {
		foreach (LineRenderer line in lines) {
			Destroy(line);
		}
		
		lines.Clear();
	}


	public void DrawLine (Vector3 startPos, Vector3 endPos, Color color, float width = 0.02f) {
		GameObject obj = (GameObject)Instantiate(linePrefab);
		obj.transform.SetParent(transform);
		obj.name = "Line";

		LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.SetColors(color, color);
        lineRenderer.SetWidth(width, width);
		lineRenderer.SetPosition(0, startPos);
		lineRenderer.SetPosition(1, endPos);

		lines.Add(lineRenderer);
	}
}
