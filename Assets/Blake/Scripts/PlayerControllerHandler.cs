using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerHandler : MonoBehaviour {
	bool inSpecialMovement = false;
	GameObject specialMovementTrigger;
	GameObject	mainCamera;
	CameraController cameraController;

	// Use this for initialization
	void Start () {
		GetComponent<DefaultController>().enabled = true;
		mainCamera = GameObject.Find("CameraObject");
		cameraController = mainCamera.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
		var inputInteract = Input.GetButton("Interact");

		if(!inSpecialMovement && inputInteract && specialMovementTrigger != null){
			this.GetComponent<DefaultController>().enabled = false;

			switch(specialMovementTrigger.GetComponent<SpecialMovementTriggers>().movementType){
				case "crawl":
					GetComponent<CrawlingController>().enabled = true;
					cameraController.ToggleFirstPerson();
				break;
				case "ladder":
					var lcc = GetComponent<LadderClimbingController>();
					lcc.ladder = specialMovementTrigger;
					lcc.enabled = true;
				break;
			}

			inSpecialMovement = true;
		}
	}

	public void ExitSpecialMovment(string movementType){
		if(inSpecialMovement){
			var con = GetComponent<CrawlingController>();
			con.enabled = false;

			var pc = GetComponent<DefaultController>();
			pc.enabled = true;

			if(!string.IsNullOrWhiteSpace(movementType) && string.Equals(movementType, "crawl")){
				cameraController.ToggleFirstPerson();
			}
			

			inSpecialMovement = false;
		}
	}

	void OnTriggerEnter(Collider col){
		print(col.gameObject.name);

		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = col.gameObject;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = null;
		}
	}

	#region debugging
	void OnDrawGizmos() {
		// var rayDown =  -1 * Vector3.up;
		// var climbCollider = climbHook.GetComponent<MeshFilter>(); 

         Gizmos.color = Color.red;
         //Gizmos.DrawRay(climbHook.transform.position, rayDown);
		// Gizmos.DrawRay(climbHook.transform.position + (climbCollider.sharedMesh.bounds.size.x / 40f) * Vector3.left, rayDown);
		// Gizmos.DrawRay(climbHook.transform.position + (climbCollider.sharedMesh.bounds.size.x / 40f) * Vector3.right, rayDown);

		if(specialMovementTrigger != null)
			Gizmos.DrawRay(specialMovementTrigger.transform.position, specialMovementTrigger.transform.forward * 5);

		//print ("Gizmos");
    }

	#endregion
}
