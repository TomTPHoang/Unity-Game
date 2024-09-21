using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public int scoreAmountOnKill;
    public int currentScore;

    [SerializeField] private Text scoreText;

    private void Start()
    {
        InitVariables();
    }

    private void InitVariables()
    {
        currentScore = 0;
    }

    public void AddToScore()
    {
        currentScore += scoreAmountOnKill;
        scoreText.text = currentScore.ToString();
    }
}
