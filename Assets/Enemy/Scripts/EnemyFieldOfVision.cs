using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFieldOfVision : MonoBehaviour {
    [SerializeField]
	GameObject parent;

    void OnTriggerEnter(Collider col){
        parent.GetComponent<AEnemyController>().TargetEnteredFieldOfVision(col.gameObject);
    }

    void OnTriggerExit(Collider col){
        parent.GetComponent<AEnemyController>().TargetExitedFieldOfVision(col.gameObject);
    }
}
