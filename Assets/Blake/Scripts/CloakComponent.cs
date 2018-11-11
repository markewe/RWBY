using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakComponent : MonoBehaviour {

	public string cloakedMaterialPath;
	bool isCloaked;
	float decloakTime;
	Dictionary<string, Material[]> defaultMaterials;

	// Use this for initialization
	void Start () {
		// save default materials for later
		defaultMaterials = new Dictionary<string, Material[]>();

		foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			if(rend != null){
				defaultMaterials.Add(rend.gameObject.name, rend.materials);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(isCloaked && Time.time > decloakTime){
			Decloak();	
		}
	}

	public void Cloak(){
		//set material to cloaked material and set status to undetectable
		foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			if(rend != null){
				var materialsCopy = rend.materials;

				for(var i=0; i<rend.materials.Length; i++){
					var mat = Resources.Load(cloakedMaterialPath + rend.materials[i].name.Replace(" (Instance)", ""), typeof(Material)) as Material;

					if(mat != null){
						materialsCopy[i] = mat;
					}
				}
				
				rend.materials = materialsCopy;
			}
		}

		isCloaked = true;
		decloakTime = Time.time + GetDecloakTime();
	}

	void Decloak(){
		// reaaply default materials and set status to detectable
		foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>())
		{
			if(rend != null){
				rend.materials = defaultMaterials[rend.gameObject.name];
			}
		}

		isCloaked = false;
	}

	float GetDecloakTime(){
		// gets decloak time based on talents and other game factors
		//TBD

		return 3f;
	}
}
