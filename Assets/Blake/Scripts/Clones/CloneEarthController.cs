using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneEarthController : ACloneController {

	public GameObject brokenVersion;

	#region ACloneController functions

	public override void Init(){
		base.Init();
	}

	public override void Despawn(){
		Instantiate(brokenVersion, transform.position, transform.rotation);
		base.Despawn();
	}

	#endregion

	#region collision events

	void OnTriggerEnter(Collider col){
		
	}

	#endregion

}
