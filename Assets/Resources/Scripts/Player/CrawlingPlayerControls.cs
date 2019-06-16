﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingPlayerControls : PlayerControls {

	Vector3 targetDirection;
	Vector3 hitboxPosition = new Vector3(0f, 0f, 0.5f);
	float currentSpeed;
	float inputX;
	float inputZ;
	float mouseX;
	float speedSmoothVelocity;
	float cameraTurnSpeed = 1f;
	static float crawlSpeedSmoothTime = 0.1f;
	static float crawlSpeed = 2f;

	public void SetHitbox(){
		characterController.height = 0.5f;
		characterController.center = new Vector3(0f,  0.5f / 2f, 0f) + hitboxPosition;
	}

	public override void HandleInputs(PlayerInputs playerInputs){
		inputX = Input.GetAxis("Horizontal");
		inputZ = Input.GetAxis("Vertical");
		mouseX = Input.GetAxis("Mouse X") * cameraTurnSpeed;
	}

	public override void Translate(){
		targetDirection = new Vector3(inputX, 0f, inputZ);

		var targetSpeed = crawlSpeed * targetDirection.magnitude;
		var smoothTime = crawlSpeedSmoothTime;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
		Vector3 vel = (transform.forward * currentSpeed * inputZ) + (transform.right * currentSpeed * inputX);

		characterController.Move(vel * Time.deltaTime);
	}

	public override void Rotate(){
		var rotation = transform.rotation.eulerAngles;

		rotation.y += mouseX;
		transform.rotation = Quaternion.Euler(rotation);
	}

	public override void SetAnimationParams(){
		animator.SetBool("IsCrawling", true);
		animator.SetFloat("HSpeed", currentSpeed);
	}

	

	public override void PostEvents(){}

public override void ExitControls(){
	
}

	void OnTriggerEnter(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			// if(col.GetComponent<SpecialMovementTriggers>().movementType.Equals("crawl")){
				
			// }
		}
	}

	void OnTriggerExit(Collider col){
		if(this.enabled && col.tag.Equals("SpecialMovementTrigger")){
			// if(col.GetComponent<SpecialMovementTriggers>().movementType.Equals("crawl") && !CheckIfCrawlSpace()){
			// 	animator.SetBool("IsCrawling", false);
			// 	GetComponent<PlayerControllerHandler>().ExitSpecialMovment("crawl");
			// }
		}
	}
		
	bool CheckIfCrawlSpace(){
		var inCrawlSpace = false;
		RaycastHit hit;
		var layers = 1 << 9; 
		layers = ~layers; // ignore player

		// check if enough space all around character to stand
		inCrawlSpace = Physics.Raycast(transform.position, Vector3.up , out hit, 1.7f, layers);
		
		return inCrawlSpace;
	}

	#region debugging
	void OnDrawGizmos() {

    }

	#endregion
}