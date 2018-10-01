using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LadderClimbingController : APlayerController {

	public GameObject ladder;
	GameObject ladderTrigger;
	bool nearEnd;
	float currentSpeed;
	float climbSpeed = 2f;
	float climbSpeedSmoothTime = 0.1f;
	float inputZ;
	float speedSmoothVelocity;
	
	public override void Init(){
		base.Init();
		nearEnd = false;
	}

	public override void Update(){
		base.Update();

	}

	public override void HandleInputs(){
		inputZ = Input.GetAxis("Vertical");
	}

	public override void MovePlayer(){
		var targetpos = new Vector3(ladder.transform.position.x, transform.position.y, ladder.transform.position.z);
		transform.position = Vector3.Slerp(transform.position, targetpos, Time.deltaTime * 10f);
	}

	public override void RotatePlayer(){
		// turn towards ladder
		transform.rotation = Quaternion.Slerp(transform.rotation, ladder.transform.rotation, Time.deltaTime * 10f);
	}

	public override void SetHitbox(){

	}

	public override void SetAnimations(){
		animator.SetBool("IsClimbingLadder", true);
		animator.SetBool("NearLadderEnd", nearEnd);
		animator.SetFloat("InputZ", inputZ);
		//animator.SetInteger("InputZ", int.Parse(inputZ.ToString()));
	}

	void OnTriggerEnter(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			var trigger = col.GetComponent<SpecialMovementTriggers>();

			if(trigger.movementType.Equals("ladder")){
				ladderTrigger = col.gameObject;				
				nearEnd = true;
			}
		}
	}

	void OnTriggerExit(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			var trigger = col.GetComponent<SpecialMovementTriggers>();
			if(trigger.movementType.Equals("ladder")){
				ladderTrigger = null;				
				nearEnd = false;
			}
		}
	}

	#region animation events

	void ClimbLadderStart(){
		animator.applyRootMotion = true;
	}

	void ClimbLadderEnd(){
		animator.SetBool("IsClimbingLadder", false);
		animator.applyRootMotion = false;
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("ladder");
	}

	#endregion
}
