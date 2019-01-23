using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolController : AEnemyController {
	
	float walkSpeed = 2f;
	LinkedList<Vector3> patrolLocations;
	LinkedListNode<Vector3> currentPatrolLocation;


	// Use this for initialization
	public override void Start () {
		base.Start();
		patrolLocations = new LinkedList<Vector3>();
		
		patrolLocations.AddLast(transform.position);
		patrolLocations.AddLast(transform.position + new Vector3(10f, 0f, 0f));

		currentPatrolLocation = patrolLocations.First;
	}
	
	// Update is called once per frame
	public override void Update () {
		if((transform.position - currentPatrolLocation.Value).magnitude < 0.1f){
			if(currentPatrolLocation.Next != null){
				currentPatrolLocation = currentPatrolLocation.Next;
			}
			else{
				currentPatrolLocation = patrolLocations.First;
			}

			if(agent.isActiveAndEnabled){
				agent.SetDestination(currentPatrolLocation.Value);
				animator.SetFloat("HSpeed", agent.velocity.magnitude * walkSpeed);
			}
		}

		if(isInTakedown){
			FaceObject(PlayerManager.instance.player, 20f);
		}

		base.Update();
	}

	public override void TargetEnteredFieldOfVision(GameObject newTarget){

	}

	public override void TargetExitedFieldOfVision(GameObject newTarget){
		
	}

	void StopIdle(){}

	void StopAttack(){}
}