using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    CharacterController characterController;
    Vector3 speedSmoothVelocity;

    void Start(){
        characterController = transform.parent.gameObject.GetComponent<CharacterController>();
    }
    
    void Update(){
        // move camera to top of hitbox
        if(Mathf.Abs(transform.localPosition.y - characterController.height) > 0.1f){
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, characterController.center, ref speedSmoothVelocity, 0.1f);
        } 
    }
}
