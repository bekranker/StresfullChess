using UnityEngine;
using UnityEngine.UI;

public class ComfortManager : MonoBehaviour
{
    [Header("UI Panels")]
    public RectTransform leftPanel;
    public RectTransform rightPanel;

    [Header("Comfort Points")]
    [Range(0, 100)] public float leftComfort = 100f;
    [Range(0, 100)] public float rightComfort = 100f;

    private float totalWidth;
    private float panelHeight;

    void Start()
    {
        RectTransform parentRect = leftPanel.parent.GetComponent<RectTransform>();
        totalWidth = parentRect.rect.width;
        panelHeight = parentRect.rect.height;
    }

    void Update()
    {
        UpdatePanels();
        Testing();
    }

    public void ApplyStress(string side, float stressPoints)
    {
        if (side == "left")
        {
            leftComfort = Mathf.Max(0, leftComfort - stressPoints);
            rightComfort = rightComfort + stressPoints;
        }
        else if (side == "right")
        {
            rightComfort = Mathf.Max(0, rightComfort - stressPoints);
            leftComfort = leftComfort + stressPoints;
        }
            
    }

    void UpdatePanels()
    {
        float leftRatio = leftComfort / (leftComfort + rightComfort);
        float rightRatio = rightComfort / (leftComfort + rightComfort);

        float leftWidth = totalWidth * leftRatio;
        float rightWidth = totalWidth * rightRatio;

        // Apply left panel
        leftPanel.sizeDelta = new Vector2(leftWidth, panelHeight);
        leftPanel.anchoredPosition = new Vector2(-totalWidth / 2f + leftWidth / 2f, 0f);

        // Apply right panel
        rightPanel.sizeDelta = new Vector2(rightWidth, panelHeight);
        rightPanel.anchoredPosition = new Vector2(totalWidth / 2f - rightWidth / 2f, 0f);
    }


    void Testing()
    {
        if (Input.GetKeyDown("a"))
        {
            ApplyStress("left",10f);
        } 
        if (Input.GetKeyDown("d"))
        {
            ApplyStress("right",10f);
        }
    }
}