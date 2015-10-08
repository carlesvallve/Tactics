using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {


	public void Init(Color color, Vector3 normal) {
		name = "Explosion";
		transform.parent = GameObject.Find("Fx").transform; //World.instancesContainer; 

		// initialize particles component
		ParticleEmitter particles  = gameObject.GetComponent<ParticleEmitter>();
		particles.worldVelocity = normal * 0.5f; // new Vector3(0, 0, 0);
		particles.Emit();

		//tint particles of given color
		ParticleAnimator particleAnimator = gameObject.GetComponent<ParticleAnimator>();
		Color[] m_Color = particleAnimator.colorAnimation;
		m_Color[4] = color; m_Color[3] = color; m_Color[2] = color; m_Color[1] = color; m_Color[0] = color;
		particleAnimator.colorAnimation = m_Color;

		// note: 
		// this prefab dont has to be destroyed manually,
		// becasue autodestruct is enabled in the particle animator
	}
}
