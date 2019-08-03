using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipComponent : MonoBehaviour
{
    [SerializeField]
    TooltipLocation tooltipLocation;

    [SerializeField]
    string tooltipText;

    bool rotateTowardsCamera;
    GameObject canvasContainer;

    // Start is called before the first frame update
    void Start()
    {
        rotateTowardsCamera = tooltipLocation == TooltipLocation.Above ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        // rotate tooltip towards camera    
        // if(canvasContainer != null && rotateTowardsCamera){
        //     canvasContainer.transform.forward = Camera.main.transform.forward;
        // }

        if(canvasContainer != null)
        {
            canvasContainer.transform.Find("Text").GetComponent<Text>().transform.position = Camera.main.WorldToScreenPoint(transform.position);
            print(transform.position);
            print(Camera.main.WorldToScreenPoint(transform.position));

        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        canvasContainer = new GameObject();
        Canvas canvas = canvasContainer.AddComponent<Canvas>();
        //canvas.renderMode = RenderMode.WorldSpace;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler cs = canvasContainer.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1.0f;
        cs.dynamicPixelsPerUnit = 100f;
        GraphicRaycaster gr = canvasContainer.AddComponent<GraphicRaycaster>();
        // canvasContainer.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
        // canvasContainer.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
        
        GameObject g2 = new GameObject();
        g2.name = "Text";
        g2.transform.parent = canvasContainer.transform;
        Text t = g2.AddComponent<Text>();
        // g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3.0f);
        // g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3.0f);
        t.GetComponent<RectTransform>().anchoredPosition.Set(0f, 0f);
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        Font ArialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
        t.font = ArialFont;
        t.fontSize = 12;
        t.text = tooltipText;
        t.enabled = true;
        t.color = Color.green;

        canvasContainer.name = "Tooltip";

        SetTooltipPosition();
    }

    void OnTriggerExit(Collider other)
    {
        GameObject.Destroy(canvasContainer);
    }

    void SetTooltipPosition(){
        print(transform.position);
        print(Camera.main.WorldToScreenPoint(transform.position));
        // canvasContainer.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        // canvasContainer.GetComponent<RectTransform>().SetParent(transform, false);
        // canvasContainer.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        // canvasContainer.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // switch(tooltipLocation){
        //     case TooltipLocation.Above:
        //         canvasContainer.transform.localPosition = new Vector3(0f, 0f, 0.05f);
        //     break;
        //     case TooltipLocation.Front:
        //         canvasContainer.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        //     break;
        // }
    }
    
}
