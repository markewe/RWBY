using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxHandler : MonoBehaviour {
	IHitboxListener hitboxListener;

	void Start(){
		hitboxListener = GetComponent<IHitboxListener>();
	}

	void OnTriggerEnter(Collider col){
		// only look for weapons coming from opposing sides with layer checking
		if(gameObject.layer != col.gameObject.layer
			&& col.gameObject.CompareTag("WeaponHitbox")){
			hitboxListener.OnWeaponHitboxEnter(col.gameObject.GetComponent<WeaponHitbox>());
		}
	}
}
