using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneEarthController : ACloneController {

	public GameObject brokenVersion;

	#region ACloneController functions

	public override void Init(){
		base.Init();
	}

	public override void MovePlayer(){
		
	}

	public override void RotatePlayer(){}

	public override void SetAnimations(){}

	public override void SetHitbox(){}

	public override void DestroyPostCallback(){
		Instantiate(brokenVersion, transform.position, transform.rotation);
	}

	#endregion

	#region animation events

	public void StopAttack(){

	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		
	}

	#endregion

}
