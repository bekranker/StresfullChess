using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    [SerializeField] private Game _gameScript;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float duration = 0.2f;
    Chessman reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void Start()
    {
        _gameScript = FindObjectsByType<Game>(FindObjectsSortMode.None)[0];
        if (attack)
        {
            _spriteRenderer.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            ScaleEffect(Vector3.zero, Vector3.one, 1, 0);
        }
        else
        {
            ScaleEffect(Vector3.one, Vector3.zero, 0, 1);
        }
    }
    public void ScaleEffect(Vector3 targetSacle, Vector3 startScale, float startFade, float targetFade)
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, startFade); // Set initial fade
        _spriteRenderer.transform.localScale = startScale; // Set initial scale
        if (_spriteRenderer != null)
        {
            _spriteRenderer.DOFade(targetFade, duration).SetEase(Ease.InOutQuad);
            _spriteRenderer.transform.DOScale(targetSacle, duration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                ScaleEffect(targetSacle, startScale, startFade, targetFade);
            });
        }
    }
    public void OnMouseUp()
    {
        if (attack)
        {
            Chessman cp = _gameScript.GetPosition(matrixX, matrixY);

            // Eğer rakip şah yeniyorsa hemen bitir
            if (cp.name == "white_king") _gameScript.Winner("black");
            if (cp.name == "black_king") _gameScript.Winner("white");

            // Calm (sakinlik kasası) sistemi üzerinden hesaplama yap
            _gameScript.PieceCaptured(cp);

            // 💡 STRES AKTARIMI
            int stress = cp.Stress;
            string side = cp.player == "white" ? "bottom" : "top";

            // Debug log
            Debug.Log($"[{side.ToUpper()} tarafi] {stress} stres kaybetti, rakibe geçti.");

            FindAnyObjectByType<ComfortManager>().ApplyStress(side, stress);

            Destroy(cp.gameObject);
        }

        // Eski konumu boşalt
        _gameScript.SetPositionsEmpty(
            reference.GetXboard(),
            reference.GetYboard()
        );

        // Yeni konumu güncelle
        reference.SetXBoard(matrixX);
        reference.SetYBoard(matrixY);
        reference.SetCoords();

        // Yeni pozisyonu kaydet
        _gameScript.SetPosition(reference);

        // Sıra değiştir + stres hesapla
        _gameScript.NextTurn();
        _gameScript.CalculateStress();

        // Tüm hareket karelerini sil
        reference.DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(Chessman cm)
    {
        reference = cm;
    }

    public Chessman GetReference()
    {
        return reference;
    }
}
