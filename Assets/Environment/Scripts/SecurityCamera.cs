using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour, IFieldOfVisionListener {

	[SerializeField]
	float alertRadius;
	[SerializeField]
	float alertTimer;
	[SerializeField]
	float observeAngle;
	[SerializeField]
	float observeSpeed;

	float observeDirection;
	float alertTimeout;
	EnemyState currentState;
	GameObject currentTarget;

	void Awake(){
		observeDirection = 1f;
		currentState = EnemyState.Patrol;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentState){
			case EnemyState.Patrol:
				Observe();
				break;
			case EnemyState.Alert:
				if(currentTarget != null){
					FaceTarget();
				}

				// after x seconds make enemies in an x radius target player and turn to hostile
				// if player exits field of vision of x seconds and not hostile, return to patrol
				if(Time.time > alertTimeout){
					if(currentTarget != null){
						currentState = EnemyState.Hostile;
					}	
					else{
						currentState = EnemyState.Patrol;
					}
				}
				break;
			case EnemyState.Hostile:
				AlertEnemies();
				break;
		}
	}

	void AlertEnemies(){
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, alertRadius);

		for(var i=0; i<hitColliders.Length; i++){
			var enemy = hitColliders[i].GetComponent<AEnemyController>();

			if(enemy != null){
				enemy.StartAttack(currentTarget);
			}
		}
	}

	void FaceTarget(){
		var direction = (currentTarget.transform.position - transform.position).normalized;
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = lookRot;
	}

	void Observe(){
		// scan back and forth in a x degree angle at x speed
		var lookRot = observeDirection > 0f 
			? Quaternion.Euler(0, 0, observeAngle) : Quaternion.identity;
		
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * observeSpeed);

		if(transform.rotation.z == 0 || transform.rotation.z == observeAngle){
			observeDirection *= -1f;
		}
	}

	#region IFieldOfVisionListener

	public void OnFieldOfVisionEnter(GameObject obj){
		// if spot player focus on player and turn to alert
		if(obj.CompareTag("player")){
			currentTarget = obj;

			if(currentState == EnemyState.Patrol){
				currentState = EnemyState.Alert;
				alertTimeout = Time.time + alertTimer;
			}	
		}
	}

	public void OnFieldOfVisionExit(GameObject obj){
		if(obj.CompareTag("player")){
			currentTarget = null;
		}
	}

	#endregion
}
