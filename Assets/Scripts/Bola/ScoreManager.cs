using Assets.Scripts.Bola;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private ScoreUI scoreUI;

    private int score;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreUI.UpdateScoreInUI(score);
    }
}