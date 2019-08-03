using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineComponent : MonoBehaviour
{
    Shader defaultShader;
    Shader outlineShader;

    // Start is called before the first frame update
    void Start()
    {
        defaultShader = gameObject.GetComponent<Renderer>().material.shader;
        outlineShader = Shader.Find("RealToon/Version 5/Default/White Outline");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        gameObject.GetComponent<Renderer>().material.shader = outlineShader;
    }

    void OnTriggerExit(Collider other)
    {
        gameObject.GetComponent<Renderer>().material.shader = defaultShader;
    }
}
