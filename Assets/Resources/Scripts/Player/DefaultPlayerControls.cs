using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultPlayerControls : PlayerControls {

	public GameObject currentAttackTarget;
	public GameObject playerWeapon;
	public GameObject playerOffhandWeapon;

	static float gravity = -9.8f;
	
	GameObject clone;
	Vector3 dodgeDirection;
	Vector3 targetDirection;
	Vector3 vel;

	bool buttonMainAttack = false;
	bool buttonSemblance;
	bool canCombo = false;
	bool isAimingGun = false;
	bool isAttackingMelee = false;
	bool isCrouching = false;
	bool isDodging = false;
	bool isJumping = false;
	bool isLanding = false;
	bool isRunning = true;
	bool isUsingSemblance = false;
	bool shootProjectile = false;
	bool toggleRun = true;
	float currentSpeed;
	float dodgeRotation;
	float fireRate = 0.1F;
	float fireTimeoutRate = 1f;
	float inputX;
	float inputZ;
	float jumpHeight = 4f;
	float jumpSmoothTime = 2f;
	float jumpSpeed = 1f;
	float meleeRange = 5f;
	float nextFire = 0.0F;
	float nextFireTimeout = 0;
	float playerMass = 1.5f;
	float runSpeed = 5f;
	float shootingRange = 15f;
	float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float turnSmoothTime = 10f;
	float velY;
	float walkSpeed = 2f;
	int comboCounter = 0;

	#region PlayerControls functions

	public override void HandleInputs(PlayerInputs playerInputs){
		buttonMainAttack = playerInputs.buttonMainAttack;
		buttonSemblance = clone == null ? playerInputs.buttonSemblance : false;
		shootProjectile = false;
		inputX = playerInputs.inputX;
		inputZ = playerInputs.inputZ;

		targetDirection = new Vector3(inputX, 0f, inputZ).normalized;

		// jump
		// if(playerInputs.buttonJump && !isAttackingMelee && !isJumping){
		// 	velY = Mathf.Sqrt(-2f * gravity * jumpHeight);
		// 	isJumping = true;
		// }

		// attacks
		if(buttonMainAttack && CanAttack()){
			// else if(!isAttackingMelee){
				// isAimingGun = false;
				isAttackingMelee = true;
			// }
		}
		// else if(buttonAttack2 && !isJumping){
		// 	if (isAimingGun){
		// 		shootProjectile = true;
		// 	}
		// 	else if(!isAimingGun){
		// 		isAttackingMelee = false;
		// 		isAimingGun = true;
		// 		shootProjectile = true;
		// 	}
		// }

		//crouch
		isCrouching = playerInputs.buttonCrouch && CanCrouch();
		//print(isCrouching);

		// dodge
		if(playerInputs.buttonDodge) {
			isDodging = true;
			dodgeDirection = targetDirection;
			dodgeRotation = Mathf.Atan2(dodgeDirection.x, dodgeDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
		}

		// semblance
		if(buttonSemblance) {
			isUsingSemblance = true;
		}

		// run
		if(playerInputs.buttonRun){
			isRunning = !toggleRun ? playerInputs.buttonRun : !isRunning;
		}
	}

	public override void Translate(){
		// determine how fast to move
		var targetSpeed = walkSpeed * targetDirection.magnitude;
		var smoothTime = speedSmoothTime;
		
		if(isLanding){
			targetSpeed = 0f;
			smoothTime = 0.2f;
		}
		else if(isJumping){
			smoothTime = new Vector3(vel.x, 0f, vel.z).magnitude > jumpSpeed ? jumpSmoothTime : speedSmoothTime;
			targetSpeed = jumpSpeed * targetDirection.magnitude;
		}
		else if(isRunning && !isCrouching){
			targetSpeed = runSpeed * targetDirection.magnitude;
		}
		
		if(!isAttackingMelee && !isDodging && !isUsingSemblance){
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
			currentSpeed = 0f;
			animator.applyRootMotion = true;
		}

		// print(controller.isGrounded);
		// print(velY);

		if(controller.isGrounded){
			velY = 0f;
			isJumping = false;
		}
		else {
			velY += Time.deltaTime * gravity * playerMass;
		}

		//print(controller.velocity);
		//print(animator.applyRootMotion);
	}

	public override void Rotate(){
		// turn slowly based on camera's forward
		if(!isJumping && !isDodging && !isUsingSemblance){
			var targetRotation = transform.eulerAngles.y;
			
			if(isAttackingMelee || isAimingGun){
				currentAttackTarget = GetNearestEnemy(isAttackingMelee ? meleeRange : shootingRange);

				if(currentAttackTarget != null){
					targetRotation = Mathf.Atan2(currentAttackTarget.transform.position.x - transform.position.x
						, currentAttackTarget.transform.position.z - transform.position.z) * Mathf.Rad2Deg;	
				}
			}
			else if(targetDirection != Vector3.zero){
				targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
			}

			Quaternion targetQuaternion = Quaternion.Euler(Vector3.up * targetRotation);
			transform.rotation = !isAttackingMelee ? Quaternion.Slerp(transform.rotation, targetQuaternion,  Time.deltaTime * turnSmoothTime) : targetQuaternion;
		}
		else if(isDodging){
			transform.rotation = Quaternion.Euler(Vector3.up * dodgeRotation);
		}
	}

	public override void SetAnimationParams(){
		// play animations
		//print(currentSpeed);
		animator.SetBool("buttonMainAttack", buttonMainAttack);
		animator.SetBool("canCombo", canCombo);
		// animator.SetInteger("comboCounter", comboCounter);
		animator.SetFloat("hSpeed", currentSpeed);
		animator.SetBool("isJumping", isJumping);
		animator.SetFloat("vSpeed", velY);
		animator.SetBool("isAttackingMelee", isAttackingMelee);
		// animator.SetBool("isAimingGun", isAimingGun);
		
		animator.SetBool("isCrouching", isCrouching);
		animator.SetFloat("inputX", inputX);
		animator.SetFloat("inputZ", inputZ);
		animator.SetFloat("hSpeedX", inputX * currentSpeed);
		animator.SetFloat("hSpeedZ", inputZ * currentSpeed);
		animator.SetBool("isDodging", isDodging);
		animator.SetBool("isUsingSemblance", isUsingSemblance);
	}

	public override void PostEvents(){
		// alert children
		if (shootProjectile && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			nextFireTimeout = Time.time + fireTimeoutRate;
			//playerWeapon.GetComponent<RangedWeaponController>().Attack(currentAttackTarget);
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

	public override void ExitControls(){

	}

	public void SetHitbox(){
		controller.height = 1.7f;
		controller.center = new Vector3(0f,  1.7f / 2f, 0f);
	}

	#endregion

	#region state check functions

	bool CanAttack(){
		return !isJumping && !isDodging && !isUsingSemblance;
	}

	bool CanCrouch(){
		return !isJumping && !isDodging && !isUsingSemblance && !isAttackingMelee;
	}

	// bool CheckForWallHug(){
	// 	// check if pressing against wall
	// }

	#endregion

	#region other event functions

	public void OnTakeDamage(){

	}

	#endregion

	#region animation event functions

	void AttackEnd(){
		isAttackingMelee = false;
	}

	void ComboWindowBegin(){
		canCombo = true;
	}

	void ComboWindowEnd(){
		canCombo = false;
		comboCounter = 0;
	}

	void LandBegin(){
		isLanding = true;
	}

	void LandEnd(){
		isLanding = false;
	}

	void DdogeEnd(){
		isDodging = false;
		dodgeDirection = Vector3.zero;
		dodgeRotation = 0f;
	}

	#endregion

	#region semblance functions

	void CreateClone(string type){
		//var clonePrefab = GameObject.Find("BlakeCloneIce");
		var clonePrefab = Resources.Load("Characters/Blake/Prefabs/BlakeCloneShadow") as GameObject;
		clone = Instantiate(clonePrefab, transform.position, transform.rotation);
		// clone.GetComponent<ACloneController>().lifeSpan = cloneLifeSpan;
		// clone.GetComponent<ACloneController>().enabled = true;
	}

	#endregion

	#region other hitbox functions

	void SetCrouchHitbox(){

	}

	void SetJumpHitbox(){
		
	}

	#endregion

	#region attacking functions

	GameObject GetNearestEnemy(float rayLength){
		RaycastHit hit;
		var raycastHitEnemies = new Dictionary<GameObject, float>();
		var layerMask = 1 << 11; // only enemy layer
		var degreeSlice = 2;

		/// ray cast 180 degrees in front of player and return nearest enemy
		for(var i=0; i<=180/degreeSlice; i++){
			var raycastCheck = Physics.Raycast(transform.position, Quaternion.Euler(0, degreeSlice * i, 0) * (transform.right * -1f), out hit, rayLength, layerMask);

			if(raycastCheck && !raycastHitEnemies.Keys.Contains(hit.collider.gameObject)){
				raycastHitEnemies.Add(hit.collider.gameObject, hit.distance);
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
		// Gizmos.color = Color.red;
		// var degreeSlice = 5;
		// 		//Gizmos.DrawRay(transform.position, Quaternion.Euler(0, degreeSlice * i, 0) * transform.forward);

		// for(var i=0; i<=180/degreeSlice; i++){
		// 	Gizmos.DrawRay(transform.position, (Quaternion.Euler(0, degreeSlice * i, 0) * (transform.right * -1f)) * 2f);
		// }
	}

	#endregion
}
