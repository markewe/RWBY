using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {

	public DoorType doorType;
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
		if(doorCloseTimer > 0
			&& state == DoorState.Open
			&& Time.time > doorCloseTimer){
			Close();
		}
	}

	void Open(){
		print("I'm OPENING");
		state = DoorState.Opening;
		animator.SetBool("open", true);
		doorCloseTimeout = Time.time + doorCloseTimer;
	}

	void Close(){
		print("I'm CLOSING");
		state = DoorState.Closing;
		animator.SetBool("open", false);
	}

	#region IInteractionListener

	public void OnInteract(){
		print("INTERACTING");

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
		print(newState);
		state = (DoorState)DoorState.Parse(typeof(DoorState), newState);
	}

	#endregion
}
