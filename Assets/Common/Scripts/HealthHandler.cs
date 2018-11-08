using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthHandler : MonoBehaviour {

	[SerializeField]
	private float hitPoints;
	Animator animator;
	NavMeshAgent agent;
	bool isDead;
	

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		CheckHealth();

		if(isDead){
			animator.SetBool("IsDead", isDead);
			agent.isStopped = true;
		}
	}

	void CheckHealth(){
		if(hitPoints <= 0){
			isDead = true;
		}
	}

	void AddHealth(float amount){
		hitPoints += amount;
	}

	public void Heal(float hitPointAmount){
		AddHealth(hitPointAmount);
	}

	public void TakeDamage(float hitPointAmount){
		AddHealth(hitPointAmount * - 1f);
	}
}
