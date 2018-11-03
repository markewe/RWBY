using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneIceController : ACloneController {

	public float explosionRadius;

	#region ACloneController

	public override void MovePlayer(){
		// if(transform.position != targetPos){
		// 	transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed);
		// }
	}

	public override void RotatePlayer(){}

	public override void SetAnimations(){}

	public override void SetHitbox(){}

	public override void DestroyPostCallback(){
		
	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		// destroy the game object
		Destroy(this.gameObject);

		// create ice explosion

		// freeze objects in explosion
		FreezeObjects();
	}

	#endregion

	#region helper functions

	void FreezeObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (var collider in hitColliders)
        {
			var iFreeze = collider.gameObject.GetComponent<IFreezableObject>();

			if(iFreeze != null){
				iFreeze.Freeze();
			}
        }
    }
	
	#endregion
}
