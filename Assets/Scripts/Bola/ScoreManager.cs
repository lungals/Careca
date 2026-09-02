using Assets.Scripts.Bola;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private LeaderboardUI leaderboardUI;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void IncreaseScore(PlayerId playerId, int amount)
    {
        leaderboardUI.IncreaseScore(playerId, amount);
    }
}