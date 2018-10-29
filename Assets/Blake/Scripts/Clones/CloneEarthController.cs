using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneEarthController : ACloneController {

	public float moveSpeed;
	Vector3 targetPos;

	public override void Init(){
		base.Init();
		targetPos = transform.position;
	}

	public override void MovePlayer(){
		if(transform.position != targetPos){
			transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed);
		}
	}

	public override void RotatePlayer(){}

	public override void SetAnimations(){}

	public override void SetHitbox(){}

	#region animation events

	public void StopAttack(){

	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		if(transform.position == targetPos){
			targetPos = transform.position + new Vector3(0, 0, 1);
		}
	}

	#endregion

}
