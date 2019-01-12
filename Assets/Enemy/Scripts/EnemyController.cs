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
	bool isIdling;
	bool walkBackwards = false;
	float attackRadius = 10f;
	float attackWalkSpeed = 0.5f;
	float patrolWalkSpeed = 2f;

	GameObject fieldOfVision;
	public GameObject target;
	public EnemyState state;
	List<Vector3> patrolWaypoints;
	System.Random random;

	// Use this for initialization
	public override void Start () {
		base.Start();
		random = new System.Random();
		state = EnemyState.Patrol;

		patrolWaypoints = new List<Vector3>();

		foreach(Transform waypoint in patrolWaypointsObject.transform){
			patrolWaypoints.Add(waypoint.position);
		}
		
		agent.SetDestination(patrolWaypoints[1]);
	}
	
	// Update is called once per frame
	public override void Update () {		
		switch(state){
			case EnemyState.Hostile:
				Attack();
				break;
			case EnemyState.Patrol:
				Patrol();
				break;
		}

		SetAnimations();
	}

	void SetAnimations(){
		var walkSpeed = isAttacking ? attackWalkSpeed : patrolWalkSpeed;

		animator.SetBool("IsAttacking", isAttacking);
		animator.SetFloat("HSpeed", agent.velocity.magnitude * walkSpeed * (walkBackwards ? -1f : 1f));
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
	}

	void Idle(){
		state = EnemyState.Idle;
	}

	public void StartAttack(GameObject target){
		target = target;
		state = EnemyState.Hostile;
		agent.stoppingDistance = stopDistance;
	}

	public void StopAttack(GameObject target){
		target = null;
		state = EnemyState.Patrol;
		agent.stoppingDistance = 0f;
	}

	float fireRate = 0.5f;
	float nextFireTime;

	void Attack(){
		var distance = Vector3.Distance(target.transform.position, transform.position);
		walkBackwards = false;

		// move to attack position
		if(distance < attackRadius && distance >= stopDistance){
			agent.updateRotation = true;
			agent.SetDestination(target.transform.position);
		}
		else if(distance < stopDistance && distance >= retreatDistance){
			FaceTarget();
		}
		else if(distance < retreatDistance){
			walkBackwards = true;
			agent.updateRotation = false;
			var retreatPos = (transform.position - target.transform.position).normalized * stopDistance;
			agent.SetDestination(retreatPos);
			FaceTarget();
		}
		
		// perform attack
		if(distance < attackRadius && Time.time > nextFireTime){
			isAttacking = true;
			nextFireTime = Time.time + fireRate;
			playerWeapon.GetComponent<DefaultWeaponController>().ShootProjectile(currentAttackTarget);
		}
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

	#region animation events

	void StopIdle(){
		state = EnemyState.Patrol;
	}
	
	#endregion

	#region debug

	#endregion
}