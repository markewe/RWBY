using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControls : MonoBehaviour {
	#region character components

	public Animator animator;
	public CharacterController characterController;

	PlayerInputs playerInputs;

	#endregion
	
	public abstract void HandleInputs(PlayerInputs playerInputs);

	public abstract void Translate();

	public abstract void Rotate();

	public abstract void SetAnimationParams();

	public abstract void PostEvents();

	public abstract void ExitControls();

	public virtual void Init(){
		characterController = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
	}

	#region Monobehavior functions

	 public virtual void OnEnable(){
		Init();
	}

	public virtual void Awake () {
		//Init();
	}
	public virtual void Start () {
		//Init();
	}

	#endregion

	#region event function

	void OnControllerColliderHit(ControllerColliderHit hit)
     {
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body != null && !body.isKinematic)
		{
			body.velocity += hit.controller.velocity.normalized * 0.1f;
		}
     }

	#endregion

	#region animation event functions

	public virtual void DisableTerrainCollisions(){
		Physics.IgnoreLayerCollision(8, 9);
		//controller.detectCollisions = false;
	}

	public virtual void EnableTerrainCollisions(){
		Physics.IgnoreLayerCollision(8, 9, false);
		//controller.detectCollisions = true;
	}

	public virtual void DisableCollisions(){
		characterController.detectCollisions = false;
	}

	public virtual void EnableCollisions(){
		characterController.detectCollisions = true;
	}

	#endregion

	#region helper functions

	public void FaceObject(GameObject target, float turnSmooth){
		var direction = (target.transform.position - transform.position).normalized;	
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSmooth);
	}

	#endregion

}
