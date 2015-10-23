using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour {

	private GameObject shieldTop;
	private GameObject shieldBottom;
	private GameObject shieldLeft;
	private GameObject shieldRight;


	void Awake () {
		shieldTop = transform.Find("Shields/SpriteTop").gameObject;
		shieldBottom = transform.Find("Shields/SpriteBottom").gameObject;
		shieldLeft = transform.Find("Shields/SpriteLeft").gameObject;
		shieldRight = transform.Find("Shields/SpriteRight").gameObject;
		ResetShields();
	}


	public void ResetShields () {
		shieldTop.SetActive(false);
		shieldBottom.SetActive(false);
		shieldLeft.SetActive(false);
		shieldRight.SetActive(false);

		
	}


	public void DisplayShield(Vector3 dir, Color color) {
		if (dir == new Vector3(0, 0, -1)) { SetShieldColor(shieldTop, color); }
		if (dir == new Vector3(0, 0, 1)) { SetShieldColor(shieldBottom, color); }
		if (dir == new Vector3(1, 0, 0)) { SetShieldColor(shieldLeft, color); }
		if (dir == new Vector3(-1, 0, 0)) { SetShieldColor(shieldRight, color); }
	}


	private void SetShieldColor (GameObject shield, Color color) {
		color = new Color(color.r, color.g, color.b, 0.5f);
		SpriteRenderer sprite = shield.GetComponent<SpriteRenderer>();
		sprite.material.SetColor("_OutlineColor", color); 
		shield.SetActive(true);
	}
}
