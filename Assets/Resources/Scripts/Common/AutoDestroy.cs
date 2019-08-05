using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
    public float lifeSpan;

    float destroyTime;

	public void Start() 
     {
        if(lifeSpan == 0){
            lifeSpan = 3f;
        }

         destroyTime = Time.time + lifeSpan;
     }
 
     public void Update() 
     {
         if(Time.time > destroyTime){
			Destroy(this.gameObject);
		}
     }
}
