using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

	public static float lookRadius = 7f;
	public static float stopDistance = 5f;
	public static float turnSmooth = 2f;
	public static float retreatDistance = 3f;	

	bool isAttacking;

	Transform target;
	NavMeshAgent agent;
	Animator animator;

	// Use this for initialization
	void Start () {
		target = PlayerManager.instance.player.transform;
		agent = GetComponent<NavMeshAgent>();
		agent.stoppingDistance = stopDistance;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		var distance = Vector3.Distance(target.position, transform.position);
		var direction = 1f;

		if(distance < lookRadius && distance >= stopDistance){
			direction = 1f;
			isAttacking = true;
			agent.updateRotation = true;
			agent.SetDestination(target.position);
		}
		else if(distance < stopDistance && distance >= retreatDistance){
			FaceTarget();
		}
		else if(distance < retreatDistance ){
			direction = -1f;
			agent.updateRotation = false;
			var retreatPos = (transform.position - target.position).normalized * stopDistance;
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

	void StopAttack(){

	}

	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag.Equals("PlayerWeapon")){
			print("helpme");
		}
	}

	void FaceTarget(){
		var direction = (target.position - transform.position).normalized;
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSmooth);
	}
}
