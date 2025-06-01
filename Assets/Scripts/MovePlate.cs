using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    GameObject reference = null;

    int matrixX;
    int matrixY;

    public bool attack = false;

    public void Start()
    {
        if(attack)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        Game gameScript = controller.GetComponent<Game>();

        if (attack)
        {
            GameObject cp = gameScript.GetPosition(matrixX, matrixY);

            // Eğer rakip şah yeniyorsa hemen bitir
            if (cp.name == "white_king") gameScript.Winner("black");
            if (cp.name == "black_king") gameScript.Winner("white");

            // Calm (sakinlik kasası) sistemi üzerinden hesaplama yap
            gameScript.PieceCaptured(cp);

            Destroy(cp);
        }

        // Eski konumu boşalt
        gameScript.SetPositionsEmpty(
            reference.GetComponent<Chessman>().GetXboard(),
            reference.GetComponent<Chessman>().GetYboard()
        );

        // Yeni konumu güncelle
        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        // Yeni pozisyonu kaydet
        gameScript.SetPosition(reference);

        // Sıra değiştir + stres hesapla
        gameScript.NextTurn();
        gameScript.CalculateStress(); // Eğer NextTurn içinde çağrılmadıysa buraya eklersin

        // Tüm hareket karelerini sil
        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }    

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
