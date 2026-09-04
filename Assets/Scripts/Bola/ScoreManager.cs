using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private LeaderboardUI leaderboardUI;

    private readonly Dictionary<PlayerId, int> scores = new();


    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    public void IncreaseScore(PlayerId playerId, int amount)
    {
        if (!IsServer)
            return;

        if (!scores.ContainsKey(playerId))
            return;

        scores[playerId] += amount;
        leaderboardUI.UpdateScoreRPC(playerId, amount);
    }
}