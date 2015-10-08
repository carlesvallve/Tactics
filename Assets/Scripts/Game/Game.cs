using UnityEngine;
using System.Collections;

[System.Serializable]
public class Containers {
	public GameObject fx;
}

public class Game : MonoBehaviour {
	
	

	public Containers containers = new Containers();

	private MapGenerator map;
	private Entity player;

	
	void Awake () {
		player = GameObject.Find("Player").GetComponent<Entity>();
		InitMap();
		InitGrid();
	}

	void Update () {
		UpdateControls();
	}

	private void InitMap () {
		map = GetComponent<MapGenerator>();
		map.Generate();
	}

	private void InitGrid () {
        Grid.InitEmpty(map.width, map.height);
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
		// check colliders in all layers
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(ray, out hit, 1000)) {}
		return hit;
	}


	private void TapOnGrid (RaycastHit hit) {
		Vector3 pos = new Vector3(Mathf.Round(hit.point.x), hit.point.y, Mathf.Round(hit.point.z));
		if (pos.x < 0 || pos.z < 0 || pos.x > Grid.xsize - 1 || pos.z > Grid.ysize - 1) { return; }

		player.SetPath(pos);
	}
}
