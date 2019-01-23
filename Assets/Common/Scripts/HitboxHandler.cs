using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxHandler : MonoBehaviour {
	HealthHandler healthHandler;

	void Start(){
		healthHandler = GetComponent<HealthHandler>();
	}

	void OnTriggerEnter(Collider col){
		// only look for weapons coming from opposing sides with layer checking
		if(gameObject.layer != col.gameObject.layer
			&& col.gameObject.CompareTag("WeaponHitbox")){
			healthHandler.TakeDamage(col.gameObject.GetComponent<WeaponHitbox>().hitAmount);
		}
	}
}
