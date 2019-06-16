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
	public RaycastHit ledgeInfo;
	bool inSpecialMovement = false;
	bool inTakedownRange = false;

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
			, buttonInteract = Input.GetButton("Interact")
			, buttonJump = Input.GetButton("Jump")
			, buttonSemblance = Input.GetButton("Semblance")
			, buttonTakeCover = Input.GetButton("Take Cover")
			, inputX = Input.GetAxis("Horizontal")
			, inputZ = Input.GetAxis("Vertical")
		};
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
			else if(playerInputs.buttonTakeCover){
				var wallInfo = GetClosestWall();

				if(CharacterIsGrounded() && wallInfo.transform != null){
					currentControls = GetComponent<WallHuggingPlayerControls>();
					(currentControls as WallHuggingPlayerControls).wallInfo = wallInfo;
				}
			}

			currentControls.enabled = true;
		}
	}

	#region physics events

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

	RaycastHit GetClosestWall(){
		// check all around character in 45 degree angles for a wall, return wall that's closest
		RaycastHit shortestHit;
		RaycastHit curHit;
		var layerMask = 1 << (int)Layers.Environment;
		var rayLength = 0.1f + characterController.radius;

		Physics.Raycast(transform.position, transform.forward, out shortestHit, rayLength, layerMask);

		for(var i=1; i<360/45; i++){
			Physics.Raycast(transform.position, Quaternion.Euler(0, i * 45, 0) * transform.forward, out curHit, rayLength, layerMask);

			if(shortestHit.transform == null || (curHit.distance < shortestHit.distance)){
				shortestHit = curHit;
			}
		}

		return shortestHit;
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
		{
			//Gizmos.DrawRay(transform.position + (characterController.radius * transform.forward), transform.forward * 0.5f);
			// Gizmos.DrawRay(transform.position + (characterController.radius) * (transform.right * 1f), transform.forward * 0.5f);
			// Gizmos.DrawRay(transform.position + (characterController.radius) * (transform.right * -1f), transform.forward * 0.5f);
			Gizmos.DrawRay(transform.position + new Vector3(0f, characterController.height / 2f, 0f), transform.forward * 0.5f);
		}
			


		//print ("Gizmos");
	}

	#endregion
}
