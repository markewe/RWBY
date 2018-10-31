using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummyController : MonoBehaviour, IFreezableObject {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region IFreezableObject

	public void Freeze(){
		print("I freeze");
	}

	#endregion
}
