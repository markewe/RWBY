using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AEnemyController : MonoBehaviour, IHealthListener {
	public NavMeshAgent agent;
	public Animator animator;
	Rigidbody rbody;

	public bool isInTakedown = false;

	public virtual void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();		
		rbody = GetComponent<Rigidbody>();
	}

	public virtual void Update(){}
	public abstract void TargetEnteredFieldOfVision(GameObject newTarget);
	public abstract void TargetExitedFieldOfVision(GameObject newTarget);

	public void FaceObject(GameObject target, float turnSmooth){
		var direction = (target.transform.position - transform.position).normalized;	
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSmooth);
	}

	public void InitTakedown(){
		isInTakedown = true;
		agent.enabled = false;
	}

	public void PerformTakedown(){
		rbody.detectCollisions = false;
		animator.applyRootMotion = true;
		animator.SetBool("PerformTakedown", true);
	}

	public void EndTakedown(){
		isInTakedown = false;
	}

	#region IHealthListener functions

	public void OnTakeDamage(){
		agent.enabled = false;
		animator.SetBool("IsHit");
	}

	public void OnHealDamage(){}

	public void OnZeroHealth(){
		agent.enabled = false;

		if(IsGrounded){
			animator.SetBool("IsKnockedOut", true);
		}
		else{
			animator.SetBool("IsKnockedOutAir", true);
		}
	}

	#endregion

	#region animation events

	public void HitStunEnd(){
		agent.enabled = true;
	}

	#endregion

	#region helper functions

	bool IsGrounded(){
		return false;
	}

	#endregion
}