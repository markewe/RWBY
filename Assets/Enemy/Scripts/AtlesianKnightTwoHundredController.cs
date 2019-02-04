using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtlesianKnightTwoHundredController : AEnemyController {

	[SerializeField]
	GameObject patrolWaypointsObject;
	[SerializeField]
	GameObject weapon;
	
	public GameObject currentTarget;
	
	bool isAttacking = false;
	bool isIdling = false;
	bool isScanning = false;
	bool walkBackwards = false;
	bool targetWithinRange = false;
	float attackRadius = 10f;
	float attackWalkSpeed = 4f;
	float patrolWalkSpeed = 2f;
	float hostileTimeout = 0f;
	float hostileTimer = 5f;
	float retreatDistance = 3f;	
	float stopDistance = 7f;
	float stuckTimeout = 0f;
	float stuckTimer = 10f;
	float turnSmooth = 2f;
	
	EnemyState state = EnemyState.Patrol;
	List<Vector3> patrolWaypoints;
	System.Random random;

	// Use this for initialization
	public override void Start () {
		base.Start();
		random = new System.Random();

		// create patrol waypoint list
		patrolWaypoints = new List<Vector3>();

		foreach(Transform waypoint in patrolWaypointsObject.transform){
			patrolWaypoints.Add(waypoint.position);
		}

		StartPatrol();
	}
	
	// Update is called once per frame
	public override void Update () {		
		switch(state){
			case EnemyState.Hostile:
				if(currentTarget != null){
					AttackTarget();
				}
				else if(Time.time <= hostileTimeout) {
					//ScanForTarget();
				}
				else if(Time.time > hostileTimeout){
					//StartPatrol();
				}
				break;
			case EnemyState.Patrol:
				Patrol();
				break;
		}

		CheckIfStuck();
		SetAnimations();
	}

	void SetAnimations(){
		var walkSpeed = isAttacking ? attackWalkSpeed : patrolWalkSpeed;

		var forwardSpeed = Vector3.Dot(agent.velocity, transform.forward);
		var rightSpeed = Vector3.Dot(agent.velocity, transform.right);

		animator.SetBool("IsIdling", state == EnemyState.Idle);
		animator.SetBool("IsAttacking", isAttacking);
		animator.SetFloat("ForwardSpeed", forwardSpeed);
		animator.SetFloat("RightSpeed", rightSpeed);
	}

	// if not moving for X seconds, something's wrong
	void CheckIfStuck(){
		if(agent.velocity.magnitude > 0.1f){
			stuckTimeout = Time.time + stuckTimer;
		}
		else if(Time.time > stuckTimeout){
			LostTarget();
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
	}

	void Idle(){
		state = EnemyState.Idle;
	}

	public void LostTarget(){
		currentTarget = null;
		hostileTimeout = Time.time + hostileTimer;
	}

	public void StartAttack(GameObject newTarget){
		print("startattack");
		currentTarget = newTarget;
		state = EnemyState.Hostile;
		agent.stoppingDistance = stopDistance;
		agent.speed = attackWalkSpeed;
	}

	public void StopAttack(){
		currentTarget = null;
		StartPatrol();
	}

	public override void TargetEnteredFieldOfVision(GameObject newTarget){
		if(currentTarget == null && CanSeeTarget(newTarget)){
			StartAttack(newTarget);
		}
	}

	public override void TargetExitedFieldOfVision(GameObject newTarget){
		if(currentTarget != null){

		}
	}

	bool CanSeeTarget(GameObject newTarget){
        var rayDir = newTarget.transform.position - transform.position;
        RaycastHit hit;
		var layerMask = (1 << LayerMask.NameToLayer("Enemies")) 
			| (1 << LayerMask.NameToLayer("Enemy Field of Vision"))
			| (1 << LayerMask.NameToLayer("Invisible Triggers"));
		layerMask = ~layerMask;

        Physics.Raycast(transform.position, rayDir, out hit, rayDir.magnitude, layerMask);

		// make sure nothing is obstructing view to target
		return hit.collider == null 
			&& (newTarget.CompareTag("Player") || newTarget.CompareTag("Clone"));
    }

	void StartPatrol(){
		state = EnemyState.Patrol;
		agent.stoppingDistance = 0f;
		agent.speed = patrolWalkSpeed;
	}

	float attackRate = 0.5f;
	float nextAttackTime = 0f;

	void AttackTarget(){
		var distance = Vector3.Distance(currentTarget.transform.position, transform.position);
		walkBackwards = false;

		// move to attack position
		if(distance < attackRadius && distance >= stopDistance){
			agent.updateRotation = true;
			agent.SetDestination(currentTarget.transform.position);
		}
		// attack if in safe range
		else if(distance < stopDistance && distance >= retreatDistance){
			FaceTarget();
		}
		// retreat if player too close
		else if(distance < retreatDistance){
			walkBackwards = true;
			agent.updateRotation = false;
			var retreatPos = (transform.position - currentTarget.transform.position).normalized * stopDistance;
			agent.SetDestination(retreatPos);
			FaceTarget();
		}
		
		// perform attack
		if(distance < attackRadius && Time.time > nextAttackTime){
			isAttacking = true;
			nextAttackTime = Time.time + attackRate;
			weapon.GetComponent<RangedWeaponController>().Attack(currentTarget);
		}
	}

	void ScanForTarget(){
		isScanning = true;
		agent.isStopped = true;
		agent.updateRotation = true;
	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag.Equals("PlayerWeapon")){
			print("helpme");
		}
	}

	void FaceTarget(){
		var direction = (currentTarget.transform.position - transform.position).normalized;
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSmooth);
	}

	

	#region animation events

	void StopIdle(){
		StartPatrol();
	}

	void OnShieldHitStunEnd(){
		HitStunEnd();
	}
	
	#endregion

	#region debug

	#endregion
}
