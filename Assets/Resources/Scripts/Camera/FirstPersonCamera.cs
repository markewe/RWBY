using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    Transform positioner;
    CharacterController characterController;
    Vector3 speedSmoothVelocity;

    void Start(){
        characterController = transform.parent.gameObject.GetComponent<CharacterController>();
    }
    
    void Update(){
        // keep camera bobbing
        // if(Mathf.Abs(transform.localPosition.y - positioner.position.y) > 0.1f){
            transform.position = Vector3.SmoothDamp(transform.position, positioner.position, ref speedSmoothVelocity, 0.1f);
        // } 
    }
}
