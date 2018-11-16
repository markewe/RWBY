using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerHandler : MonoBehaviour {

	
	bool inSpecialMovement = false;
	bool inTakedownRange = false;
	Animator animator;
	GameObject specialMovementTrigger;
	GameObject	mainCamera;
	CameraController cameraController;

	// Use this for initialization
	void Start () {
		GetComponent<DefaultController>().enabled = true;
		animator = GetComponent<Animator>();
		mainCamera = GameObject.Find("CameraObject");
		cameraController = mainCamera.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
		var inputInteract = Input.GetButton("Interact");
		//print(inputInteract);

		if(!inSpecialMovement && inputInteract && inTakedownRange){
			print("Interacting");
			GetComponent<DefaultController>().enabled = false;

			var tdc = GetComponent<TakedownController>();
			tdc.enemy = specialMovementTrigger;
			tdc.enabled = true;

			inSpecialMovement = true;
		}
		else if(!inSpecialMovement && inputInteract && specialMovementTrigger != null){
			GetComponent<DefaultController>().enabled = false;
			//print("Interacting");

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
				case "pickup":
					var hoc = GetComponent<HoldingObjectController>();
					hoc.pickupObject = specialMovementTrigger.transform.parent.gameObject;
					hoc.enabled = true;
				break;
			}

			inSpecialMovement = true;
		}
	}

	public void ExitSpecialMovment(string movementType){
		if(inSpecialMovement && !string.IsNullOrWhiteSpace(movementType)){

			switch(movementType){
				case "crawl":
					GetComponent<CrawlingController>().enabled = false;
					cameraController.ToggleFirstPerson();
				break;
				case "ladder":
					GetComponent<LadderClimbingController>().enabled = false;
				break;
				case "pickup":
					GetComponent<HoldingObjectController>().enabled = false;
				break;
				case "takedown":
					GetComponent<TakedownController>().enabled = false;
				break;
			}

			GetComponent<DefaultController>().enabled = true;
			inSpecialMovement = false;
			inTakedownRange = false;
		}
	}

	void OnTriggerEnter(Collider col){
		//print(col.gameObject.name + " Enter");

		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = col.gameObject;
		}
	}

	void OnTriggerExit(Collider col){
		//print(col.gameObject.name + " Exit");

		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = null;
		}
	}

	public void OnTakedownTriggerEnter(GameObject enemy){
		specialMovementTrigger = enemy;
		inTakedownRange = true;
	}

	public void OnTakedownTriggerExit(){
		specialMovementTrigger = null;
		inTakedownRange = false;
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
