using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    CinemachineFreeLook playerCamera;

    private void Start()
    {
        playerCamera = GetComponent<CinemachineFreeLook>();
    }
 
    public void UpdateLookAtTarget(Transform newLookAtTarget){
        playerCamera.LookAt = newLookAtTarget;
    }

    public void UpdateFollowTarget(Transform newFollowTarget){
        playerCamera.Follow = newFollowTarget;
    }

    public void UpdateCameraTarget(Transform newFollowTarget, Transform newLookAtTarget){
        playerCamera.Follow = newFollowTarget;
        playerCamera.LookAt = newLookAtTarget;
    }

    public void DisableMouseMovement(){
        playerCamera.m_XAxis.m_InputAxisName = "";
        playerCamera.m_YAxis.m_InputAxisName = "";
    }

    public void EnableMouseMovement(){
        playerCamera.m_XAxis.m_InputAxisName = "Mouse X";
        playerCamera.m_YAxis.m_InputAxisName = "Mouse Y";
    }
}
