using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakedownController : APlayerController {
	public GameObject enemy;

	float turnSmooth = 4f;

	public override void OnEnable(){
		if(enemy != null){
			Physics.IgnoreCollision(GetComponent<Collider>(), enemy.GetComponent<Collider>());
			enemy.GetComponent<AEnemyController>().InitTakedown();
			animator.SetFloat("HSpeed", 0f);
			animator.SetFloat("VSpeed", 0f);
			animator.SetBool("IsInTakedown", true);
		}
	}

	public override void HandleInputs(){
	}

	public override void MovePlayer(){
	}

	public override void RotatePlayer(){
		// check if player and enemy are facing each other.
		if(Vector3.Dot(transform.forward, enemy.transform.forward) + 1f < 0.01f){
			animator.SetBool("PerformTakedown", true);
			enemy.GetComponent<AEnemyController>().PerformTakedown();
			animator.SetBool("IsInTakedown", false);
		}
		else{
			FaceObject(enemy, 20f);
		}
	}

	public override void SetAnimations(){
		
	}

	public override void SetHitbox(){
	}

	public override void PostEvents(){
	}

	#region animation events

	public void EndTakedown(){
		enemy.GetComponent<AEnemyController>().EndTakedown();
		animator.SetBool("PerformTakedown", false);
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("takedown");
	}

	#endregion
}

