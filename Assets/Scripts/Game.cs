using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject chesspiece;

    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";

    private bool gameOver = false;
    
    public int calmWhite = 100;
    public int calmBlack = 100;


    // Start is called before the first frame update
    void Start()
    {
        playerWhite = new GameObject[] {Create("white_rook", 0, 0), Create("white_knight", 1, 0),
        Create("white_king", 4, 0), Create("white_bishop", 2, 0), Create("white_queen", 3, 0),
        Create("white_rook", 7, 0), Create("white_knight", 6, 0), Create("white_bishop", 5, 0),
        Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
        Create("white_pawn", 5, 1), Create("white_pawn", 4, 1), Create("white_pawn", 3, 1),
        Create("white_pawn", 6, 1), Create("white_pawn", 7, 1)};

        playerBlack = new GameObject[] {Create("black_rook", 0, 7), Create("black_knight", 1, 7),
        Create("black_king", 4, 7), Create("black_bishop", 2, 7), Create("black_queen", 3, 7),
        Create("black_rook", 7, 7), Create("black_knight", 6, 7), Create("black_bishop", 5, 7),
        Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
        Create("black_pawn", 5, 6), Create("black_pawn", 4, 6), Create("black_pawn", 3, 6),
        Create("black_pawn", 6, 6), Create("black_pawn", 7, 6)};
    
        for(int i=0;i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        positions[cm.GetXboard(), cm.GetYboard()] = obj;
    }

    public void SetPositionsEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
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
        if(currentPlayer == "white")
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
        if(gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            SceneManager.LoadScene("Game");
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
        foreach (GameObject piece in playerWhite)
            if(piece != null) piece.GetComponent<Chessman>().stress = 0;
        foreach (GameObject piece in playerBlack)
            if(piece != null) piece.GetComponent<Chessman>().stress = 0;

        // Her taş, diğer oyuncunun tüm taşları tarafından tehdit ediliyor mu kontrol et
        foreach (GameObject enemy in playerWhite)
        {
            if (enemy == null) continue;
            List<Vector2Int> threats = GetThreats(enemy);
            foreach (Vector2Int threat in threats)
            {
                GameObject target = GetPosition(threat.x, threat.y);
                if (target != null && target.GetComponent<Chessman>().player != "white")
                    target.GetComponent<Chessman>().stress += 1;
            }
        }

        foreach (GameObject enemy in playerBlack)
        {
            if (enemy == null) continue;
            List<Vector2Int> threats = GetThreats(enemy);
            foreach (Vector2Int threat in threats)
            {
                GameObject target = GetPosition(threat.x, threat.y);
                if (target != null && target.GetComponent<Chessman>().player != "black")
                    target.GetComponent<Chessman>().stress += 1;
            }
        }
    }
    public List<Vector2Int> GetThreats(GameObject piece) {
    Chessman cm = piece.GetComponent<Chessman>();
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
            int[,] offsets = { {1,2},{-1,2},{2,1},{2,-1},{1,-2},{-1,-2},{-2,1},{-2,-1}};
            for (int i = 0; i < offsets.GetLength(0); i++)
            {
                int tx = x + offsets[i, 0];
                int ty = y + offsets[i, 1];
                if (PositionOnBoard(tx, ty)) threatList.Add(new Vector2Int(tx, ty));
            }
            break;
        case "white_king":
        case "black_king":
            for(int dx=-1;dx<=1;dx++)
                for(int dy=-1;dy<=1;dy++)
                    if(dx != 0 || dy != 0)
                        if(PositionOnBoard(x+dx,y+dy)) threatList.Add(new Vector2Int(x+dx,y+dy));
            break;
        case "white_pawn":
            if(PositionOnBoard(x+1,y+1)) threatList.Add(new Vector2Int(x+1,y+1));
            if(PositionOnBoard(x-1,y+1)) threatList.Add(new Vector2Int(x-1,y+1));
            break;
        case "black_pawn":
            if(PositionOnBoard(x+1,y-1)) threatList.Add(new Vector2Int(x+1,y-1));
            if(PositionOnBoard(x-1,y-1)) threatList.Add(new Vector2Int(x-1,y-1));
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
    public void PieceCaptured(GameObject captured)
    {
        if (captured == null) return;

        Chessman cm = captured.GetComponent<Chessman>();
        int loss = cm.baseValue + cm.stress;

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
