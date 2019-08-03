using UnityEngine;

public class ButtonClose : MonoBehaviour
{
    [SerializeField]
    GameObject uiCanvas;

    public void CloseUI(){
        uiCanvas.SetActive(false);
    }
}
