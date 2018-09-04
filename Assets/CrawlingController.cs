using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingController : MonoBehaviour {

	#region character components
	Animator animator;
	CharacterController controller;
	Rigidbody rigidbody;
	Transform mainCameraT;
	#endregion
	Vector3 targetDirection;

	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		animator = GetComponent<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		mainCameraT = Camera.main.transform;
		targetDirection = new Vector3(0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		var inputX =  Input.GetAxisRaw("Horizontal");
		var inputZ = Input.GetAxisRaw("Vertical");


	}

	//float Closest
}
