using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Game : MonoBehaviour
{
    [Header("DOTween Props")]
    [SerializeField] private float _duration = 0.2f;
    [SerializeField] private float _boardSpawnIntervalDuration = 0.2f;
    [SerializeField] private float _boardPunchScale = 0.2f;



    [Header("Chess Borad Props")]
    [SerializeField] private GameObject _boardPrefab;
    [SerializeField] private Color _whiteColor;
    [SerializeField] private Color _blackColor;
    [SerializeField] private Transform _boardCenter;


    [Header("Chess Piece Props")]
    [SerializeField] private Transform _parent;
    public GameObject chesspiece;

    private Chessman[,] positions = new Chessman[8, 8];
    private Chessman[] playerBlack = new Chessman[16];
    private Chessman[] playerWhite = new Chessman[16];

    private string currentPlayer = "white";

    private bool gameOver = false;

    public int calmWhite = 100;
    public int calmBlack = 100;
    
    public ComfortManager comfortManager;

    public TextMeshProUGUI topComfortText;
    public TextMeshProUGUI bottomComfortText;


    public IEnumerator Start()
    {
        Vector2 tileSize = _boardPrefab.transform.localScale;
        float totalWidth = 8 * tileSize.x;
        float totalHeight = 8 * tileSize.y;

        Vector3 bottomLeft = _boardCenter.position - new Vector3(totalWidth / 2f, totalHeight / 2f, 0);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject tile = Instantiate(_boardPrefab, Vector3.zero, Quaternion.identity);
                Vector3 position = bottomLeft + new Vector3(x * tileSize.x, y * tileSize.y, 0);
                tile.transform.position = position;

                bool isWhite = (x + y) % 2 == 0;
                tile.GetComponent<SpriteRenderer>().color = isWhite ? _whiteColor : _blackColor;
                tile.transform.DOPunchScale(Vector2.one * _boardPunchScale, _duration);
                yield return new WaitForSeconds(_boardSpawnIntervalDuration);
            }
        }



        playerWhite = new Chessman[] {
        Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
        Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
        Create("white_pawn", 6, 1), Create("white_pawn", 7, 1),
        Create("white_rook", 0, 0), Create("white_rook", 7, 0),
        Create("white_bishop", 2, 0), Create("white_bishop", 5, 0),
        Create("white_knight", 1, 0), Create("white_knight", 6, 0),
        Create("white_king", 4, 0), Create("white_queen", 3, 0)};

        playerBlack = new Chessman[] {
        Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
        Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
        Create("black_pawn", 6, 6), Create("black_pawn", 7, 6),
        Create("black_rook", 0, 7), Create("black_rook", 7, 7),
        Create("black_bishop", 2, 7), Create("black_bishop", 5, 7),
        Create("black_knight", 1, 7), Create("black_knight", 6, 7),
        Create("black_king", 4, 7), Create("black_queen", 3, 7)};

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
            yield return new WaitForSeconds(_duration);
        }
    }

    public Chessman Create(string name, int x, int y)
    {
        Chessman cm = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Chessman>();
        cm.InitChessMan(Resources.Load<ChessPieceData>("Scriptables/ChessPieces/" + name), x, y, name, this, _parent);
        return cm;
    }

    public void SetPosition(Chessman cm)
    {
        if (cm == null) return;
        cm.EnableSprite();
        cm.PunchSprite();
        positions[cm.GetXboard(), cm.GetYboard()] = cm;
    }

    public void SetPositionsEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public Chessman GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            SceneManager.LoadScene("Game");
        }
        UpdateComfortTexts();
    }
    
    void UpdateComfortTexts()
    {
        if (comfortManager != null)
        {
            topComfortText.text = $"Top Comfort: {comfortManager.topComfort:F1}";
            bottomComfortText.text = $"Bottom Comfort: {comfortManager.bottomComfort:F1}";
        }
    }

    public void Winner(string playerWinner)
    {
        gameOver = true;

        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }

    public void CalculateStress()
    {
        // Önce tüm taşların stresini sıfırla
        foreach (Chessman piece in playerWhite)
            if (piece != null) piece.Stress = 0;
        foreach (Chessman piece in playerBlack)
            if (piece != null) piece.Stress = 0;

        // Her taş, diğer oyuncunun tüm taşları tarafından tehdit ediliyor mu kontrol et
        foreach (Chessman enemy in playerWhite)
        {
            if (enemy == null) continue;
            List<Vector2Int> threats = GetThreats(enemy);
            foreach (Vector2Int threat in threats)
            {
                Chessman target = GetPosition(threat.x, threat.y);
                if (target != null && target.GetComponent<Chessman>().player != "white")
                    target.GetComponent<Chessman>().Stress += 1;
            }
        }

        foreach (Chessman enemy in playerBlack)
        {

            if (enemy == null) continue;
            List<Vector2Int> threats = GetThreats(enemy);
            foreach (Vector2Int threat in threats)
            {
                Chessman target = GetPosition(threat.x, threat.y);
                if (target != null && target.player != "black")
                    target.Stress += 1;
            }
        }
    }
    public List<Vector2Int> GetThreats(Chessman cm)
    {
        List<Vector2Int> threatList = new List<Vector2Int>();

        int x = cm.GetXboard();
        int y = cm.GetYboard();

        switch (cm.name)
        {
            case "white_queen":
            case "black_queen":
                threatList.AddRange(GetLineThreats(x, y, 1, 0));
                threatList.AddRange(GetLineThreats(x, y, 0, 1));
                threatList.AddRange(GetLineThreats(x, y, 1, 1));
                threatList.AddRange(GetLineThreats(x, y, -1, 0));
                threatList.AddRange(GetLineThreats(x, y, 0, -1));
                threatList.AddRange(GetLineThreats(x, y, -1, -1));
                threatList.AddRange(GetLineThreats(x, y, -1, 1));
                threatList.AddRange(GetLineThreats(x, y, 1, -1));
                break;
            case "white_rook":
            case "black_rook":
                threatList.AddRange(GetLineThreats(x, y, 1, 0));
                threatList.AddRange(GetLineThreats(x, y, 0, 1));
                threatList.AddRange(GetLineThreats(x, y, -1, 0));
                threatList.AddRange(GetLineThreats(x, y, 0, -1));
                break;
            case "white_bishop":
            case "black_bishop":
                threatList.AddRange(GetLineThreats(x, y, 1, 1));
                threatList.AddRange(GetLineThreats(x, y, 1, -1));
                threatList.AddRange(GetLineThreats(x, y, -1, 1));
                threatList.AddRange(GetLineThreats(x, y, -1, -1));
                break;
            case "white_knight":
            case "black_knight":
                int[,] offsets = { { 1, 2 }, { -1, 2 }, { 2, 1 }, { 2, -1 }, { 1, -2 }, { -1, -2 }, { -2, 1 }, { -2, -1 } };
                for (int i = 0; i < offsets.GetLength(0); i++)
                {
                    int tx = x + offsets[i, 0];
                    int ty = y + offsets[i, 1];
                    if (PositionOnBoard(tx, ty)) threatList.Add(new Vector2Int(tx, ty));
                }
                break;
            case "white_king":
            case "black_king":
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (dx != 0 || dy != 0)
                            if (PositionOnBoard(x + dx, y + dy)) threatList.Add(new Vector2Int(x + dx, y + dy));
                break;
            case "white_pawn":
                if (PositionOnBoard(x + 1, y + 1)) threatList.Add(new Vector2Int(x + 1, y + 1));
                if (PositionOnBoard(x - 1, y + 1)) threatList.Add(new Vector2Int(x - 1, y + 1));
                break;
            case "black_pawn":
                if (PositionOnBoard(x + 1, y - 1)) threatList.Add(new Vector2Int(x + 1, y - 1));
                if (PositionOnBoard(x - 1, y - 1)) threatList.Add(new Vector2Int(x - 1, y - 1));
                break;
        }

        return threatList;
    }
    public List<Vector2Int> GetLineThreats(int x, int y, int dx, int dy)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        x += dx; y += dy;
        while (PositionOnBoard(x, y))
        {
            result.Add(new Vector2Int(x, y));
            if (GetPosition(x, y) != null) break;
            x += dx; y += dy;
        }
        return result;
    }
    public void PieceCaptured(Chessman cm)
    {
        if (cm == null) return;

        int loss = cm.BaseValue + cm.Stress;

        if (cm.player == "white")
            calmWhite -= loss;
        else
            calmBlack -= loss;

        if (calmWhite <= 0)
            Winner("black");
        else if (calmBlack <= 0)
            Winner("white");
    }
}