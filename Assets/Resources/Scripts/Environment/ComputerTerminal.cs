using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTerminal : MonoBehaviour, IInteractable
{
    [SerializeField]
    GameObject lookAtCamera;

    [SerializeField]
    GameObject uiCanvas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract(){
        // zoom camera to terminal
        Camera.main.GetComponent<MainCamera>().SwitchCamera(lookAtCamera);

        // show ui
        uiCanvas.SetActive(true);
    }
}
