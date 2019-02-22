using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectObjectController : MonoBehaviour, IUIListener {
	#region IUIListener

	public void OnUIExit(){
		GetComponent<PlayerControllerHandler>().ExitSpecialMovment("inspect");
	}

	#endregion
}
