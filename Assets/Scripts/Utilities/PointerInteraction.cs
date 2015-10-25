using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Diagnostics;


public static class PointerInteraction {

	private static int fingerId = 0;

	[Conditional("UNITY_EDITOR")]
	private static void SetFingerId() {
		fingerId = -1;
	}


	public static bool GetPointerHit (out RaycastHit hit, LayerMask layerMask) {
		hit = new RaycastHit();

		if (IsPointerOverGameObject()) { return false; }
	
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Magical number to be sure to hit the right gameobject (ask rob)
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
			return true;
		}

		return false;
	}


	public static bool IsPointerOverGameObject () {
		// escape if there is no EventSystem
		if (!EventSystem.current) {
			UnityEngine.Debug.Log("No event system was found!");
			return false;
		}

		SetFingerId();

		return EventSystem.current.IsPointerOverGameObject(fingerId);
	}

}