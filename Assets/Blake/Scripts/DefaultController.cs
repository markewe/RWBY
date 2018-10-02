using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultController : APlayerController {

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
	float runSpeed = 6f;
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
		inputX = !isHanging ? Input.GetAxis("Horizontal") : 0f;
		inputZ = Input.GetAxis("Vertical");
		isCrouching = Input.GetButton("Crouch") & !isJumping;

		targetDirection = new Vector3(inputX, 0f, inputZ);

		if(inputZ < 0f && isHanging && !isClimbingLedge){
			isHanging = false;
		}

		if(Input.GetButtonDown("Attack1") && !isJumping){
			if(isAttacking && !missedComboWindow){
				comboCounter++;
			}
			else if(!isAttacking){
				isAttacking = true;
			}
		}

		if(Input.GetButtonDown("Jump") && (!isAttacking || isHanging || !isJumping || !isClimbingLedge)){
			velY = Mathf.Sqrt(-2f * gravity * jumpHeight);
			isHanging = false;
			isJumping = true;
			isAttacking = false;
		}

		if(Input.GetButtonDown("Dodge")) {
			isDodging = true;
			dodgeDirection = targetDirection;
			dodgeRotation = Mathf.Atan2(dodgeDirection.x, dodgeDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
		}

		if(!toggleRun){
			isRunning = Input.GetButton("Run");
		}
		else {
			if(Input.GetButtonDown("Run")) {
				isRunning = !isRunning;
			}
		}
	}

	public override void MovePlayer(){
		// determine how fast to move
		var targetSpeed = walkSpeed * targetDirection.magnitude;
		var smoothTime = speedSmoothTime;
		
		if(isJumping){
			smoothTime = jumpSmoothTime;
			targetSpeed = jumpSpeed * targetDirection.magnitude;
		}
		else if(isRunning && !isCrouching){
			targetSpeed = runSpeed * targetDirection.magnitude;
		}
		
		if(isHanging){
			smoothTime = 0f;
			targetSpeed = 0f;
		}
		
		// move
		// if(isDodging){
		// 	//controller.Move(transform.forward * (runSpeed * dodgeDirection.magnitude) * Time.deltaTime);
			
		// }
		// else
		if(!isAttacking && !isDodging){
			currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
			Vector3 vel = transform.forward * currentSpeed + Vector3.up * velY;
			controller.Move(vel * Time.deltaTime);
		}
		else{
			animator.applyRootMotion = isAttacking | isDodging;
		}

		if(!controller.isGrounded && (isHanging || CheckForLedges())){
			velY = 0f;
			isJumping = false;
			isHanging = true;
		}
		else if(!controller.isGrounded){
			velY += Time.deltaTime * gravity;
		}
		else if(controller.isGrounded){
			velY = 0f;
			isJumping = false;
		}

		print(controller.velocity);
	}

	public override void RotatePlayer(){
		// turn slowly based on camera's forward
		if(!isJumping && !isDodging){
			var targetRotation = transform.eulerAngles.y;
			
			if(isAttacking){
				//targetRotation = Camera.main.transform.eulerAngles.y;
			}
			else if(isHanging){
				targetRotation = TurnTowardsLedge();
			}
			else if(targetDirection != Vector3.zero){
				targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
			}

			Quaternion target = Quaternion.Euler(Vector3.up * targetRotation);
			transform.rotation = !isAttacking ? Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * turnSmoothTime) : target;
		}
		else if (isDodging){
			transform.rotation = Quaternion.Euler(Vector3.up * dodgeRotation);
		}
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

	#region animation event functions

	void ToggleWeaponHitbox(){
		var collider = playerWeapon.GetComponent<BoxCollider>();
		collider.enabled = !collider.enabled;
	}

	void StopAttack(){
		isAttacking = false;
		comboCounter = 0;
		missedComboWindow = false;
	}

	void BeginComboWindow(){
		missedComboWindow = false;
	}

	void EndComboWindow(){
		missedComboWindow = true;
	}

	void EndDodge(){
		isDodging = false;
		dodgeDirection = Vector3.zero;
		dodgeRotation = 0f;
	}

	#endregion

	#region hitbox functions

	void SetCrouchHitbox(){

	}

	void SetJumpHitbox(){
		
	}

	#endregion

	#region climbing functions

	void ClimbLedgeStart(){
		isClimbingLedge = true;
		controller.detectCollisions = false;
		animator.applyRootMotion = true;
	}

	void ClimbLedgeEnd(){
		isClimbingLedge = false;
		controller.detectCollisions = true;
		isHanging = false;
		animator.applyRootMotion = false;
	}

	float TurnTowardsLedge(){
		var ledgeFrontRot = (180f + hangingLedge.rotation.eulerAngles.y);
		var ledgeBackRot = (hangingLedge.rotation.eulerAngles.y);

		// turn perpendicular to ledge
		return System.Math.Abs(transform.rotation.eulerAngles.y - ledgeFrontRot) 
			> System.Math.Abs(transform.rotation.eulerAngles.y - ledgeBackRot) ? ledgeBackRot : ledgeFrontRot;
	}

	bool CheckForLedges(){
		var ledgenear = false;
		RaycastHit hit;
		var layerMask = 1 << 8;
		var rays = new Vector3[3];
		var climbCollider = climbHook.GetComponent<BoxCollider>(); 

		// check both ends and middle of hook for raycast
		rays[0] = climbHook.transform.position;
		rays[1] = climbHook.transform.position + (climbCollider.size.x / 40f) * Vector3.left ;
		rays[2] = climbHook.transform.position + (climbCollider.size.x / 40f) * Vector3.right ;

		// check for wall
		for(var i=0; i< rays.Length; i++){
			ledgenear = !Physics.Raycast(rays[0], Vector3.up , out hit, 0.05f, layerMask)
				& !Physics.Raycast(rays[0], Vector3.forward , out hit, 0.5f, layerMask) // climbhook is backwards???
				& Physics.Raycast(rays[0], -1f * Vector3.up , out hit, 0.05f, layerMask)
				& !climbHook.GetComponent<ClimbHookScript>().isColliding;

			// Need to rotate perpendicular to ledge
			if(ledgenear){
				hangingLedge = hit.transform;
				break;
			}
		}

		return ledgenear;
	}

	#endregion

	#region calculation helper functions

	float NormalizeAngle (float angle){
		while(angle < -180f){
			angle += 180f;
		}

		while(angle > 180f){
			angle -= 180f;
		}

		return angle;
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

		// if(specialMovementTrigger != null)
		// 	Gizmos.DrawRay(specialMovementTrigger.transform.position, specialMovementTrigger.transform.forward * 5);

		//print ("Gizmos");
    }

	#endregion
}