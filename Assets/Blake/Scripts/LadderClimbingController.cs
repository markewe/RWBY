using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LadderClimbingController : APlayerController {

	public GameObject ladder;
	float currentSpeed;
	float climbSpeed = 2f;
	float climbSpeedSmoothTime = 0.1f;
	float inputZ;
	float speedSmoothVelocity;
	
	public override void HandleInputs(){
		inputZ = Input.GetAxisRaw("Vertical");
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
		animator.SetInteger("InputZRaw", int.Parse(inputZ.ToString()));
	}

	void ClimbLadderStart(){
		controller.detectCollisions = false;
		animator.applyRootMotion = true;
	}

	void ClimbLadderEnd(){
		animator.SetBool("IsClimbingLadder", false);
		controller.detectCollisions = true;
		animator.applyRootMotion = false;
	}
}
