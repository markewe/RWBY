using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour {//, IHealthListener {
	
	Animator animator;
	CameraController cameraController;
	CharacterController characterController;

	[SerializeField]
	GameObject ledgeChecker;
	GameObject specialMovementTrigger;
	GameObject mainCamera;
	PlayerControls currentControls;
	PlayerInputs playerInputs;
	bool canWallHug = false;
	public RaycastHit ledgeInfo;
	bool inSpecialMovement = false;
	bool inTakedownRange = false;
	float wallHugPressTimer = 0f;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
		currentControls = GetComponent<DefaultPlayerControls>();
		currentControls.enabled = true;
		animator = GetComponent<Animator>();
		// mainCamera = GameObject.Find("CameraObject");
		// cameraController = mainCamera.GetComponent<CameraController>();
	}

	void Update(){
		playerInputs = CaptureInputs();

		SetControls();
		currentControls.HandleInputs(playerInputs);
		currentControls.Translate();
		currentControls.Rotate();
		currentControls.SetAnimationParams();
		currentControls.PostEvents();
	}

	PlayerInputs CaptureInputs(){
		return new PlayerInputs(){
			buttonMainAttack = Input.GetButton("Main Attack")
			, buttonAltAttack = Input.GetButton("Alt Attack")
			, buttonCrouch = Input.GetButton("Crouch")
			, buttonDodge = Input.GetButton("Dodge")
			, buttonInteract = Input.GetButton("Interact")
			, buttonJump = Input.GetButton("Jump")
			, buttonRun = Input.GetButton("Run")
			, buttonSemblance = Input.GetButton("Semblance")
			, inputX = Input.GetAxis("Horizontal")
			, inputZ = Input.GetAxis("Vertical")
		};;
	}

	void SetControls() {
		if(currentControls is DefaultPlayerControls){
			currentControls.enabled = false;

			if(playerInputs.buttonInteract && specialMovementTrigger != null){

			}
			else if(playerInputs.buttonJump && ledgeInfo.transform != null){
				currentControls = GetComponent<LedgeClimbingPlayerControls>();
				(currentControls as LedgeClimbingPlayerControls).ledgeInfo = ledgeInfo;
			}

			currentControls.enabled = true;
		}
	}

	#region physics events

	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.gameObject.layer == (int)Layers.Environment){
			//if(playerInputs.buttonDodge && wallHugPressTimer >= 0.5f){
			if(wallHugPressTimer >= .25f){
				var raycastHit = CheckWallHug();

				//print(raycastHit.transform);
				//print(CharacterIsGrounded());

				if(raycastHit.transform != null && CharacterIsGrounded()){
					//print(raycastHit.point);
					var whc = GetComponent<WallHuggingPlayerControls>();
					
					currentControls.enabled = false;
					currentControls = whc;
					whc.wallInfo = raycastHit;
					whc.enabled = true;
				}

				wallHugPressTimer = 0f;
				canWallHug = false;
			}
			else if(playerInputs.inputZ > 0f){
				wallHugPressTimer += Time.deltaTime;
			}
		}
		else{
			wallHugPressTimer = 0f;
			canWallHug = false;
		}
	}

	void OnCollisionEnter(Collision collision){
		print(collision.gameObject.name);
	}

	void OnTriggerEnter(Collider col){
		print(col.gameObject.name + " Enter");

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

	public bool CharacterIsGrounded(){
		return characterController.isGrounded || Math.Abs(characterController.velocity.y) <= 0.1f;
	}

	#endregion

	#region control switch functions

	RaycastHit CheckWallHug(){
		// check if facing a wall and next to it and not falling
		// then become Solid Snake
		RaycastHit hit;
		var wallMaxDist = 0.01f;
		var layerMask = 1 << (int)Layers.Environment;
		var playerMidpoint = transform.position;
		var wallNear = Physics.Raycast(playerMidpoint + (characterController.radius * transform.forward), transform.forward, out hit, wallMaxDist, layerMask);

		// if(wallNear)
		// 	print("WALL NEAR " + wallNear);

		return hit;
	}

	RaycastHit CheckLedgeClimb(){
		RaycastHit hit;
		var layerMask = 1 << (int)Layers.Environment;
		var ledgeCheckerCollider = ledgeChecker.GetComponent<BoxCollider>();
		var rayLength = (ledgeChecker.transform.position - transform.position).y - 1f;

        Physics.Raycast(ledgeChecker.transform.position, (ledgeChecker.transform.up * -1f), out hit, rayLength, layerMask);

		return hit;
	}

	public void RestoreDefaultControls(){
		currentControls.enabled = false;
		currentControls = GetComponent<DefaultPlayerControls>();
		currentControls.enabled = true;
	}

	#endregion

	#region IHealthListener functions

	// public void OnTakeDamage(){
	// 	if(currentControls is HoldingObjectController){
	// 		(currentControls as HoldingObjectController).DropObject();
	// 	}
	// 	else if(currentControls is DefaultController){
	// 		(currentControls as DefaultController).OnTakeDamage();
	// 	}
	// }

	public void OnHealDamage(){}

	public void OnZeroHealth(){
		currentControls.enabled = false;

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

		// if(specialMovementTrigger != null)
			//Gizmos.DrawRay(specialMovementTrigger.transform.position, specialMovementTrigger.transform.forward * 5);

		if(characterController != null)
			Gizmos.DrawRay(transform.position + (characterController.radius * transform.forward), transform.forward * 0.5f);

		//print ("Gizmos");
	}

	#endregion
}
