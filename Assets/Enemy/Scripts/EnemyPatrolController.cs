using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolController : MonoBehaviour {
	NavMeshAgent agent;
	Animator animator;
	float walkSpeed = 2f;
	LinkedList<Vector3> patrolLocations;
	LinkedListNode<Vector3> currentPatrolLocation;

	// Use this for initialization
	void Start () {
		patrolLocations = new LinkedList<Vector3>();
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();

		patrolLocations.AddLast(transform.position);
		patrolLocations.AddLast(transform.position + new Vector3(10f, 0f, 0f));

		currentPatrolLocation = patrolLocations.First;
	}
	
	// Update is called once per frame
	void Update () {
		if((transform.position - currentPatrolLocation.Value).magnitude < 0.1f){
			if(currentPatrolLocation.Next != null){
				currentPatrolLocation = currentPatrolLocation.Next;
			}
			else{
				currentPatrolLocation = patrolLocations.First;
			}
		}

		if(agent.isActiveAndEnabled){
			agent.SetDestination(currentPatrolLocation.Value);
			animator.SetFloat("HSpeed", agent.velocity.magnitude * walkSpeed);
		}
	}

	void StopAttack(){}
}
