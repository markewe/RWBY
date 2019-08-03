using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    [SerializeField]
    GameObject currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchCamera(GameObject newCamera){
		if(currentCamera != newCamera){
			newCamera.SetActive(true);
			currentCamera.SetActive(false);
			currentCamera = newCamera;
		}
        
	}
}
