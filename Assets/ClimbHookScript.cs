using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbHookScript : MonoBehaviour {
	public bool isColliding = false;
	void OnTriggerEnter(Collider col){
		//print("ClimbHookCollideEnter");
		isColliding = true;
	}
	void OnTriggerExit(Collider col){
		//print("ClimbHookCollideExit");
		isColliding = false;
	}

}
