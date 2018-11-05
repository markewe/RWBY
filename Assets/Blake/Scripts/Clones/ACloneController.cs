using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACloneController : MonoBehaviour {
	
	#region variables

	float despawnTime;
	float hSpeed;
	public float lifeSpan;

	#endregion

	public virtual void Init(){
		despawnTime = Time.time + lifeSpan;
	}

	#region Monobehavior functions

	 public virtual void OnEnable(){
		Init();
	}

	public virtual void Awake () {
		Init();
	}
	public virtual void Start () {
		Init();
	}

	public virtual void Update(){
		if(lifeSpan > 0 && Time.time > despawnTime){
			Despawn();
		}
	}

	public virtual void Despawn(){
		Destroy(this.gameObject);
	}

	#endregion

	#region debugging

	#endregion
}
