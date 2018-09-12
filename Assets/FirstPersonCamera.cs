using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour {

	SphereCollider collider;
	public GameObject target;

	// Use this for initialization
	void Init(){
		collider = GetComponent<SphereCollider>();
		collider.enabled = false;
		transform.parent = target.GetComponent<CharacterController>().transform;
		transform.rotation = target.transform.rotation;//Quaternion.identity;
		transform.localPosition = new Vector3(0f, target.GetComponent<CharacterController>().height, 0f);
	}

	void OnEnable()
    {
        Init();
    }

	void Awake(){
		Init();
	}

	void Start () {
		Init();
	}

	static float cameraTurnSpeed = 1f;
	
	// Update is called once per frame
	void LateUpdate () {
		// float mouseX = Input.GetAxis("Mouse X") * cameraTurnSpeed;
		// float mouseY = Input.GetAxis("Mouse Y") * cameraTurnSpeed;

		// var rotation = transform.rotation.eulerAngles;

		// rotation.x += mouseY;
		// rotation.y += mouseX;

		// transform.rotation = Quaternion.Euler(rotation);
	}
}
