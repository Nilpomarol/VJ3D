using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    public void updateScore(int score) { 
        scoreText.text = score.ToString();
    }
 }
