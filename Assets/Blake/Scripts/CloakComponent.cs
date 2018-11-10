using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakComponent : MonoBehaviour {

	float decloakTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > decloakTime){
			Decloak();	
		}
	}

	public void Cloak(){
		//set material to cloaked material and player status to undetectable
		foreach (Transform child in transform)
		{
			var	rend = child.gameObject.GetComponent<Renderer>();

			if(rend != null){
				rend.material = Resources.Load("Characters/Blake/Materials/Cloaked/" + rend.material.name.Replace(" (Instance)", "") , typeof(Material)) as Material;
			}
		}
	}

	void Decloak(){

	}
}
