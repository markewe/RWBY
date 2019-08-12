using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInputHandler : MonoBehaviour {//, IHealthListener {
	[SerializeField]
	GameObject firstPersonCamera;

	[SerializeField]
	GameObject thirdPersonCamera;

	[SerializeField]
	GameObject ledgeChecker;

	Animator animator;
	CharacterController characterController;
	GameObject currentCamera;
	GameObject interactionTarget;
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
		thirdPersonCamera.SetActive(true);
		firstPersonCamera.SetActive(false);
		currentCamera = thirdPersonCamera;
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
			, buttonInteract = Input.GetButtonDown("Interact")
			, buttonJump = Input.GetButton("Jump")
			, buttonSemblance = Input.GetButton("Semblance")
			, buttonTakeCover = Input.GetButtonDown("Take Cover")
			, inputX = Input.GetAxis("Horizontal")
			, inputZ = Input.GetAxis("Vertical")
			, mouseX = Input.GetAxis("Mouse X")
		};
	}

	void SetControls() {
		if(currentControls is DefaultPlayerControls){
			currentControls.enabled = false;

			if(playerInputs.buttonInteract && interactionTarget != null){
				playerInputs.buttonInteract = false;
				interactionTarget.GetComponent<IInteractable>().OnInteract();
				// currentControls = GetComponent<InteractingPlayerControls>();

				if(interactionTarget.GetComponent<Door>().doorType == DoorType.Vent){
					currentControls = GetComponent<CrawlingPlayerControls>();
				}
			}
			else if(playerInputs.buttonJump && ledgeInfo.transform != null){
				currentControls = GetComponent<LedgeClimbingPlayerControls>();
				(currentControls as LedgeClimbingPlayerControls).ledgeInfo = ledgeInfo;
			}
			else if(playerInputs.buttonTakeCover){
				var wallInfo = GetClosestWall();
				playerInputs.buttonTakeCover = false;	// so it doesn't trigger when new controls get enabled

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
		if(col.gameObject.GetComponent<IInteractable>() != null){
			interactionTarget = col.gameObject;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.gameObject.layer == (int)GameLayers.Environment){
			interactionTarget = null;
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
		var layerMask = 1 << (int)GameLayers.Environment;
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
		var layerMask = 1 << (int)GameLayers.Environment;
		var ledgeCheckerCollider = ledgeChecker.GetComponent<BoxCollider>();
		var rayLength = (ledgeChecker.transform.position - transform.position).y - 1f;

        Physics.Raycast(ledgeChecker.transform.position, (ledgeChecker.transform.up * -1f), out hit, rayLength, layerMask);

		return hit;
	}

	public void RestoreDefaultControls(){
		currentControls.ExitControls();
		currentControls.enabled = false;
		currentControls = GetComponent<DefaultPlayerControls>();
		currentControls.enabled = true;

		SwitchCamera(thirdPersonCamera);
	}

	void SwitchCamera(GameObject newCamera){
		if(currentCamera != newCamera){
			newCamera.SetActive(true);
			currentCamera.SetActive(false);
			currentCamera = newCamera;
		}
        
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
