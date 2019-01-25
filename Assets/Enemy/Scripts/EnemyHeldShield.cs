using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeldShield : MonoBehaviour {
	[SerializeField]
	float hitPoints;
	[SerializeField]
	float rechargeTime;

	bool isActive = true;
	float currentHitPoints;
	float rechargeTimeout = 0f;
	HealthHandler healthHandler;
	IEnemyHeldShieldListener listener;

	void Start(){
		listener = GetComponent<IEnemyHeldShieldListener>();
		currentHitPoints = hitPoints;
	}

	void Update(){
		// check if shield can recharge
		if(Time.time > rechargeTimeout){
			isActive = true;
			currentHitPoints = hitPoints;
			listener.OnShieldRecharge();
		}
	}

	void OnTriggerEnter(Collider col){
		if(gameObject.layer == col.gameObject.layer
			|| !col.gameObject.CompareTag("WeaponHitbox")){
			return;
		}

		var hitbox = GetComponent<WeaponHitbox>();

		// held shield takes no damage from melee
		if(hitbox != null && !hitbox.isMelee){
			// if hit takes hit points below zero, onshieldbroken
			if(isActive){
				if(hitPoints - hitbox.hitAmount < 0){
					listener.OnShieldBroken();
					isActive = false;
				}
				else{
					listener.OnShieldActiveHit();
				}

				hitPoints -= hitbox.hitAmount;
				rechargeTimeout = Time.time + rechargeTime;
			}
			else{
				listener.OnShieldInactiveHit();
			}
		}
	}
}
