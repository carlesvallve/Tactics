using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour {

	// ===================================================================
	// Seeded Random
	// ===================================================================

	public static void SetRandomSeed (int seed) {
		if (seed > 0) { Random.seed = seed; }
	}


	public static void ResetRandomSeed () {
		Random.seed = (int)System.DateTime.Now.Ticks;
	}


	public static int IntParseFast(string value) {
		int result = 0;
		for (int i = 0; i < value.Length; i++) {
			char letter = value[i];
			result = 10 * result + (letter - 48);
		}
		return result;
	}
	
	// ===================================================================
	// RaycastHits, Positions and Orientations
	// ===================================================================

	// return a hit from a casted ray from start to end points in a maximum distance
	
	public static RaycastHit SetRay(Vector3 startPoint, Vector3 endPoint, float distance) {

		//get direction vector
		Vector3 direction = (endPoint - startPoint).normalized;
		
		//cast a ray from start to end points
		Ray ray  = new Ray(startPoint, direction); 
		RaycastHit hit = new RaycastHit();
		
		//check if we hitted something
		Color color;

		if (Physics.Raycast(ray, out hit, distance)) { //, colliderLayer
			color = Color.green;
			if (LayerMask.LayerToName(hit.transform.gameObject.layer) != "Avatar") {
				color = Color.red;
			} 
		} else {
			hit.point = ray.GetPoint(distance);
			hit.distance = distance;
			color = Color.red;
		}

		//Debug.DrawRay (startPoint, direction * hit.distance, color);
		Debug.DrawLine (startPoint, hit.point, color);
		
		// return the raycast hit object
		return hit;
	}


	public static Vector3 GetPositionOnLayer (string layerName, Vector3 pos) {
		return GetPositionOnHit(GetHitOnLayer (layerName, pos));
	}


	//	return a hit on a given layer, of a casted ray from position to an infinite downwards direction

	public static RaycastHit GetHitOnLayer (string layerName, Vector3 pos) {
		RaycastHit hit;
		int colliderLayerMask = 1 << LayerMask.NameToLayer(layerName);

		if (Physics.Raycast(pos + Vector3.up * 500f, Vector3.down, out hit, Mathf.Infinity, colliderLayerMask)) {
			Debug.DrawLine (pos, hit.point, Color.white);
		} 

		return hit;
	}


	public static Vector3 GetPositionOnHit (RaycastHit hit) {
		return hit.transform ? hit.point : Vector3.zero;
	}


	public static Quaternion GetRotationOnHit (RaycastHit hit, Vector3 fwd) { 
		Quaternion rot  = Quaternion.identity;

		if (hit.transform) {
			Vector3 proj  = fwd - (Vector3.Dot(fwd, hit.normal)) * hit.normal;
			rot = Quaternion.LookRotation(proj, hit.normal); 
		} 

		return rot;
	}

	
	public static float GetAngleDiff (Vector3 fwd, Quaternion rotationA, Quaternion rotationB) {
		// get a "forward vector" for each rotation
		Vector3 forwardA = rotationA * fwd;
		Vector3 forwardB = rotationB * fwd;
		// get a numeric angle for each vector, on the X-Z plane (relative to world forward)
		float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
		float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;
		// get the signed difference in these angles
		float angleDiff = Mathf.DeltaAngle( angleA, angleB );
		// return it
		return angleDiff;
	}


	public static float GetDistance2D(Vector3 pos1, Vector3 pos2) {
		return Vector3.Distance(new Vector3(pos1.x, 0, pos1.z), new Vector3(pos2.x, 0, pos2.z));
	}


	// ===================================================================
	// Finding gmaeObjects
	// ===================================================================

	public static Transform getChildByName(Transform fromTransform, string withName) {
		Transform[] ts = fromTransform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t;
		return null;
	}


	// ===================================================================
	// Special Effects
	// ===================================================================
	
	/*public static void Explode (Color color, Vector3 pos, Quaternion rot, Vector3 normal) {
		GameObject obj = (GameObject)Instantiate(Resources.Load("Fx/Explosion"), pos, rot);
		Explosion expl = obj.GetComponent<Explosion>();
		expl.Init(color, normal);
	}

	public static void Spark (Color color, Vector3 pos, Quaternion rot, Vector3 normal) {
		GameObject obj = (GameObject)Instantiate(Resources.Load("Fx/Spark"), pos, rot);
		Explosion expl = obj.GetComponent<Explosion>();
		expl.Init(color, normal);
	}*/
}
