using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTakedownHandler : MonoBehaviour {
	Animator animator;
	NavMeshAgent agent;
	Rigidbody rigidBody;
	bool isInTakedown;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rigidBody = GetComponent<Rigidbody>();
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InitTakedown(){
		isInTakedown = true;
		agent.enabled = false;
	}

	public void PerformTakedown(){
		rigidBody.detectCollisions = false;
		animator.applyRootMotion = true;
		animator.SetBool("PerformTakedown", true);
	}

	public void EndTakedown(){
		isInTakedown = false;
	}
}
