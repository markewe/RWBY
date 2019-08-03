using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractingPlayerControls : PlayerControls
{
    public override void HandleInputs(PlayerInputs playerInputs){
        
    }

    public override void Translate(){
        
    }
    
    public override void Rotate(){
        
    }

    public override void SetAnimationParams(){
        animator.SetBool("isInteracting", true);
    }

    public override void PostEvents(){
        
    }

    public override void ExitControls(){

    }
}
