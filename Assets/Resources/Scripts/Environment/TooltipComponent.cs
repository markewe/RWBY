using UnityEngine;
using UnityEngine.UI;

public class TooltipComponent : MonoBehaviour
{
    [SerializeField]
    Vector3 tooltipOffset;

    [SerializeField]
    string tooltipText;

    GameObject canvasContainer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // set tooltip position
        if(canvasContainer != null)
        {
            canvasContainer.transform.Find("Text").GetComponent<Text>().transform.position = Camera.main.WorldToScreenPoint(transform.position)
                + tooltipOffset;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        canvasContainer = new GameObject();
        Canvas canvas = canvasContainer.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasContainer.name = "Tooltip";

        CanvasScaler cs = canvasContainer.AddComponent<CanvasScaler>();
        cs.scaleFactor = 1.0f;
        cs.dynamicPixelsPerUnit = 100f;
        GraphicRaycaster gr = canvasContainer.AddComponent<GraphicRaycaster>();
        
        GameObject g2 = new GameObject();
        g2.name = "Text";
        g2.transform.parent = canvasContainer.transform;
        
        Text t = g2.AddComponent<Text>();
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
    }

    void OnTriggerExit(Collider other)
    {
        GameObject.Destroy(canvasContainer);
    }
    
}
