using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneShadowController : ACloneController {

	[SerializeField]
	GameObject movingShadowClone;

	#region ACloneController

	public override void Update(){
		base.Update();

		Despawn();
	}

	public override void Despawn(){
		// create 4 clones that shoot in different directions
		CreateClones();

		// change player to cloaked status
		CloakPlayer();

		base.Despawn();
	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		Despawn();
	}

	#endregion

	#region helper functions

	void CreateClones(){
		var cloneForward = Instantiate(movingShadowClone, transform.position, transform.rotation);
		cloneForward.GetComponent<CloneShadowMoveController>().direction = Vector3.forward;

		var cloneBackward = Instantiate(movingShadowClone, transform.position, transform.rotation);
		cloneBackward.GetComponent<CloneShadowMoveController>().direction = Vector3.back;
		
		var cloneLeft = Instantiate(movingShadowClone, transform.position, transform.rotation);
		cloneLeft.GetComponent<CloneShadowMoveController>().direction = Vector3.left;

		var cloneRight = Instantiate(movingShadowClone, transform.position, transform.rotation);
		cloneRight.GetComponent<CloneShadowMoveController>().direction = Vector3.right;
	}
	
	void CloakPlayer(){
		//TBD
	}

	#endregion
}
