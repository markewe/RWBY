using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVision : MonoBehaviour {
    [SerializeField]
	GameObject parent;

    void OnTriggerEnter(Collider col){
        parent.GetComponent<IFieldOfVisionListener>().OnFieldOfVisionEnter(col.gameObject);
    }

    void OnTriggerExit(Collider col){
        parent.GetComponent<IFieldOfVisionListener>().OnFieldOfVisionExit(col.gameObject);
    }
}
