using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummyController : MonoBehaviour, IFreezableObject {

	List<Material> defaultMaterials;
	float freezeTimer = 3f;
	float unFreezeTime;

	// Use this for initialization
	void Start () {
		defaultMaterials = new List<Material>();

		// save the default material for later
		foreach (Transform child in transform)
		{
			var	rend = child.gameObject.GetComponent<Renderer>();

			if(rend != null){
				defaultMaterials.Add(rend.material);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > unFreezeTime){
			var i=0;

			foreach (Transform child in transform)
			{
				var	rend = child.gameObject.GetComponent<Renderer>();

				if(rend != null){
					rend.material = defaultMaterials[i];
				}
				
				i++;
			}
		}
	}

	#region IFreezableObject

	public void Freeze(){
		// assign the frozen material		
		foreach (Transform child in transform)
		{
			var	rend = child.gameObject.GetComponent<Renderer>();

			if(rend != null){
				rend.material = Resources.Load("Ice", typeof(Material)) as Material;
			}
			
		}

		unFreezeTime = Time.time + freezeTimer;
	}

	#endregion
}
