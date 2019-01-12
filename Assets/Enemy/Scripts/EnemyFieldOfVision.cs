using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfVision : MonoBehaviour {
    [SerializeField]
	GameObject parent;

	public void Start () {

	}

	public void Update(){

	}

    void OnTriggerEnter(Collider col){
        var ec = parent.GetComponent<EnemyController>();

        ec.StartAttack(ec.gameObject);
    }

}
