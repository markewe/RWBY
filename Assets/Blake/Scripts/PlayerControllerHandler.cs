using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerHandler : MonoBehaviour {
	bool inSpecialMovement = false;
	GameObject specialMovementTrigger;

	// Use this for initialization
	void Start () {
		this.GetComponent<PlayerController>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		var inputInteract = Input.GetButton("Interact");

		if(!inSpecialMovement && inputInteract && specialMovementTrigger != null){
			var pc = this.GetComponent<PlayerController>();
			pc.enabled = false;

			var con = this.GetComponent<CrawlingController>();
			con.specialMovementTrigger = specialMovementTrigger;
			con.enabled = true;

			inSpecialMovement = true;
		}
	}

	public void ExitSpecialMovment(){
		if(inSpecialMovement){
			var con = this.GetComponent<CrawlingController>();
			con.specialMovementTrigger = specialMovementTrigger;
			con.enabled = false;

			var pc = this.GetComponent<PlayerController>();
			pc.enabled = true;

			inSpecialMovement = false;
		}
	}

	void OnTriggerEnter(Collider col){
		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = col.gameObject;
		}
	}

	void OnTriggerExit(Collider col){
		if(col.tag.Equals("SpecialMovementTrigger")){
			specialMovementTrigger = null;
		}
	}
}
