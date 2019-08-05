using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlesianKnightTwoHundredSController : AtlesianKnightTwoHundredController, IShieldListener {
	ShieldHandler shieldHandler;

	public override void Start(){
		base.Start();
		shieldHandler = GetComponent<ShieldHandler>();
	}
	
	#region 200S specific functions

	// 200S can block any attack that is in the front
	bool AttackIsBlockable(WeaponHitbox hitbox){
		var attackDir = hitbox.gameObject.transform.position - transform.position;

		if(Vector3.Dot(transform.forward, attackDir) < 0){
			return true;
		}

		return false;
	}

	#endregion

	#region IHitboxListener
	
	public override void OnWeaponHitboxEnter(WeaponHitbox hitbox){
		if(AttackIsBlockable(hitbox)){
			// melee causes no shield damage
			if(hitbox.isMelee){
				shieldHandler.OnShieldHit(0);
			}
			else{
				shieldHandler.OnShieldHit(hitbox.hitAmount);
			}
		}
		else{
			healthHandler.TakeDamage(hitbox.hitAmount);
		}
	}

	#endregion

	#region IShieldListener

	public void OnShieldBroken(){
		animator.SetBool("ShieldBroken", true);
	}

	public void OnShieldActiveHit(){
		agent.enabled = false;
		animator.SetBool("Block", true);
	}

	public void OnShieldInactiveHit(float hitAmount){
		healthHandler.TakeDamage(hitAmount);
	}

	public void OnShieldRecharge(){
		animator.SetBool("ShieldBroken", false);
	}

	#endregion

	#region animation events

	void BlockHitStunEnd(){
		agent.enabled = true;
	}

	#endregion
}
