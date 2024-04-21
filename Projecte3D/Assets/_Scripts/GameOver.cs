using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOverCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int score;

    public void StopGame(int score) { 
        gameCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
        this.score = score;
        scoreText.text = score.ToString();
        SubmitScore();
    }


    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void SubmitScore() { 
        
    }
}
