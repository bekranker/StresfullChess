using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComfortManager : MonoBehaviour
{
    [Header("UI Panels")]
    public RectTransform topPanel;
    public RectTransform bottomPanel;

    [Header("Small Panels or TMP")]
    public RectTransform topSmallPanel;
    public RectTransform bottomSmallPanel;

    [Header("Comfort Points")]
    [Range(0, 150)] public float topComfort = 75;
    [Range(0, 150)] public float bottomComfort = 75;
    
    [Header("Winner UI")]
    public RectTransform winnerPanel;
    public TextMeshProUGUI winnerText;
    public float winnerPanelDuration = 0.8f;

    private float totalHeight;
    private float panelWidth;
    
    bool gameEnded = false;

    private Vector2 topSmallStartPos;
    private Vector2 bottomSmallStartPos;

    public Image upImage;   // EKLENDİ
    public Image downImage;  // EKLENDİ

    void Start()
    {
        RectTransform parentRect = topPanel.parent.GetComponent<RectTransform>();
        totalHeight = parentRect.rect.height;
        panelWidth = parentRect.rect.width;

        topSmallStartPos = topSmallPanel.anchoredPosition;
        bottomSmallStartPos = bottomSmallPanel.anchoredPosition;
    }


    void Update()
    {
        UpdatePanels();
        UpdateFlashing(upImage, bottomComfort);   // EKLENDİ
        UpdateFlashing(downImage, topComfort); // EKLENDİ
        CheckForWinner(); // EKLENDİ
        Testing();
    }
    
    void CheckForWinner()
    {
        if (gameEnded) return;

        if (topComfort <= 0f)
        {
            ShowWinner("Bottom");
            gameEnded = true;
        }
        else if (bottomComfort <= 0f)
        {
            ShowWinner("Top");
            gameEnded = true;
        }
    }

    void UpdatePanels()
    {
        float totalComfort = topComfort + bottomComfort;
        float topRatio = topComfort / totalComfort;
        float bottomRatio = bottomComfort / totalComfort;

        float topHeight = totalHeight * topRatio;
        float bottomHeight = totalHeight * bottomRatio;

        // Apply top panel
        topPanel.sizeDelta = new Vector2(panelWidth, topHeight);
        topPanel.anchoredPosition = new Vector2(0f, (totalHeight / 2f) - (topHeight / 2f));

        // Apply bottom panel
        bottomPanel.sizeDelta = new Vector2(panelWidth, bottomHeight);
        bottomPanel.anchoredPosition = new Vector2(0f, -(totalHeight / 2f) + (bottomHeight / 2f));

        // Calculate and apply small panels (from initial placed positions)
        float topDelta = topPanel.rect.height - (totalHeight / 2f);
        topSmallPanel.anchoredPosition = topSmallStartPos - new Vector2(0f, topDelta);

        float bottomDelta = bottomPanel.rect.height - (totalHeight / 2f);
        bottomSmallPanel.anchoredPosition = bottomSmallStartPos + new Vector2(0f, bottomDelta);
    }

    public void ApplyStress(string side, float stressPoints)
    {
        if (side == "top")
        {
            topComfort = Mathf.Max(0, topComfort - stressPoints);
            bottomComfort = Mathf.Min(150, bottomComfort + stressPoints);
        }
        else if (side == "bottom")
        {
            bottomComfort = Mathf.Max(0, bottomComfort - stressPoints);
            topComfort = Mathf.Min(150, topComfort + stressPoints);
        }
    }

    void Testing()
    {
        if (Input.GetKeyDown("w"))
        {
            ApplyStress("top", 35f);
        }
        if (Input.GetKeyDown("s"))
        {
            ApplyStress("bottom", 35f);
        }
    }

    void UpdateFlashing(Image panelImage, float comfort)
    {
        if (comfort >= 75f)
        {
            // Flash efektini kapat, tam görünürlükte tut
            panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, 1f);
            return;
        }

        // comfort < 50 olduğunda yanıp sönme başlar
        float flashSpeed = Mathf.Lerp(0.5f, 5f, (75f - comfort) / 75f); // 0 → yavaş, 50 → hızlı
        float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);
        panelImage.color = new Color(panelImage.color.r, panelImage.color.g, panelImage.color.b, alpha);
    }
    
    void ShowWinner(string winner)
    {
        // Metin ve renk taraflara göre atanıyor
        if (winner == "Bottom")
        {
            winnerText.text = "🔴 KIRMIZI KAZANDI!";
            winnerText.color = Color.red;
        }
        else if (winner == "Top")
        {
            winnerText.text = "🔵 MAVİ KAZANDI!";
            winnerText.color = Color.blue;
        }

        // Panel konumunu dışarıdan başlat
        winnerPanel.anchoredPosition = new Vector2(0, -Screen.height);

        // Yukarı çıkış animasyonu
        winnerPanel.DOAnchorPosY(0, winnerPanelDuration).SetEase(Ease.OutBack);
    }
}
