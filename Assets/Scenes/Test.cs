using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	private Animation anim;

	void Start () {
		anim = GetComponent<Animation>();

		foreach (AnimationState state in anim) {
            print (state.name); //state.speed = 0.5F;
        }

        
	}
	

	void Update () {
		if (Input.GetKeyDown(KeyCode.I)) { anim.CrossFade("soldierIdle", 0.2f); }
		if (Input.GetKeyDown(KeyCode.X)) { anim.CrossFade("soldierIdleRelaxed", 0.2f); }
		if (Input.GetKeyDown(KeyCode.W)) { anim.CrossFade("soldierWalk", 0.2f); }
		if (Input.GetKeyDown(KeyCode.R)) { anim.CrossFade("soldierRun", 0.2f); }
		if (Input.GetKeyDown(KeyCode.S)) { anim.CrossFade("soldierSprint", 0.2f); }
	}
}
