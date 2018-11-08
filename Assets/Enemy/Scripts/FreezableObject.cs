using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FreezableObject : MonoBehaviour {

	bool isFrozen;
	List<Material> defaultMaterials;
	float freezeTimer = 3f;
	float unFreezeTime;

	Animator animator;
	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		defaultMaterials = new List<Material>();
		isFrozen = false;

		// save the default material for later
		foreach (Transform child in transform)
		{
			var	rend = child.gameObject.GetComponent<Renderer>();

			if(rend != null){
				defaultMaterials.Add(rend.material);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isFrozen && Time.time > unFreezeTime){
			var i=0;

			foreach (Transform child in transform)
			{
				var	rend = child.gameObject.GetComponent<Renderer>();

				if(rend != null){
					rend.material = defaultMaterials[i];
				}
				
				i++;
			}

			if(animator != null){
				AnimatorResume();
			}

			if(agent != null){
				AgentResume();
			}

			isFrozen = false;
		}
	}

	public void Freeze(){
		// assign the frozen material		
		foreach (Transform child in transform)
		{
			var	rend = child.gameObject.GetComponent<Renderer>();

			if(rend != null){
				rend.material = Resources.Load("Ice", typeof(Material)) as Material;
			}
		}

		// freeze animation and movement
		if(animator != null){
			AnimatorPause();
		}

		if(agent != null){
			AgentPause();
		}

		unFreezeTime = Time.time + freezeTimer;
		isFrozen = true;
	}

	Vector3 lastAgentVelocity;
	NavMeshPath lastAgentPath;

	void AgentPause() {
		lastAgentVelocity = agent.velocity;
		agent.enabled = false;
     }

	 void AgentResume() {
		agent.enabled = true;
		agent.velocity = lastAgentVelocity;
     }
     
	 void AnimatorPause(){
		animator.speed = 0;
		animator.enabled = false;
	 }

	 void AnimatorResume(){
		animator.enabled = true;
		animator.speed = 1;
	 }
}
