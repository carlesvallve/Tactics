using UnityEngine;
using System.Collections;

public class PathDot : MonoBehaviour {

	private SpriteRenderer sprite;


	public void Init (int num, Vector3 pos, Vector3 scale, Color color) {
		sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		sprite.material.SetColor("_OutlineColor", color); 

		sprite.transform.localScale = scale;

		SetPosition(pos);
	}


	private void SetPosition (Vector3 pos) {
		Vector3 startPoint = new Vector3(pos.x, 10, pos.z);
		Vector3 endPoint = new Vector3(pos.x, 0, pos.z);
		RaycastHit hit = Utilities.SetRay(startPoint, endPoint, 10);
		
		transform.localPosition = new Vector3(pos.x, hit.point.y, pos.z);
	}
}
