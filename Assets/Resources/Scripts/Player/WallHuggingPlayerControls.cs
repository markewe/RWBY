using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHuggingPlayerControls : PlayerControls
{
    public RaycastHit wallInfo;

    [SerializeField]
    GameObject wallChecker;

    bool endOfWallLeft = false;
    bool endOfWallRight = false;
    float walkSpeed = 2f;
    float speedSmoothTime = 0.1f;
    float currentSpeed;
    float speedSmoothVelocity;

    Vector3 targetDirection;
    Vector3 vel;

    public override void Init(){
        base.Init();

        // move player perpendicular to wall at point
        PositionPlayer();
    }

    public override void HandleInputs(PlayerInputs playerInputs){
        var inputDirection = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * new Vector3(playerInputs.inputX, 0f, playerInputs.inputZ);
        targetDirection = Quaternion.Euler(0, transform.eulerAngles.y * -1f, 0) * inputDirection;

        //if(playerInputs.but)
    }

    public override void Translate(){
        // determine how fast to move
        var targetSpeed = endOfWallLeft || endOfWallRight ? 0f : walkSpeed;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed * targetDirection.x, ref speedSmoothVelocity, speedSmoothTime);
        Vector3 moveToPoint = transform.right * currentSpeed * Time.deltaTime;

        if(targetDirection.z < -0.9f){
            ExitControls();
        }
        else if(targetDirection.x < 0 && !endOfWallLeft){
            characterController.Move(moveToPoint);
            endOfWallLeft = CheckEndOfWall(-1f);
            endOfWallRight = false;
        }
        else if(targetDirection.x > 0 && !endOfWallRight){
            characterController.Move(moveToPoint);
            endOfWallRight = CheckEndOfWall(1f);
            endOfWallLeft = false;
        }
    }
    
    public override void Rotate(){
        // no rotation against walls
    }

    public override void SetAnimationParams(){
        animator.SetFloat("hSpeed", currentSpeed);
        animator.SetBool("isWallHugging", true);
    }

    public override void PostEvents(){
        // check if wall hug line can see around corner
        // if true, then we've reached the end of the wall
    }

    // positve right / negative left
    bool CheckEndOfWall(float side){
		RaycastHit hit;
		var layerMask = 1 << (int)Layers.Environment;
		var wallCheckerCollider = wallChecker.GetComponent<BoxCollider>();
		var rayStart = wallChecker.transform.position + (wallCheckerCollider.size.x *(wallChecker.transform.localScale.x / 2f)) * (wallChecker.transform.right * side);

        return !Physics.Raycast(rayStart, (wallChecker.transform.forward), out hit, characterController.radius * 2f, layerMask);
    }
    
    public override void ExitControls(){
        animator.SetFloat("hSpeed", 0f);
        animator.SetBool("isWallHugging", false);
        GetComponent<PlayerInputHandler>().RestoreDefaultControls();
    }

    void PositionPlayer(){
        // move player to point
        var newPosition = wallInfo.point + (wallInfo.normal * characterController.radius);
        
        newPosition.y = transform.position.y;
        transform.position = newPosition;// + new Vector3(0f, characterController.height / 2f, 0f);

        // rotate player so that their forward is parallel to normal of wall
        transform.forward = wallInfo.normal * -1f;
    }

    #region debug

    // void OnDrawGizmos() {
    //     var wallCheckerCollider = wallChecker.GetComponent<BoxCollider>();
	// 	Gizmos.color = Color.red;
    //     Gizmos.DrawRay(wallChecker.transform.position + (wallCheckerCollider.size.x * wallChecker.transform.localScale.x / 2f) * (wallChecker.transform.right * 1f), wallChecker.transform.forward * 0.5f);
    //     Gizmos.DrawRay(wallChecker.transform.position + (wallCheckerCollider.size.x) * (wallChecker.transform.right * -1f), wallChecker.transform.forward * 0.5f);
	// }

    #endregion
}
