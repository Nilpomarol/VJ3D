using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject gameMenuCanvas;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject player;

    private int score;

    public void StopGame(int score) { 
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();
        player.SetActive(false);
    }

    public void StartGame() {
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        gameMenuCanvas.SetActive(false);
        this.score = 0;
        scoreText.text = score.ToString();
        player.SetActive(true);
    }


    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        StartGame();
    }
}
