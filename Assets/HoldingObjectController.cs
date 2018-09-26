using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingObjectController : APlayerController {

		public bool toggleRun = true;
	float inputX;
	float inputZ;

	bool isClimbingLedge = false;
	bool isCrouching = false;
	bool isRunning = true;
	bool isJumping = false;
	bool isAttacking = false;
	bool isHanging = false;
	bool isDodging = false;
	bool missedComboWindow = false;
	float jumpHeight = 4f;
	float walkSpeed = 2f;
	float runSpeed = 8f;
	float jumpSpeed = 1f;
	float speedSmoothTime = 0.1f;
	float jumpSmoothTime = 2f;
	float currentSpeed;
	float speedSmoothVelocity;
	float turnSmoothTime = 10f;
	float velY;
	int comboCounter = 0;
	Vector3 targetDirection;
	Vector3 dodgeDirection;
	float dodgeRotation;
	static float gravity = -9.8f;
	public GameObject playerWeapon;
	public GameObject climbHook;
	Transform hangingLedge;

	#region APlayerController functions

	public override void HandleInputs(){
		inputX = Input.GetAxis("Horizontal");
		inputZ = Input.GetAxis("Vertical");

		targetDirection = new Vector3(inputX, 0f, inputZ);
	}

	public override void MovePlayer(){
		// determine how fast to move
		var targetSpeed = walkSpeed * targetDirection.magnitude;
		var smoothTime = speedSmoothTime;
		
		// move
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
		Vector3 vel = transform.forward * currentSpeed;
		controller.Move(vel * Time.deltaTime);
	}

	public override void RotatePlayer(){
		// turn slowly based on camera's forward
		var targetRotation = transform.eulerAngles.y;
			
		if(targetDirection != Vector3.zero){
			targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
		}

		Quaternion target = Quaternion.Euler(Vector3.up * targetRotation);
		transform.rotation = !isAttacking ? Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * turnSmoothTime) : target;
	}

	public override void SetAnimations(){
		// play animations
		animator.SetFloat("HSpeed", currentSpeed);
		animator.SetBool("IsJumping", isJumping);
		animator.SetFloat("VSpeed", velY);
		animator.SetBool("IsAttacking", isAttacking);
		animator.SetInteger("ComboCounter", comboCounter);
		animator.SetBool("IsCrouching", isCrouching);
		animator.SetBool("IsHanging", isHanging);
		animator.SetFloat("InputX", inputX);
		animator.SetFloat("InputZ", inputZ);
		animator.SetBool("IsDodging", isDodging);
	}

	public override void SetHitbox(){
		controller.height = 1.7f;
		controller.center = new Vector3(0f,  1.7f / 2f, 0f);
	}

	#endregion

	#region debugging
	
	#endregion

}
