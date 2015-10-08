using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	private GameObject selector;

	public int mapWidth = 14;
	public int mapHeight = 14;

	
	void Awake () {
		selector = GameObject.Find("Selector");
		selector.SetActive(false);

		InitGrid();
	}

	void Update () {
		UpdateControls();
	}


	private void InitGrid () {
		// init grid cells for astar calculations
        Grid.InitEmpty(mapWidth, mapHeight);
        print ("Grid initialized: " + Grid.xsize + "," + Grid.ysize);
	}


	private void UpdateControls () {

		if (Input.GetButtonDown("Fire1")) {
			RaycastHit hit = GetHit(Input.mousePosition);
			if (!hit.transform) { return; }

			string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
			switch (layerName) {

			case "Grid":
				TapOnGrid(hit);
				break;

			}
		}
	}


	private RaycastHit GetHit(Vector3 pos) {
		// check collisions with colliders in all layers
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1000)) {}
		return hit;
	}


	private void TapOnGrid (RaycastHit hit) {
		Vector3 pos = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z));
		if (pos.x < 0 || pos.z < 0 || pos.x > Grid.xsize - 1 || pos.z > Grid.ysize - 1) { return; }

		print (pos);
		
		selector.transform.position = pos + Vector3.up * 0.01f;
		selector.SetActive(true);
	}
}
