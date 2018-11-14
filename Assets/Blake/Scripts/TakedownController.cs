using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakedownController : APlayerController {
	public GameObject enemy;

	public override void Init(){
		base.Init();
		Physics.IgnoreCollision(GetComponent<Collider>(), enemy.GetComponent<Collider>());

	}

	public override void HandleInputs(){
	}

	public override void MovePlayer(){
	}

	public override void RotatePlayer(){
	}

	public override void SetAnimations(){
		var rot = enemy.transform.rotation.eulerAngles;
				
		rot = new Vector3(rot.x,rot.y+180,rot.z);
 		transform.rotation = Quaternion.Euler(rot);
 		transform.position = enemy.transform.position + (enemy.transform.forward * 1f);
		animator.applyRootMotion = true;
		animator.SetBool("IsInTakedown", true);
		enemy.GetComponent<AEnemyController>().PerformTakedown();
}

	public override void SetHitbox(){
	}

	public override void PostEvents(){
	}
}
