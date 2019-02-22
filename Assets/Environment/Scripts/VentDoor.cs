using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentDoor : MonoBehaviour, IInteractionListener {

	Animator animator;
	DoorState state;

	void Start(){
		state = DoorState.Closed;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Open(){
		animator.SetBool("Open", true);
	}

	public void Close(){
		animator.SetBool("Open", false);
	}

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
