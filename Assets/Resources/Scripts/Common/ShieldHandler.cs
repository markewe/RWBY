using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHandler : MonoBehaviour {
	[SerializeField]
	float hitPoints;
	[SerializeField]
	float rechargeTime;

	bool isActive = true;
	float currentHitPoints;
	float rechargeTimeout = 0f;
	IShieldListener listener;

	void Start(){
		listener = GetComponent<IShieldListener>();
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

	public void MultiplyCurrentShield(float multiplier){
		currentHitPoints *= multiplier;
	}

	public void DivideCurrentShield(float multiplier){
		currentHitPoints /= multiplier;
	}

	public void OnShieldHit(float hitAmount){
		if(isActive){
			if(hitPoints - hitAmount < 0){
				listener.OnShieldBroken();
				isActive = false;
			}
			else{
				listener.OnShieldActiveHit();
			}

			hitPoints -= hitAmount;
			rechargeTimeout = Time.time + rechargeTime;
		}
		else{
			listener.OnShieldInactiveHit(hitAmount);
		}
	}
}
