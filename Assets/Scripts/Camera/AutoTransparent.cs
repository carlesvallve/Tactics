using UnityEngine;
using System.Collections;


public class AutoTransparent : MonoBehaviour {
	private Shader m_OldShader = null;
	private Color m_OldColor = Color.black;
	private float m_Transparency = 1f;// 0.1f
	private const float m_TargetTransparency = 0.2f;
	private const float m_FallOff = 0.5f; // returns to 100% in m_FallOff seconds


	public void BeTransparent() {
		// reset the transparency;
		m_Transparency = m_TargetTransparency;
		if (m_OldShader == null) {
			// Save the current shader
			m_OldShader = GetComponent<Renderer>().material.shader;
			m_OldColor  = GetComponent<Renderer>().material.color;
			GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
		}
	}


	void Update() {
		if (m_Transparency < 1.0f) {
			Color C = GetComponent<Renderer>().material.color;
			C.a = m_Transparency;
			GetComponent<Renderer>().material.color = C;
		} else {
			// Reset the shader
			//GetComponent<Renderer>().material.shader = m_OldShader;
			//GetComponent<Renderer>().material.color = m_OldColor;
			// And remove this script
			//Destroy(this, m_FallOff);
		}

		m_Transparency += ((1.0f-m_TargetTransparency)*Time.deltaTime) / m_FallOff;
	}

}