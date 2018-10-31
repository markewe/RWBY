using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	// Use this for initialization
	bool toggleCamera;

	void Start () {
			this.GetComponent<ObjectFollowObject>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Hook")){
			// this.GetComponent<ObjectFollowObject>().enabled = !this.GetComponent<ObjectFollowObject>().enabled;
			// this.GetComponent<AimingCamera>().enabled = !this.GetComponent<AimingCamera>().enabled;
			//ToggleFirstPerson();
		}
	}

	public void ToggleFirstPerson(){
		this.GetComponent<ObjectFollowObject>().enabled = !this.GetComponent<ObjectFollowObject>().enabled;
		this.GetComponent<FirstPersonCamera>().enabled = !this.GetComponent<FirstPersonCamera>().enabled;
	}

	//void EnableCamera()
}
