using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneShadowMoveController : MonoBehaviour {

	public Vector3 direction;

	[SerializeField]
	float cloneSpeed;

	void Update(){
		if(direction != null)
		{
			transform.Translate(direction * cloneSpeed * Time.deltaTime);
		}
	}
}
