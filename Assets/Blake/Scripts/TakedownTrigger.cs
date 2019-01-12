using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakedownTrigger : MonoBehaviour {
	void OnTriggerEnter(Collider col){
		if(col.gameObject.tag.Equals("Enemy")){
			PlayerManager.instance.player.GetComponent<PlayerControllerHandler>().OnTakedownTriggerEnter(col.gameObject);
		}
		
	}

	void OnTriggerExit(Collider col){
		if(col.gameObject.tag.Equals("Enemy")){
			PlayerManager.instance.player.GetComponent<PlayerControllerHandler>().OnTakedownTriggerExit();
		}
	}
}
