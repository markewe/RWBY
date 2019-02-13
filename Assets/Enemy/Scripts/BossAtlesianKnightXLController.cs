using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossAtlesianKnightXLController : MonoBehaviour
	, IHealthListener, IShieldListener, IHitboxListener {

	[SerializeField]
	float attackRadius;
	[SerializeField]
	float attackRate;
	[SerializeField]
	float turretModeFireRateMultiplier;
	[SerializeField]
	float turretModeShieldMultiplier;

	bool throwGrenade = false;
	bool isAttacking = false;
	bool inTurretMode = false;
	float grenadeTimer = 10f;
	float nextAttackTime = 0f;
	float nextTurretModeTime = 0f;
	float nextGrenadeThrowTime = 0f;
	float turnSmooth = 2f;
	float turretModeTimer = 10f;
	float turretModeTimeout = 0f;
	float turretModeTurnSmooth = 10f;
	GameObject currentTarget;

	Animator animator;
	HealthHandler healthHandler;
	NavMeshAgent agent;
	ShieldHandler shieldHandler;

	void OnEnable(){
		nextTurretModeTime = GetNextTurretModeTime();
		nextGrenadeThrowTime = GetNextGrenadeThrowTime();
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		healthHandler = GetComponent<HealthHandler>();
		shieldHandler = GetComponent<ShieldHandler>();

		agent.stoppingDistance = attackRadius;
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

		SetAnimations();
	}

	void SetAnimations(){
		animator.SetFloat("HSpeed", agent.velocity);
		animator.SetBool("ThrowGrenade", throwGrenade);
		animator.SetBool("InTurretMode", inTurretMode);
	}

	float GetNextTurretModeTime(){
		// every 10-20s
		return Time.time + (10f * Random.Range(1f, 2f));
	}

	float GetNextGrenadeThrowTime(){
		return Time.time + (10f * Random.Range(1f, 2f));
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
				GetComponent<RangedWeaponController>().Attack(currentTarget);
			}

			// throw grenade
			if(!inTurretMode && Time.time > nextGrenadeThrowTime){
				throwGrenade = true;
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

	#region animation events

	void ThrowGrenade(){
		GetComponent<ThrownWeaponController>().Attack(currentTarget);
		throwGrenade = false;
	}

	#endregion

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

