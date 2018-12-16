using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultController : APlayerController {

	public bool toggleRun = true;
	public GameObject currentAttackTarget;
	float inputX;
	float inputZ;
	bool buttonAttack1;
	bool buttonAttack2;
	bool buttonSemblance;
	bool buttonDodge;
	bool ignoreCollisions = false;
	float cloneLifeSpan = 10f;
	float fireRate = 0.1F;
	float nextFire = 0.0F;
	float fireTimeoutRate = 1f;
	float nextFireTimeout = 0;
	float playerMass = 1.5f;
	bool isClimbingLedge = false;
	bool isCrouching = false;
	bool isRunning = true;
	bool isJumping = false;
	bool isAttacking = false;
	bool isHanging = false;
	bool isDodging = false;
	bool isAimingGun = false;
	bool isUsingSemblance = false;
	bool shootProjectile = false;
	float meleeRange = 5f;
	float shootingRange = 15f;
	bool missedComboWindow = false;
	float jumpHeight = 4f;
	float walkSpeed = 2f;
	float runSpeed = 5f;
	float jumpSpeed = 1f;
	float speedSmoothTime = 0.1f;
	float jumpSmoothTime = 2f;
	float currentSpeed;
	float speedSmoothVelocity;
	float turnSmoothTime = 10f;
	float velY;
	GameObject clone;
	int comboCounter = 0;
	Vector3 targetDirection;
	Vector3 dodgeDirection;
	Vector3 vel;
	float dodgeRotation;
	static float gravity = -9.8f;
	public GameObject playerWeapon;
	public GameObject playerOffhandWeapon;
	public GameObject climbHook;
	Transform hangingLedge;

	#region APlayerController functions

	public override void HandleInputs(){
		buttonSemblance = clone == null ? Input.GetButtonDown("Semblance"): false;
		buttonDodge = Input.GetButtonDown("Dodge");
		shootProjectile = false;
		inputX = !isHanging ? Input.GetAxis("Horizontal") : 0f;
		inputZ = Input.GetAxis("Vertical");
		buttonAttack1 = Input.GetButtonDown("Attack1");
		buttonAttack2 = Input.GetButtonDown("Attack2");
		isCrouching = Input.GetButton("Crouch") & !isJumping;

		targetDirection = new Vector3(inputX, 0f, inputZ).normalized;

		if(inputZ < 0f && isHanging && !isClimbingLedge){
			isHanging = false;
		}

		// attacks
		if(buttonAttack1 && !isJumping){
			if(isAttacking && !missedComboWindow){
				comboCounter++;
			}
			else if(!isAttacking){
				isAimingGun = false;
				isAttacking = true;
			}
		}
		else if(buttonAttack2 && !isJumping){
			if (isAimingGun){
				shootProjectile = true;
			}
			else if(!isAimingGun){
				isAttacking = false;
				isAimingGun = true;
				shootProjectile = true;
			}
		}

		// jump
		if(Input.GetButtonDown("Jump") && (!isAttacking || isHanging || !isJumping || !isClimbingLedge)){
			velY = Mathf.Sqrt(-2f * gravity * jumpHeight);
			isHanging = false;
			isJumping = true;
			isAttacking = false;
		}

		// dodge
		if(Input.GetButtonDown("Dodge")) {
			isDodging = true;
			dodgeDirection = targetDirection;
			dodgeRotation = Mathf.Atan2(dodgeDirection.x, dodgeDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
		}

		// semblance
		if(buttonSemblance) {
			isUsingSemblance = true;
		}

		// run
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
			smoothTime = new Vector3(vel.x, 0f, vel.z).magnitude > jumpSpeed ? jumpSmoothTime : speedSmoothTime;
			targetSpeed = jumpSpeed * targetDirection.magnitude;
		}
		else if(isRunning && !isCrouching){
			targetSpeed = runSpeed * targetDirection.magnitude;
		}
		
		if(isHanging){
			smoothTime = 0f;
			targetSpeed = 0f;
		}

		//print(targetSpeed);
		//print(smoothTime);
		
		if(!isAttacking && !isDodging && !isUsingSemblance){
			currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
			vel = !isAimingGun ? transform.forward * currentSpeed + Vector3.up * velY
				: (Camera.main.transform.forward * currentSpeed * inputZ) + (Camera.main.transform.right * currentSpeed * inputX);

			// weird animation glitch
			if(isAimingGun){
				vel.y = 0f;
			}

			//print(vel);
			//print(vel.magnitude);
			animator.applyRootMotion = false;
			controller.Move(vel * Time.deltaTime);
		}
		else{
			animator.applyRootMotion = true;
		}

		if(!controller.isGrounded && (isHanging || CheckForLedges())){
			velY = 0f;
			isJumping = false;
			isHanging = true;
		}
		else if(!controller.isGrounded){
			velY += Time.deltaTime * gravity * playerMass;
		}
		else if(controller.isGrounded){
			velY = 0f;
			isJumping = false;
		}

		//print(controller.velocity);
		//print(animator.applyRootMotion);
	}

	public override void RotatePlayer(){
		// turn slowly based on camera's forward
		if(!isJumping && !isDodging && !isUsingSemblance){
			var targetRotation = transform.eulerAngles.y;
			
			if(isAttacking || isAimingGun){
				currentAttackTarget = (buttonAttack1 || buttonAttack2) 
					? GetNearestEnemy(isAttacking ? meleeRange : shootingRange) : currentAttackTarget;

				if(currentAttackTarget != null){
					targetRotation = Mathf.Atan2(currentAttackTarget.transform.position.x - transform.position.x
						, currentAttackTarget.transform.position.z - transform.position.z) * Mathf.Rad2Deg;	
				}
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
		else if(isDodging){
			transform.rotation = Quaternion.Euler(Vector3.up * dodgeRotation);
		}
	}

	public override void SetAnimations(){
		// play animations
		animator.SetFloat("HSpeed", currentSpeed);
		animator.SetBool("IsJumping", isJumping);
		animator.SetFloat("VSpeed", velY);
		animator.SetBool("IsAttacking", isAttacking);
		animator.SetBool("IsAimingGun", isAimingGun);
		animator.SetInteger("ComboCounter", comboCounter);
		animator.SetBool("IsCrouching", isCrouching);
		animator.SetBool("IsHanging", isHanging);
		animator.SetFloat("InputX", inputX);
		animator.SetFloat("InputZ", inputZ);
		animator.SetFloat("HSpeedX", inputX * currentSpeed);
		animator.SetFloat("HSpeedZ", inputZ * currentSpeed);
		animator.SetBool("IsDodging", isDodging);
		animator.SetBool("IsUsingSemblance", isUsingSemblance);
	}

	public override void PostEvents(){
		// alert children
		if (shootProjectile && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			nextFireTimeout = Time.time + fireTimeoutRate;
			playerWeapon.GetComponent<DefaultWeaponController>().ShootProjectile(currentAttackTarget);
		}
		else if(Time.time > nextFireTimeout){
			isAimingGun = false;
		}

		if(buttonSemblance){
			CreateClone("earth");
		}

		if(isUsingSemblance && clone != null){
			Physics.IgnoreCollision(controller, clone.GetComponent<Collider>());
		}
		else if (!isUsingSemblance && clone != null){
			Physics.IgnoreCollision(controller, clone.GetComponent<Collider>(), false);
		}
		
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

	void EndSemblance(){
		isUsingSemblance = false;
	}

	#endregion

	#region semblance functions

	void CreateClone(string type){
		//var clonePrefab = GameObject.Find("BlakeCloneIce");
		var clonePrefab = Resources.Load("Characters/Blake/Prefabs/BlakeCloneShadow") as GameObject;
		clone = Instantiate(clonePrefab, transform.position, transform.rotation);
		clone.GetComponent<ACloneController>().lifeSpan = cloneLifeSpan;
		clone.GetComponent<ACloneController>().enabled = true;
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

	#region attacking functions

	void ShootProjectile(){
		playerWeapon.GetComponent<DefaultWeaponController>();
	}

	GameObject GetNearestEnemy(float rayLength){
		RaycastHit hit;
		var raycastHitEnemies = new Dictionary<GameObject, float>();
		var layerMask = 1 << 11; // only enemy layer
		var degreeSlice = 2;
		var rays = new Vector3[180/degreeSlice];
		

		// check both ends and middle of hook for raycast
		rays[0] = climbHook.transform.position;

		/// ray cast 180 degrees in front of player and return nearest enemy
		for(var i=0; i<=180/degreeSlice; i++){
			var raycastCheck = Physics.Raycast(transform.position, Quaternion.Euler(0, degreeSlice * i, 0) * (transform.right * -1f), out hit, rayLength, layerMask);

			if(raycastCheck && !raycastHitEnemies.Keys.Contains(hit.collider.gameObject)){
				raycastHitEnemies.Add(hit.collider.gameObject, hit.distance);
				// print("Enemy Found " + hit.collider.gameObject.name);
				// print("AT distance " + hit.distance);
			}
		}

		// if player already has a target, don't switch
		// else return the closest enemy
		if(currentAttackTarget != null && raycastHitEnemies.ContainsKey(currentAttackTarget)){
			return currentAttackTarget;
		}
		else{
			return raycastHitEnemies.Count > 0 
				? raycastHitEnemies.OrderBy(x => x.Value).First().Key : null;
		}
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
         Gizmos.color = Color.red;
		 var degreeSlice = 5;
         //Gizmos.DrawRay(transform.position, Quaternion.Euler(0, degreeSlice * i, 0) * transform.forward);

		 for(var i=0; i<=180/degreeSlice; i++){
			Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, degreeSlice * i, 0) * (transform.right * -1f)) * 2f);
		}
    }

	#endregion
}
