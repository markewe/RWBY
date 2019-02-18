using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IInteractionListener {

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
			&& Time.time > doorCloseTimer){
			Close();
		}
	}

	#region IDoor

	public void Open(){
		animator.SetBool("Open", true);
		doorCloseTimeout = Time.time + doorCloseTimer;
	}

	public void Close(){
		animator.SetBool("Open", false);
	}

	#endregion

	#region IInteractionListener

	public void OnInteract(){
		if(state == DoorState.Closed){
			Open();
		}
		else if(state == DoorState.Open){
			Close();
		}
	}

	#endregion

	#region animation events

	public void SetState(string newState){
		state = (DoorState)DoorState.Parse(typeof(DoorState), newState);
	}

	#endregion
}
