using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject creatorPanel;
    void Start()
    {
        howToPlayPanel.SetActive(false); // Başlangıçta kapalı
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void HowToPlay()
    {
        howToPlayPanel.SetActive(true); // Paneli aç
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor...");
        Application.Quit();
    }
    
    public void Creator()
    {
        creatorPanel.SetActive(true); // Paneli aç
    }

    void Update()
    {
        // Eğer How To Play paneli açıksa ve herhangi bir yere tıklanırsa geri kapat
        if (howToPlayPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            howToPlayPanel.SetActive(false);
            
        }
        
        if (creatorPanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            creatorPanel.SetActive(false);
            
        }
    }
}