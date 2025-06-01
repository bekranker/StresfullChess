using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    [SerializeField] private Game _gameScript;

    Chessman reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
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

            Destroy(cp);
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
        _gameScript.CalculateStress(); // Eğer NextTurn içinde çağrılmadıysa buraya eklersin

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
