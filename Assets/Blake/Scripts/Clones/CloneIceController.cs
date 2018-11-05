using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneIceController : ACloneController {

	public float explosionRadius;
	public GameObject explosion;

	GameObject explosionClone;

	#region ACloneController

	public override void Despawn(){
		// create ice explosion
		CreateIceExplosion();

		// freeze objects in explosion
		FreezeObjects();

		base.Despawn();
	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		Despawn();
	}

	#endregion

	#region helper functions

	void CreateIceExplosion(){
		explosionClone = Instantiate(explosion, transform.position, transform.rotation);
	}

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

