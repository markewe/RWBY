using UnityEngine;
using cakeslice;

public class OutlineComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        gameObject.AddComponent<Outline>();

        foreach(Transform child in transform){
            if(child.GetComponent<Renderer>() != null){
                child.gameObject.AddComponent<Outline>();
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        Destroy(gameObject.GetComponent<Outline>());
        

        foreach(Transform child in transform){
            Destroy(child.GetComponent<Outline>());
        }
        
    }
}
