using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class APlayerController : MonoBehaviour {
	#region character components

	public Animator animator;
	public CharacterController controller;

	#endregion
	
	public abstract void HandleInputs();

	public abstract void MovePlayer();

	public abstract void RotatePlayer();

	public abstract void SetAnimations();

	public abstract void SetHitbox();
	public abstract void PostEvents();

	public virtual void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		SetHitbox();
	}

	#region Monobehavior functions

	 public virtual void OnEnable(){
		Init();
	}

	public virtual void Awake () {
		Init();
	}
	public virtual void Start () {
		Init();
	}

	public virtual void Update(){
		HandleInputs();
		RotatePlayer();
		MovePlayer();
		SetAnimations();
		PostEvents();
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
		controller.detectCollisions = false;
	}

	public virtual void EnableCollisions(){
		controller.detectCollisions = true;
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
