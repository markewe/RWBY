using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : MonoBehaviour, IFieldOfVisionListener {

	[SerializeField]
	float observeAngle;
	[SerializeField]
	float observeRadius;
	[SerializeField]
	float observeSpeed;

	float observeDirection = 1f;
	EnemyState state = EnemyState.Patrol;
	GameObject currentTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		switch(state){
			case EnemyState.Patrol:
				Observe();
				break;
			case EnemyState.Alert:
				// after x seconds make enemies in an x radius target player and turn to hostile
				break;
			case EnemyState.Hostile:
				break;
		}
	}

	void Observe(){
		// scan back and forth in a x degree angle at x speed
		
	}

	

	#region IFieldOfVisionListener

	public void OnFieldOfVisionEnter(){
		// if spot player focus on player and turn to alert
	}

	public void OnFieldOfVisionExit(){
		// if player exits field of vision of x seconds and not hostile, return to patrol
	}

	#endregion
}
