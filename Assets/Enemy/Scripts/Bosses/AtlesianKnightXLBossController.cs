using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlesianKnightXLBossController : MonoBehaviour, IHealthListener, IShieldListener {

	[SeriazlizeField]
	float fireRate;
	[SeriazlizeField]
	float turretModeFireRateMultiplier;
	[SeriazlizeField]
	float turretModeShieldMultiplier;

	Animator animator;
	HealthHandler healthHandler;
	NavMeshAgent navMeshAgent;
	Random random;
	ShieldHandler shieldHandler;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// enter turret mode
		// deploy shield
		// increase fire rate
		// knockback player when shields recharge
	}

}
