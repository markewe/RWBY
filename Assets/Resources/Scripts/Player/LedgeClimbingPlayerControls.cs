using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeClimbingPlayerControls : PlayerControls
{
	public RaycastHit ledgeInfo;

    public override void HandleInputs(PlayerInputs playerInputs){}

	bool isJumpingToLedge = false;
	Vector3 wallNormal;

	public override void OnEnable(){
		base.OnEnable();
		PositionPlayer();
	}

	public override void Translate(){
		if(isJumpingToLedge){
			transform.position = Vector3.Slerp(
				transform.position
				, new Vector3(transform.position.x, ledgeInfo.point.y - characterController.height - .2f, transform.position.z)
				,  Time.deltaTime * 10f
			);	
		}
	}

	public override void Rotate(){
		// rotate to facing wall
		//transform.forward = wallNormal * -1f;
		// transform.forward = transform.position = Vector3.Slerp(
		// 	transform.rotation.eulerAngles
		// 	, wallNormal * -1f
		// 	, Time.deltaTime * 10f
		// );	
		
		//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(wallNormal.x, wallNormal.y, wallNormal.z),  Time.deltaTime * 10f);
	}

	public override void SetAnimationParams(){
		// determine height of ledge
		var ledgeHeight = ledgeInfo.point.y - transform.position.y;
		
		// if height greater than player, jump to it
		if(ledgeHeight > characterController.height){
			//animator.applyRootMotion = true;
			characterController.detectCollisions = false;
			animator.SetBool("isClimbingBoxUp", true);
		}
		// if height between waist and head, climb box without jumping
		else if(ledgeHeight <= characterController.height && ledgeHeight > characterController.height / 2f){
			
		}
		// if height below waist, small climb
		else if(ledgeHeight <= characterController.height / 2f){

		}

		//print()
	}

	public override void PostEvents(){}

	public override void ExitControls(){
		animator.applyRootMotion = false;
		characterController.detectCollisions = true;
		animator.SetBool("isClimbingBoxUp", false);
		GetComponent<PlayerInputHandler>().RestoreDefaultControls();
	}

	void PositionPlayer(){
		RaycastHit hit;
		var layerMask = 1 << (int)Layers.Environment;
		Physics.Raycast(transform.position, transform.forward, out hit, 1f, layerMask);

		wallNormal = Vector3.Dot(transform.forward, hit.normal) >= 0f ? hit.normal : hit.normal * -1f;

		print(wallNormal);

		transform.forward = wallNormal;
	}

	void IsJumpingToLedgeBegin(){
		isJumpingToLedge = true;
	}

	void IsJumpingToLedgeEnd(){
		isJumpingToLedge = false;
	}

	void ApplyRootMotion(){
		animator.applyRootMotion = true;
	}
}