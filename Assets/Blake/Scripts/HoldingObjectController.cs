using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingObjectController : APlayerController {

	public GameObject pickupObject;
	public bool toggleRun = true;
	float inputX;
	float inputZ;
	bool inPickup;
	bool inThrow;
	bool inSetDown;
	bool isAttacking = false;
	bool inputInteract = false;
	float walkSpeed = 2f;
	float speedSmoothTime = 0.1f;
	float currentSpeed;
	float speedSmoothVelocity;
	float turnSmoothTime = 10f;
	Vector3 targetDirection;

	#region APlayerController functions

	public override void OnEnable(){
		base.OnEnable();
		inPickup = true;
		animator.SetBool("ObjectPickup", true);
	}

	public override void Update(){
		base.Update();
	}

	public override void HandleInputs(){
		inputInteract = inPickup ? false : Input.GetButton("Interact");
		inputX = Input.GetAxis("Horizontal");
		inputZ = Input.GetAxis("Vertical");

		targetDirection = new Vector3(inputX, 0f, inputZ);
 		
		if(inputInteract && !inSetDown && ((inputX > 0.5) || (inputZ > 0.5))){
			inThrow = true;
		}
		else if(inputInteract && !inThrow && ((inputX < 0.5) && (inputZ < 0.5))){
			inSetDown = true;
		}
	}

	public override void MovePlayer(){
		// determine how fast to move
		var targetSpeed = walkSpeed * targetDirection.magnitude;
		var smoothTime = speedSmoothTime;
		
		// move
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
		Vector3 vel = transform.forward * currentSpeed;

		if(!inThrow && !inSetDown && !inPickup){
			controller.Move(vel * Time.deltaTime);
		}
	}

	public override void RotatePlayer(){
		// turn slowly based on camera's forward
		var targetDegrees = transform.eulerAngles.y;
			
		if(targetDirection != Vector3.zero){
			targetDegrees = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
		}

		var targetRotation = Quaternion.Euler(Vector3.up * targetDegrees);

		if(!inThrow && !inSetDown && !inPickup){
			transform.rotation = !isAttacking 
				? Quaternion.Slerp(transform.rotation, targetRotation,  Time.deltaTime * turnSmoothTime) : targetRotation;
		}
	}

	public override void SetAnimations(){
		//print(inputInteract);

		// play animations
		animator.SetFloat("HSpeed", currentSpeed);
		animator.SetBool("ObjectThrow", inThrow);
		animator.SetBool("ObjectSetDown", inSetDown);
	}

	public override void PostEvents(){}

	public override void SetHitbox(){
		controller.height = 1.7f;
		controller.center = new Vector3(0f,  1.7f / 2f, 0f);
	}

	#endregion

	#region pick up object functions

	void InitPickupObject(){
		pickupObject.GetComponent<Rigidbody>().detectCollisions = false;
		//pickupObject.transform.parent = GameObject.Find("Blake").transform;
		pickupObject.transform.parent = GameObject.Find("Blake/Armature/root ground/root hips/spine root/spine/arm right shoulder 1/arm right shoulder 2/arm right elbow/arm right wrist/pickup holder").transform;
		pickupObject.GetComponent<Rigidbody>().useGravity = false;
		pickupObject.transform.localPosition = new Vector3(0.004f, 0.001f, -0.003f);//Vector3.zero;
		pickupObject.transform.rotation = Quaternion.identity;
	}

	void ResetPickupObject(){
		pickupObject.transform.parent = null;
		pickupObject.GetComponent<Rigidbody>().detectCollisions = true;
		pickupObject.GetComponent<Rigidbody>().useGravity = true;
		print("ResetPickupObject");
	}

	#endregion

	#region Animation Events

	public void ObjectPickupBegin(){
	}

	public void ObjectPickupEnd(){
		InitPickupObject();
		inPickup = false;
		animator.SetBool("ObjectPickup", false);
	}

	public void ObjectSetDownBegin(){
		//ResetPickupObject();
	}

	public void ObjectSetDownEnd(){
		inSetDown = false;
		animator.SetBool("ObjectSetDown", false);
		inSetDown = false;
		//ResetPickupObject();
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("pickup");
	}

	public void ObjectSetDownLetGo(){
		ResetPickupObject();
	}

	public void ObjectThrowBegin(){
		print("throwbegin");
		animator.applyRootMotion = true;
	}

	public void ObjectThrowEnd(){
		animator.applyRootMotion = false;
		inThrow = false;
		animator.SetBool("ObjectThrow", false);
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("pickup");
	}

	public void ObjectThrowLetGo(){
		pickupObject.transform.parent = null;
		var pickupObjectRB = pickupObject.GetComponent<Rigidbody>();
		pickupObjectRB.velocity += (controller.velocity.normalized * 5f) + (Vector3.up * 5f);
		pickupObjectRB.useGravity = true;
		pickupObjectRB.detectCollisions = true;
	}

	#endregion

	#region debugging
	
	#endregion

}
