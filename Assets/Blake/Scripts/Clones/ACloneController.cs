using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACloneController : MonoBehaviour {
	
	#region components	

	public Animator animator;
	public CharacterController controller;

	#endregion

	#region variables

	float despawnTime;
	float hSpeed;
	public float lifeSpan;

	#endregion

	#region functions

	public abstract void MovePlayer();

	public abstract void RotatePlayer();

	public abstract void SetAnimations();

	public abstract void SetHitbox();

	#endregion

	public virtual void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		SetHitbox();
		despawnTime = Time.time + lifeSpan;
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
		RotatePlayer();
		MovePlayer();
		SetAnimations();

		if(lifeSpan > 0 && Time.time > despawnTime){
			Destroy(this.gameObject);
		}
	}

	#endregion

	#region debugging

	public void StopAttack(){

	}

	#endregion
}
