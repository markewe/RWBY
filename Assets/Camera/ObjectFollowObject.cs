using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowObject : MonoBehaviour {

	public Transform target;
	static float smoothSpeed = 10f;
	static float cameraTurnSpeed = 2f;
	public Vector3 offset;
	public Transform lookAtTarget;

	// Use this for initialization
	void Init(){
		// pass through follow target
		Physics.IgnoreCollision(target.GetComponent<Collider>(), GetComponent<Collider>());
		transform.parent = null;
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
	
	// Update is called once per frame
	void LateUpdate () {
		//print(transform.parent);

		// add camera rotation to the offset
		offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * cameraTurnSpeed, Vector3.up) * offset;
		offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * cameraTurnSpeed, Vector3.right) * offset;

		// don't clip through things
		var desiredPos = target.position + offset;
		desiredPos.y  = desiredPos.y < 0 ? 0 : desiredPos.y;

		// add some lag time to the camera
		var smoothPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

		transform.position = smoothPos;
		transform.LookAt(lookAtTarget);
	}
}
