using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeaponController : AWeaponController {
	float bulletSpeed = 50f;

	public void ShootProjectile(GameObject target){
		var emitter = GameObject.Find("ProjectileEmitter");
		var fireDirection = target != null 
			? (target.transform.position - emitter.transform.position).normalized : emitter.transform.forward;

		fireDirection.y = 0;
		
		var projectile = Instantiate(GameObject.Find("Bullet"), emitter.transform.position, Quaternion.identity);
		projectile.GetComponent<Rigidbody>().velocity = fireDirection * bulletSpeed;
	}
}
