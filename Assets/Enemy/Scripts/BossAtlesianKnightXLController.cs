using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAtlesianKnightXLController : Monobehavior
	, IHealthListener, IShieldListener, IHitboxListener {

	[SeriazlizeField]
	float attackRadius
	[SeriazlizeField]
	float attackRate;
	[SeriazlizeField]
	float turretModeFireRateMultiplier;
	[SeriazlizeField]
	float turretModeShieldMultiplier;

	bool inTurretMode = false;
	
	float nextTurretModeTime;
	float turnSmooth = 2f;
	float turretModeTimer = 10f;
	float turretModeTimeout;
	float turretModeTurnSmooth = 10f;
	GameObject currentTarget;

	Animator animator;
	HealthHandler healthHandler;
	NavMeshAgent agent;
	Random random;
	ShieldHandler shieldHandler;

	void OnEnable(){
		nextTurretModeTime = GetNextTurretModetime();
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		agent = GetComponent<Animator>();
		healthHandler = GetComponent<HealthHandler>();
		shieldHandler = GetComponent<ShieldHandler>();
		random = new System.Random();

		agent.stopDistance = attackRadius;
		nextTurretModeTime = 0f;
		turretModeTimeout = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		Attack();

		// randomly enter turret mode
		if(!inTurretMode && Time.time > nextTurretModeTime){
			TurretModeEnter();
		}
		else if(inTurretMode && Time.time > turretModeTimeout){
			TurretModeExit();
		}

		SetAnimation();
	}

	void SetAnimations(){
		 animator.setBool("inTurretMode", inTurretMode);
	}

	float GetNextTurretModetime(){
		// every 10-20s
		nextTurretModeTime = Time.time + (10f * System.Random.Range(1f, 2f));
	}

	void Attack(){
		var distance = Vector3.Distance(currentTarget.transform.position, transform.position);

		// move to attack position
		if(distance > attackRadius){
			agent.updateRotation = true;
			agent.SetDestination(currentTarget.transform.position);
		}
		// attack if in safe range
		else {
			agent.updateRotation = false;
			FaceTarget();

			// perform attack
			if(Time.time > nextAttackTime){
				isAttacking = true;
				nextAttackTime = Time.time + attackRate;
				weapon.GetComponent<RangedWeaponController>().Attack(currentTarget);
			}
		}
	}

	void TurretModeEnter(){
		// deploy shield
		shieldHandler.MultiplyCurrentShield(turretModeShieldMultiplier);

		// increase fire rate
		attackRate *= turretModeFireRateMultiplier;

		turretModeTimeout = Time.time + turretModeTimer;

		inTurretMode = true;
	}

	void TurretModeExit(){
		shieldHandler.DivideCurrentShield(turretModeShieldMultiplier);
		attackRate /= turretModeFireRateMultiplier;
		nextTurretModeTime = GetNextTurretModetime();
		inTurretMode = false;
	}

	void FaceTarget(){
		var curTurnSmooth = inTurretMode ? turretModeTurnSmooth : turnSmooth;
		var direction = (currentTarget.transform.position - transform.position).normalized;
		var lookRot = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

		transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * curTurnSmooth);
	}

	bool AttackIsBlockable(WeaponHitbox hitbox){
		var attackDir = hitbox.gameObject.transform.position - transform.position;

		if(Vector3.Dot(transform.forward, attackDir) < 0){
			return true;
		}

		return false;
	}

	#region IHealthListener

	public void OnTakeDamage(){
		//agent.enabled = false;
		//animator.SetBool("IsHit", true);
	}

	public void OnHealDamage(){}

	public void OnZeroHealth(){
		agent.enabled = false;
		animator.SetBool("IsKnockedOut", true);
	}

	#endregion

	#region IShieldListener

	public void OnShieldBroken(){
		animator.SetBool("ShieldBroken", true);
	}

	public void OnShieldActiveHit(){}

	public void OnShieldInactiveHit(float hitAmount){
		healthHandler.TakeDamage(hitAmount);
	}

	public void OnShieldRecharge(){
		// knockback player when shields recharge
		animator.SetBool("ShieldBroken", false);
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

}

