using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpecialAttackController : APlayerController {
	public override void OnEnable(){
		base.OnEnable();

		// play special attack FX

		// play special attack cutscene camera
		var csTimeline = GetComponent<PlayableDirector>(); //Instantiate(GameObject.Find("Special Attack Timeline"), transform.position, transform.rotation) as PlayableDirector;
		csTimeline.Play();

		// play special attack animations
		animator.applyRootMotion = true;	
		animator.SetBool("InSpecialAttack", true);
	}

	public override void HandleInputs(){}

	public override void MovePlayer(){}

	public override void RotatePlayer(){}

	public override void SetAnimations(){}

	public override void SetHitbox(){}

	public override void PostEvents(){}

	#region animation events

	public void ExitSpecialAttack(){
		animator.applyRootMotion = false;
		animator.SetBool("InSpecialAttack", false);
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("specialattack");
	}

	public void Shoot(){}
	public void FootL(){}
	public void FootR(){}
	public void Hit(){}

	#endregion
}
