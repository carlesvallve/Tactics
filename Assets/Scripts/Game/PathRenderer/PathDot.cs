using UnityEngine;
using System.Collections;

public class PathDot : MonoBehaviour {

	private SpriteRenderer sprite;


	public void Init (int num, Vector3 pos, Vector3 scale, Color color) {
		sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		sprite.color = color;
		sprite.transform.localScale = scale; //new Vector3(sc, sc, sc);
		transform.localPosition = pos;
	}
}
