using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MapPrefabs {
	public GameObject cube;
}

public class MapGenerator : MonoBehaviour {
	public int width = 14;
	public int height = 14;
	public MapPrefabs mapPrefabs = new MapPrefabs();
	private GameObject floor;
	private List<GameObject> cubes;


	public void Generate () {
		SetSize();
	}

	private void SetSize () {
		floor = GameObject.Find("Floor");
		floor.transform.localScale = new Vector3(width + 1, 0.2f, height + 1);
		floor.transform.localPosition = new Vector3(
			-1 + floor.transform.localScale.x / 2, 
			-floor.transform.localScale.y / 2, 
			-1 + floor.transform.localScale.z / 2 
		);

		Vector2 vec = new Vector2(floor.transform.localScale.x / 3f, floor.transform.localScale.z / 3f);
		Material material = floor.GetComponent<Renderer>().material;
		material.SetTextureScale("_MainTex", vec);
		material.SetTextureScale("_BumpMap", vec);	
	}
	
	
	private void GenerateRandomCubes () {
		
	}
}
