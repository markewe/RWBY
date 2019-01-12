using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {

	float despawnTime;
	public float lifeSpan;

	// Use this for initialization
	void Start () {
		if(lifeSpan == 0){
			lifeSpan = 3f;		
		}

		despawnTime = Time.time + lifeSpan;
	}
	
	// Update is called once per frame
	void Update () {
		if(lifeSpan > 0 && Time.time > despawnTime){
			Destroy(this.gameObject);
		}
	}
}
