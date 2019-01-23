using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerHandler : MonoBehaviour, IHealthListener {
	bool inSpecialMovement = false;
	bool inTakedownRange = false;
	Animator animator;
	APlayerController currentController;
	GameObject specialMovementTrigger;
	GameObject	mainCamera;
	CameraController cameraController;

	// Use this for initialization
	void Start () {
		currentController = GetComponent<DefaultController>();
		currentController.enabled = true;
		animator = GetComponent<Animator>();
		mainCamera = GameObject.Find("CameraObject");
		cameraController = mainCamera.GetComponent<CameraController>();
	}
	
	// Update is called once per frame
	void Update () {
		var inputInteract = Input.GetButton("Interact");
		var specialAttack = Input.GetButton("SpecialAttack");

		if(currentController is DefaultController){
			if(!inSpecialMovement && inputInteract && inTakedownRange){
				currentController.enabled = false;

				var tdc = GetComponent<TakedownController>();
				tdc.enemy = specialMovementTrigger;
				tdc.enabled = true;

				inSpecialMovement = true;
				currentController = tdc;
			}
			else if(!inSpecialMovement && inputInteract && specialMovementTrigger != null){
				GetComponent<DefaultController>().enabled = false;

				switch(specialMovementTrigger.GetComponent<SpecialMovementTriggers>().movementType){
					case "crawl":
						var cc = GetComponent<CrawlingController>();
						cc.enabled = true;
						cameraController.ToggleFirstPerson();
						currentController = cc;
					break;
					case "ladder":
						var lcc = GetComponent<LadderClimbingController>();
						lcc.ladder = specialMovementTrigger;
						lcc.enabled = true;
						currentController = lcc;
					break;
					case "pickup":
						var hoc = GetComponent<HoldingObjectController>();
						hoc.pickupObject = specialMovementTrigger.transform.parent.gameObject;
						hoc.enabled = true;
						currentController = hoc;
					break;
				}

				inSpecialMovement = true;
			}
			else if(!inSpecialMovement && specialAttack){
				currentController.enabled = false;
				var sac = GetComponent<SpecialAttackController>();
				sac.enabled = true;
				currentController = sac;
				inSpecialMovement = true;
			}
		}
	}

	public void ExitSpecialMovment(string movementType){
		if(inSpecialMovement && !string.IsNullOrWhiteSpace(movementType)){
			switch(movementType){
				case "crawl":
					cameraController.ToggleFirstPerson();
					break;
			}

			currentController.enabled = false;
			currentController = GetComponent<DefaultController>();
			currentController.enabled = true;
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

	#region IHealthListener functions

	public void OnTakeDamage(){
		if(currentController is HoldingObjectController){
			(currentController as HoldingObjectController).DropObject();
		}
		else if(currentController is DefaultController){
			(currentController as DefaultController).OnTakeDamage();
		}
	}

	public void OnHealDamage(){}

	public void OnZeroHealth(){
		currentController.enabled = false;

		if(GetComponent<CharacterController>().isGrounded){
			animator.SetBool("IsKnockedOut", true);
		}
		else{
			animator.SetBool("IsKnockedOutAir", true);
		}
		
		//GameManager.instance.GameOver();
	}

	#endregion

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
