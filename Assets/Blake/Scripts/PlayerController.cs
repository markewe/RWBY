using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public bool toggleRun = true;
	bool isClimbingLedge = false;
	bool isSliding;
	bool isRunning = true;
	bool isJumping = false;
	bool isAttacking = false;
	bool isHanging = false;
	bool isDodging = false;
	
	bool missedComboWindow = false;
	float hitboxHeight = 1.7f;
	float hitboxRadius = 0.12f;

	float jumpHeight = 4f;

	float walkSpeed = 2f;
	float runSpeed = 8f;
	float jumpSpeed = 1f;
	float speedSmoothTime = 0.1f;

	float slideSmoothTime = 0.5f;
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
	static int attackTimeout = 120;

	Transform mainCameraT;

	CharacterController controller;

	public GameObject playerWeapon;
	public GameObject climbHook;
	Transform hangingLedge;
	
	Animator animator;
	Rigidbody rigibody;

	// Use this for initialization
	void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		rigibody = GetComponent<Rigidbody>();
		mainCameraT = Camera.main.transform;
		targetDirection = new Vector3(0f, 0f, 0f);
		dodgeDirection = new Vector3(0f, 0f, 0f);
		SetNormalHitbox();
	}

	void OnEnable(){
		Init();
	}

	void Start () {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		rigibody = GetComponent<Rigidbody>();
		mainCameraT = Camera.main.transform;
		targetDirection = new Vector3(0f, 0f, 0f);
		dodgeDirection = new Vector3(0f, 0f, 0f);
		SetNormalHitbox();
	}
	
	// Update is called once per frame
	void Update () {
		
		var inputX = !isHanging ? Input.GetAxisRaw("Horizontal") : 0f;
		var inputZ = Input.GetAxisRaw("Vertical");
		var isCrouching = Input.GetButton("Crouch") & !isJumping;

		targetDirection = new Vector3(inputX, 0f, inputZ);
		//targetDirection = targetDirection.normalized;

		//print("begin " + targetDirection);

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

			//isRunning = false;
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
			dodgeRotation = Mathf.Atan2(dodgeDirection.x, dodgeDirection.z) * Mathf.Rad2Deg + mainCameraT.eulerAngles.y;
		}

		if(!toggleRun){
			isRunning = Input.GetButton("Run");
		}
		else {
			if(Input.GetButtonDown("Run")) {
				isRunning = !isRunning;
			}
		}

		// determine whether to slide or not
		// if(currentSpeed > 7f && isRunning && isCrouching && !isSliding && !isJumping){
		// 	isSliding = true;
		// }
		
		// turn slowly based on camera's forward
		if(!isJumping && !isDodging){
			var targetRotation = transform.eulerAngles.y;
			
			if(isAttacking){
				targetRotation = mainCameraT.eulerAngles.y;
			}
			else if(isHanging){
				targetRotation = TurnTowardsLedge();
			}
			else if(targetDirection != Vector3.zero){
				targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + mainCameraT.eulerAngles.y;
			}

			Quaternion target = Quaternion.Euler(Vector3.up * targetRotation);
			transform.rotation = !isAttacking ? Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * turnSmoothTime) : target;

			// print(targetRotation);
			// print(transform.rotation);
		}
		else if (isDodging){
			transform.rotation = Quaternion.Euler(Vector3.up * dodgeRotation);
		}

		var targetSpeed = walkSpeed * targetDirection.magnitude;
		var smoothTime = speedSmoothTime;
		
		//print("before " + targetDirection);

		if(isSliding){
			smoothTime = slideSmoothTime;
			targetSpeed = 0f;
		}
		else if(isJumping){
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
		
		//print("after " + targetDirection);

		if(isDodging){
			//print(dodgeDirection.magnitude);
			//print(runSpeed);
			//print(transform.forward);
			//print("dodging");

			controller.Move(transform.forward * (runSpeed * dodgeDirection.magnitude) * Time.deltaTime);
		}
		
		else if(!isAttacking && !isDodging){
			currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
			Vector3 vel = transform.forward * currentSpeed + Vector3.up * velY;

			//print(vel);
			// print(controller.isGrounded);
			//// print(isHanging);
			//print("walking");
			controller.Move(vel * Time.deltaTime);
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

		// play animations
		
		animator.SetFloat("HSpeed", currentSpeed);
		animator.SetBool("IsJumping", isJumping);
		animator.SetFloat("VSpeed", velY);
		animator.SetBool("IsAttacking", isAttacking);
		animator.SetInteger("ComboCounter", comboCounter);
		animator.SetBool("IsCrouching", isCrouching);
		animator.SetBool("IsSliding", isSliding);
		animator.SetBool("IsHanging", isHanging);
		animator.SetFloat("InputX", inputX);
		animator.SetFloat("InputZ", inputZ);
		animator.SetBool("IsDodging", isDodging);

		//print(currentSpeed);
	}

	void LateUpdate(){
		
	}

	void ToggleWeaponHitbox(){
		var collider = playerWeapon.GetComponent<BoxCollider>();
		collider.enabled = !collider.enabled;
	}

	float DeltaFloat(float inputSpeed)
	{
		return inputSpeed * Time.deltaTime;
	}

	void StopAttack(){
		isSliding = false;
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

	void EndSlide(){
		//print("EndSlide");
		//isRunning = false;
		isSliding = false;
	}

	void EndDodge(){
		isDodging = false;
		dodgeDirection = Vector3.zero;
		dodgeRotation = 0f;
	}

	void OnCollisionEnter(Collision col){
		//print(col.gameObject.tag);
		// if(col.gameObject.tag.Equals("Enemy")){
		// 	currentSpeed = 0;
		// }
	}



	#region hitbox functions

	void SetNormalHitbox(){
		controller.height = 1.7f;
		controller.center = new Vector3(0f,  1.7f / 2f, 0f);
	}

	void SetCrouchHitbox(){

	}

	void SetJumpHitbox(){
		
	}

	#endregion

	#region jumping functions



	#endregion

	#region crawling functions

	void StartCrawl(){
		
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

	#region helper functions

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