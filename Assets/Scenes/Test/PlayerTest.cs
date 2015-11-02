using UnityEngine;
using System.Collections;

public class PlayerTest : MonoBehaviour {

	private NavMeshAgent agent;

	void Start () {
		agent = GetComponent<NavMeshAgent>(); 
	}
	
	
	void Update () {

		if (Input.GetMouseButtonDown(0)) {
        	Ray screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

	        RaycastHit hit;
	        if (Physics.Raycast(screenRay, out hit)) {
	        	Vector3 goal = hit.point;
	            agent.SetDestination(goal);
	            print ("setting agent destination: " + goal);
	        }
    	}
	}
}
