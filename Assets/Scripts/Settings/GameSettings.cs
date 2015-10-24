using UnityEngine;
using System.Collections;


public class GameColors {
	public Color grey = new Color(0.5f, 0.5f, 0.5f, 0.5f);
	public Color cyan = new Color(0, 1, 1, 0.5f);
	public Color yellow = new Color(1, 1, 0, 0.5f);
	public Color magenta = new Color(1, 0, 1, 0.5f);
	public Color red = new Color(1, 0, 0, 0.5f);
}


public class GameSettings : MonoBehaviour {

	public static GameColors colors = new GameColors();

}
