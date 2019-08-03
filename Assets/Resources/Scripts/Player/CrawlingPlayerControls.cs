using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingPlayerControls : PlayerControls {

	public GameObject crawlTrigger;

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

	public override void OnEnable(){
		base.OnEnable();
		SetHitbox();
		PositionPlayer();
	}

	public void SetHitbox(){
		characterController.height = 0.5f;
		characterController.center = new Vector3(0f,  0.5f / 2f, 0f) + hitboxPosition;
	}

	public override void HandleInputs(PlayerInputs playerInputs){
		inputX = playerInputs.inputX;
		inputZ = playerInputs.inputZ;
		mouseX = playerInputs.mouseX * cameraTurnSpeed;
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
		animator.SetBool("isCrawling", true);
		animator.SetFloat("hSpeed", currentSpeed);
	}

	public override void PostEvents(){}

	public override void ExitControls(){
		animator.SetBool("isCrawling", false);
	}

	void OnTriggerExit(Collider col){
		if(!CheckIfCrawlSpace()){
			//GetComponent<PlayerInputHandler>().RestoreDefaultControls();
		}
	}
		
	bool CheckIfCrawlSpace(){
		var inCrawlSpace = false;
		RaycastHit hit;
		var layers = 1 << (int)GameLayers.Environment;

		// check if enough space all around character to stand
		inCrawlSpace = Physics.Raycast(transform.position, Vector3.up , out hit, 1.8f, layers);

		//print(inCrawlSpace);
		
		return inCrawlSpace;
	}

	void PositionPlayer(){
		// move player to in front of trigger (inside crawl space)
		transform.rotation = crawlTrigger.transform.rotation;
		print("NEW POSTION");
		print(crawlTrigger.transform.position + (crawlTrigger.transform.forward * characterController.radius * 2f));
		//transform.position = crawlTrigger.transform.position + (crawlTrigger.transform.forward * characterController.radius * 2f);
	}

	#region debugging
	void OnDrawGizmos() {

    }

	#endregion
}