using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultWeaponController : MonoBehaviour {
	float bulletSpeed = 50f;

	// Use this for initialization
	void Start () {
		//bulletSpeed = 5f;
	}
	
	// Update is called once per frame
	void Update () {
		//projectile.GetComponent<Rigidbody>().velocity = bulletDir * bulletSpeed;
	}

	public void ShootProjectile(GameObject target){
		if (target != null)
			print(target.name);
		var emitter = GameObject.Find("ProjectileEmitter");
		var fireDirection = target != null 
			? (target.transform.position - emitter.transform.position).normalized : emitter.transform.forward;

		fireDirection.y = 0;
		
		var projectile = Instantiate(GameObject.Find("Bullet"), emitter.transform.position, Quaternion.identity);
		projectile.GetComponent<Rigidbody>().velocity = fireDirection * bulletSpeed;
	}
}
