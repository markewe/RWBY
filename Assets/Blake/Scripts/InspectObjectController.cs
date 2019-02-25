using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectObjectController : APlayerController, IUIListener {

	public override void HandleInputs(){
	}

	public override void MovePlayer(){
	}

	public override void RotatePlayer(){

	}

	public override void SetAnimations(){
		
	}

	public override void SetHitbox(){
	}

	public override void PostEvents(){
	}


	#region IUIListener

	public void OnUIExit(){
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("inspect");
	}

	#endregion
}
