using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthHandler : MonoBehaviour {

	[SerializeField]
	float hitPoints;

	public bool invulnerable = false;
	float currentHitPoints;

	IHealthListener healthListener;

	// Use this for initialization
	void Start () {
		healthListener = GetComponent<IHealthListener>();
		currentHitPoints = hitPoints;
	}

	void CheckHealth(){
		if(currentHitPoints <= 0){
			healthListener.OnZeroHealth();
		}
	}

	void AddHealth(float amount){
		currentHitPoints += amount;
		CheckHealth();
	}

	public void HealDamage(float hitPointAmount){
		AddHealth(hitPointAmount);
		healthListener.OnHealDamage();
	}

	public void TakeDamage(float hitPointAmount){
		if(!invulnerable){
			AddHealth(hitPointAmount * - 1f);
			healthListener.OnTakeDamage();
		}
	}
}
