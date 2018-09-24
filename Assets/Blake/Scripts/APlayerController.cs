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

	public virtual void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		SetHitbox();
	}

	#region Monobehavior functions

	 public virtual void OnEnable(){
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
	}

	#endregion

}
