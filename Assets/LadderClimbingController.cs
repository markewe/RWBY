using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LadderClimbingController : MonoBehaviour {
	public GameObject ladder;
	#region character components
	Animator animator;
	CharacterController controller;
	Rigidbody rigidbody;
	Transform mainCameraT;
	#endregion
	float currentSpeed;
	float climbSpeed = 2f;
	float climbSpeedSmoothTime = 0.1f;
	float speedSmoothVelocity;

	// Use this for initialization
	void Init(){
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		mainCameraT = Camera.main.transform;
		SetHitbox();
	}

	void OnEnable(){
		Init();
	}

	void Start () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		var inputZ = Input.GetAxisRaw("Vertical");
		var targetDirection = new Vector3(0f, inputZ, 0f);

		// turn towards ladder
		transform.rotation = Quaternion.Slerp(transform.rotation, ladder.transform.rotation, Time.deltaTime * 10f);

		var targetpos = new Vector3(ladder.transform.position.x, transform.position.y, ladder.transform.position.z);
		transform.position = Vector3.Slerp(transform.position, targetpos, Time.deltaTime * 10f);

		animator.SetBool("IsClimbingLadder", true);
		animator.SetInteger("InputZRaw", int.Parse(inputZ.ToString()));
	}

	void SetHitbox(){

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
