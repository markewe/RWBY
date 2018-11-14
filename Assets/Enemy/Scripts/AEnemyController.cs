using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AEnemyController : MonoBehaviour {
	public NavMeshAgent agent;
	public Animator animator;
	public CharacterController controller;
	Rigidbody rigidbody;

	bool isInTakedown = false;

	public virtual void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();		
		controller = GetComponent<CharacterController>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public virtual void Update(){
		if(isInTakedown){
			
		}
	}

	public void PerformTakedown(){
		//isInTakedown = true;
		agent.enabled = false;
		rigidbody.detectCollisions = false;
		animator.applyRootMotion = true;
		animator.SetBool("IsInTakedown", true);
	}
}
