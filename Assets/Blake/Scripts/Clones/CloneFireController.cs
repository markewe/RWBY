using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneFireController : ACloneController {

	[SerializeField]
	float explosionRadius;

	[SerializeField]
	GameObject explosion;
	GameObject explosionClone;

	#region ACloneController

	public override void Despawn(){
		// create explosion
		CreateExplosion();

		// damage objects in explosion
		DamageObjects();

		base.Despawn();
	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		Despawn();
	}

	#endregion

	#region helper functions

	void CreateExplosion(){
		explosionClone = Instantiate(explosion, transform.position, transform.rotation);
	}

	void DamageObjects()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (var collider in hitColliders)
        {
			var hpHandler = collider.gameObject.GetComponent<HealthHandler>();

			if(hpHandler != null){
				hpHandler.TakeDamage(100);
			}
        }
    }
	
	#endregion
}
