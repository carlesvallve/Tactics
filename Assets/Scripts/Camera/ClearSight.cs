using UnityEngine;
using System.Collections;


public class ClearSight : MonoBehaviour {
	public LayerMask layerMask;


	void Start () {
		Transform mapContainer = GameObject.Find("3ds file").transform;
		Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer renderer in renderers) {
			Material[] materialsList = renderer.materials;
			for (int i = 0; i < materialsList.Length; i++) {
				Material material = materialsList[i];
				material.shader = Shader.Find("Custom/Overdraw");
			}
		}
	}

	/*void Update() {

		Transform mapContainer = GameObject.Find("3ds file").transform;
		Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers) {
			renderer.enabled = true;
		}

		Player player = Game.instance.currentSquad.currentPlayer;
		Vector3 startPos = Camera.main.transform.localPosition;
		Vector3 endPos = player.transform.localPosition + Vector3.up * 0.5f;
		Vector3 dir =  endPos - startPos;

		RaycastHit[] hits;
		// you can also use CapsuleCastAll()
		// TODO: setup your layermask it improve performance and filter your hits.
		hits = Physics.RaycastAll(transform.position, dir.normalized, dir.magnitude, layerMask);
		foreach(RaycastHit hit in hits) {
			//if (hit.transform == null) { continue; }

			//if (hit.transform.tag == "Map") {
			Renderer R = hit.collider.GetComponent<Renderer>();
			if (R == null) {
				continue; // no renderer attached? go to next hit
			}
			// TODO: maybe implement here a check for GOs that should not be affected like the player
			if (hit.transform.tag == "Player") {
				break;
			}
			//R.enabled = false;
			//}
		
			
			 AutoTransparent AT = R.GetComponent<AutoTransparent>();
			 if (AT == null) { // if no script is attached, attach one
				 AT = R.gameObject.AddComponent<AutoTransparent>();
			 }

			 AT.BeTransparent(); // get called every frame to reset the falloff

		 }
	 }*/


	 /*void Update () {
	 	Transform mapContainer = GameObject.Find("3ds file").transform;

		Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in renderers) {

			Material[] materialsList = renderer.materials;
			for (int i = 0; i < materialsList.Length; i++) {
				Material material = materialsList[i];
				material.shader = Shader.Find("Custom/Overdraw");
			}
		}
	 }*/
 }

