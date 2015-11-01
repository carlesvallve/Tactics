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
		Vector3 startPoint = new Vector3(pos.x, pos.y + 0.3f, pos.z);
		Vector3 endPoint = new Vector3(pos.x, pos.y, pos.z);
		RaycastHit hit = Utilities.SetRay(startPoint, endPoint, 0.3f);
		
		transform.localPosition = new Vector3(pos.x, hit.point.y, pos.z);
	}
}
