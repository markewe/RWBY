using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeaponController : AWeaponController {
	[SerializeField]
	float projectileSpeed;
	[SerializeField]
	GameObject fxObject;
	[SerializeField]
	GameObject projectileObject;

	public override void Attack(GameObject target){
		var emitter = transform.Find("ProjectileEmitter");
		
		// play shooting FX
		//Instantiate(fxObject, emitter.position, emitter.rotation);

		// shoot projectile
		var projectile = Instantiate(projectileObject, emitter.transform.position, Quaternion.identity);
		projectile.GetComponent<Rigidbody>().velocity = emitter.transform.forward * projectileSpeed;
	}
}
