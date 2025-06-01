using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Chessman : MonoBehaviour
{
    public Game controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;

    public string player;

    public float tileSpacing = 0.66f;
    public Vector2 boardOffset = new Vector2(-2.3f, -2.3f);

    [SerializeField] private SpriteRenderer _spriteRenderer;
    // public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    // public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public TextMeshProUGUI stressText;
    public ChessPieceData Data;


    public void InitChessMan(ChessPieceData data, int x, int y, string name, Game game, Transform boardParent = null)
    {
        transform.SetParent(boardParent);
        controller = game;
        Data = data;
        SetXBoard(x);
        SetYBoard(y);
        gameObject.name = name;
        Activate();
    }
    public void Activate()
    {
        SetCoords();
        _spriteRenderer.sprite = Data.PieceSprite;
        if (name[0] == 'b')
        {
            player = "black";
        }
        else if (name[0] == 'w')
        {
            player = "white";
        }
        // Activate() fonksiyonunun sonuna eklenmeli:
        SetBaseValue();
    }

    public void SetCoords()
    {
        float x = xBoard * tileSpacing + boardOffset.x;
        float y = yBoard * tileSpacing + boardOffset.y;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXboard()
    {
        return xBoard;
    }

    public int GetYboard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            DestroyMovePlates();

            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king":
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
                break;
        }
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 0);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 0);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }

            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null &&
                sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null &&
                sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x = x * tileSpacing + boardOffset.x;
        y = y * tileSpacing + boardOffset.y;
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x = x * tileSpacing + boardOffset.x;
        y = y * tileSpacing + boardOffset.y;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
    // Eklenen değişkenler
    public int Stress = 0;
    public int BaseValue = 1;

    // Base değerleri otomatik ayarlayan fonksiyon
    public void SetBaseValue()
    {
        // switch (name)
        // {
        //     case "white_pawn":
        //     case "black_pawn":
        //         baseValue = 1; break;
        //     case "white_knight":
        //     case "black_knight":
        //     case "white_bishop":
        //     case "black_bishop":
        //         baseValue = 3; break;
        //     case "white_rook":
        //     case "black_rook":
        //         baseValue = 5; break;
        //     case "white_queen":
        //     case "black_queen":
        //         baseValue = 9; break;
        //     case "white_king":
        //     case "black_king":
        //         baseValue = 10; break;
        // }
        BaseValue = Data.BaseAmount;
    }

    private void Update()
    {
        UpdateStressDisplay();

    }

    public void UpdateStressDisplay()
    {
        if (Stress == 0)
        {
            stressText.enabled = false;
            return;
        }
        stressText.enabled = true;
        stressText.text = Stress.ToString();
    }
}
