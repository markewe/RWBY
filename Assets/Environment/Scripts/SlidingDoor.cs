using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehavior, IInteractionListener {

	[SerializeField]
	float doorCloseTimer;
	float doorCloseTimeout;
	Animator animator;
	DoorState state;

	void Start(){
		state = DoorState.Closed;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(state == DoorState.Open
			&& doorCloseTimeout.time > doorCloseTimer){
			Close();
		}
	}

	#region IInteractionListener

	public void OnInteraction(){
		if(state == DoorState.Closed){
			animator.SetBool("Open", true);
			doorCloseTimeout = Time.time + doorCloseTimer;
		}
		else if(state == DoorState.Open){
			animator.SetBool("Open", false);
		}
	}

	#endregion

	#region animation events

	public void SetState(string newState){
		state = (DoorState)DoorState.Parse(typeof(DoorState), newState);
	}

	#endregion
}
