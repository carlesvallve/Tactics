using UnityEngine;
using System.Collections;


public class ClearSight : MonoBehaviour {
	public LayerMask layerMask;


	/*void Start () {
		Transform mapContainer = GameObject.Find("3ds file").transform;
		Renderer[] renderers = mapContainer.GetComponentsInChildren<Renderer>();
		
		foreach (Renderer renderer in renderers) {
			Material[] materialsList = renderer.materials;
			for (int i = 0; i < materialsList.Length; i++) {
				Material material = materialsList[i];
				material.shader = Shader.Find("Custom/Overdraw");
			}
		}
	}*/

	void Update() {
		// get parameters from camera to player to execute the raycast
		Player player = Game.instance.currentSquad.currentPlayer;
		Vector3 startPos = Camera.main.transform.localPosition;
		Vector3 endPos = player.transform.localPosition + Vector3.up * 1f;
		Vector3 dir =  (endPos - startPos).normalized;
        float dist = Vector3.Distance(endPos, startPos);

        // execute raycast
		RaycastHit[] hits; // you can also use CapsuleCastAll()
		hits = Physics.RaycastAll(transform.position, dir, dist, layerMask);

		foreach(RaycastHit hit in hits) {

			Renderer R = hit.collider.GetComponent<Renderer>();
			if (R == null) { continue; } // escape if hit has no renderer
			if (hit.transform.tag == "Player") { break; } // escape if we arrived to the camera target
			
			// get autotransparent component and execute it
			// if no script is attached, attach one
			AutoTransparent AT = R.GetComponent<AutoTransparent>();
			if (AT == null) { AT = R.gameObject.AddComponent<AutoTransparent>(); } 
			AT.BeTransparent(); // get called every frame to reset the falloff
		 }
	 }


 
    /*public Transform WatchTarget;
    public LayerMask OccluderMask;
    //This is the material with the Transparent/Diffuse With Shadow shader
    //public Material HiderMaterial;
 
    private Dictionary<Transform, Material> _LastTransforms;
 
    void Start () {
        _LastTransforms = new Dictionary<Transform, Material>();
    }
 
    void Update () {
        //reset and clear all the previous objects
        if(_LastTransforms.Count > 0){
            foreach(Transform t in _LastTransforms.Keys){
                t.renderer.material = _LastTransforms[t];
            }
            _LastTransforms.Clear();
        }
 
        //Cast a ray from this object's transform the the watch target's transform.
        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            WatchTarget.transform.position - transform.position,
            Vector3.Distance(WatchTarget.transform.position, transform.position),
            OccluderMask
        );
 
        //Loop through all overlapping objects and disable their mesh renderer
        if(hits.Length > 0){
            foreach(RaycastHit hit in hits){
                if(hit.collider.gameObject.transform != WatchTarget && hit.collider.transform.root != WatchTarget){
                    _LastTransforms.Add(hit.collider.gameObject.transform, hit.collider.gameObject.renderer.material);
                    hit.collider.gameObject.renderer.material = HiderMaterial;
                }
            }
        }
    }*/

	 
 }

