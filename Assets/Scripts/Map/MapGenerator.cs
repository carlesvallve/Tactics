using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {
	
	public static MapGenerator instance;

	public MapPrefabs prefabs = new MapPrefabs();

	public int width = 14;
	public int height = 14;
	public int maxCubes = 1;
	
	private Transform mapContainer;
	private List<GameObject> cubes;


	void Awake () {
		instance = this;
	}


	public void Generate () {
		mapContainer = Game.instance.containers.map;

		SetSize();
		GenerateRandomCubes();
	}


	private void SetSize () {
		Transform floor = mapContainer.Find("Floor");
		floor.localScale = new Vector3(width + 1, 0.2f, height + 1);
		floor.localPosition = new Vector3(
			-1 + floor.localScale.x / 2, 
			-floor.localScale.y / 2, 
			-1 + floor.localScale.z / 2 
		);

		Vector2 vec = new Vector2(floor.localScale.x / 3f, floor.localScale.z / 3f);
		Material material = floor.GetComponent<Renderer>().material;
		material.SetTextureScale("_MainTex", vec);
		material.SetTextureScale("_BumpMap", vec);	
	}
	
	
	private void GenerateRandomCubes () {
		for (int i = 0; i < maxCubes; i++) {
			GameObject cube = (GameObject)Instantiate(prefabs.cube);
			cube.transform.SetParent(mapContainer);
			cube.name = "Cube";

			Vector3 pos = new Vector3(Random.Range(0, width), 0, Random.Range(0, height));
			RaycastHit hit = Utilities.SetRay(pos + Vector3.up * 10, pos, 10);
			cube.transform.localPosition = hit.point;
		}
	}
}
