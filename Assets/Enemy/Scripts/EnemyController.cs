using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : AEnemyController {

	[SerializeField]
	GameObject patrolWaypointsObject;

	public static float lookRadius = 7f;
	public static float stopDistance = 5f;
	public static float turnSmooth = 2f;
	public static float retreatDistance = 3f;	

	bool isAttacking;

	GameObject target;
	EnemyState state;
	List<Vector3> patrolWaypoints;
	System.Random random;

	// Use this for initialization
	public override void Start () {
		base.Start();
		random = new System.Random();
		state = EnemyState.Patrol;

		patrolWaypoints = new List<Vector3>();

		//agent.stoppingDistance = stopDistance;

		foreach(Transform waypoint in patrolWaypointsObject.transform){
			patrolWaypoints.Add(waypoint.position);
		}
		
		agent.SetDestination(patrolWaypoints[1]);
	}
	
	// Update is called once per frame
	public override void Update () {
		switch(state){
			case EnemyState.Patrol:
				Patrol();
				break;
			case EnemyState.Hostile:
				Attack();
				break;
		}
	}

	void Patrol(){
		if((transform.position - agent.destination).magnitude < 0.1f){
			var nextDestination = patrolWaypoints[random.Next(patrolWaypoints.Count)];

			if((nextDestination - agent.destination).magnitude > 0.1f){
				agent.SetDestination(nextDestination);
			}
			else{
				Idle();
			}
		}

		animator.SetFloat("HSpeed", agent.velocity.magnitude);
	}

	void Idle(){

	}

	void Attack(){
		var distance = Vector3.Distance(target.transform.position, transform.position);
		var direction = 1f;

		if(distance < lookRadius && distance >= stopDistance){
			direction = 1f;
			isAttacking = true;
			agent.updateRotation = true;
			agent.SetDestination(target.transform.position);
		}
		else if(distance < stopDistance && distance >= retreatDistance){
			FaceTarget();
		}
		else if(distance < retreatDistance ){
			direction = -1f;
			agent.updateRotation = false;
			var retreatPos = (transform.position - target.transform.position).normalized * stopDistance;
			agent.SetDestination(retreatPos);
			FaceTarget();
		}
		else if(distance >= lookRadius){
			isAttacking = false;
		}

		animator.SetBool("IsAttacking", isAttacking);
		animator.SetFloat("HSpeed", agent.velocity.magnitude * 2f * direction);
		//print(agent.destination);
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag.Equals("PlayerWeapon")){
			print("helpme");
		}
	}

	void FaceTarget(){
		var direction = (target.transform.position - transform.position).normalized;
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSmooth);
	}

	#region debug

	public void StopAttack(){

	}

	#endregion
}
