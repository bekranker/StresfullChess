using UnityEngine;
using UnityEngine.UI;

public class ComfortManager : MonoBehaviour
{
    [Header("UI Panels")]
    public RectTransform leftPanel;
    public RectTransform rightPanel;
    
    public RectTransform leftSmallPanel;
    public RectTransform rightSmallPanel;
    
    private Vector2 leftSmallStartPos;
    private Vector2 rightSmallStartPos;

    public Image leftImage;   // EKLENDİ
    public Image rightImage;  // EKLENDİ

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

        leftSmallStartPos = leftSmallPanel.anchoredPosition;
        rightSmallStartPos = rightSmallPanel.anchoredPosition;
    }

    void Update()
    {
        UpdatePanels();
        UpdateFlashing(leftImage, leftComfort);   // EKLENDİ
        UpdateFlashing(rightImage, rightComfort); // EKLENDİ
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
        
        // Sol panelin büyüme/daralma farkı
        float leftDelta = leftPanel.rect.width - (totalWidth / 2f);
        leftSmallPanel.anchoredPosition = leftSmallStartPos + new Vector2(leftDelta, 0f);

        // Sağ panelin büyüme/daralma farkı
        float rightDelta = rightPanel.rect.width - (totalWidth / 2f);
        rightSmallPanel.anchoredPosition = rightSmallStartPos - new Vector2(rightDelta, 0f);
    }

    void UpdateFlashing(Image panelImage, float comfort) // EKLENDİ
    {
        if (comfort >= 100f)
        {
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 1f);
            return;
        }

        float flashSpeed = Mathf.Lerp(0.5f, 5f, 0.7f - comfort / 100f); // comfort düşükse hız artar
        float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, alpha);
    }

    void Testing()
    {
        if (Input.GetKeyDown("a"))
        {
            ApplyStress("left", 10f);
        }
        if (Input.GetKeyDown("d"))
        {
            ApplyStress("right", 10f);
        }
    }
}
