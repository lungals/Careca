using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Leaderboard")]
    [SerializeField]
    private int maxLeaderboardEntries = 100;

    private NetworkList<ScoreEntry> leaderboard;
    private readonly Dictionary<PlayerId, int> scores = new();

    public NetworkList<ScoreEntry> Leaderboard => leaderboard;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        leaderboard = new NetworkList<ScoreEntry>();
    }

    public override void OnNetworkSpawn()
    {
        PlayerRegister.OnPlayerRegister += PlayerRegister_OnPlayerRegister;
        PlayerRegister.OnPlayerUnregister += PlayerRegister_OnPlayerUnregister;
        base.OnNetworkSpawn();
    }

    public override void OnNetworkDespawn()
    {
        PlayerRegister.OnPlayerRegister -= PlayerRegister_OnPlayerRegister;
        PlayerRegister.OnPlayerUnregister -= PlayerRegister_OnPlayerUnregister;
        base.OnNetworkDespawn();
    }

    #region Player

    private void PlayerRegister_OnPlayerRegister(PlayerId playerId)
    {
        RegisterPlayer(playerId);
    }

    private void PlayerRegister_OnPlayerUnregister(PlayerId playerId)
    {
        UnregisterPlayer(playerId);
    }

    public void RegisterPlayer(PlayerId playerId)
    {
        if (!IsServer)
            return;

        if (!IsValidPlayerId(playerId))
            return;

        if (scores.ContainsKey(playerId))
            return;

        scores.Add(playerId, 0);
    }

    public void UnregisterPlayer(PlayerId playerId)
    {
        if (!IsServer)
            return;

        scores.Remove(playerId);

        RemoveFromLeaderboard(playerId);
    }

    #endregion

    #region Score

    [Rpc(SendTo.Server)]
    public void AddScoreRPC(PlayerId playerId, int amount)
    {
        /*if (!IsServer)
        {
            Debug.LogWarning(
                "AddScore só pode ser chamado no servidor."
            );

            return;
        }*/

        if (!IsValidPlayerId(playerId))
            return;

        if (amount <= 0)
        {
            Debug.LogWarning(
                $"Tentativa de adicionar score inválido: {amount}"
            );

            return;
        }

        if (!scores.ContainsKey(playerId))
            scores.Add(playerId, 0);

        scores[playerId] += amount;

        UpdateLeaderboard(playerId);
    }

    public int GetScore(PlayerId playerId)
    {
        if (!scores.TryGetValue(playerId, out int score))
            return 0;

        return score;
    }

    #endregion

    #region Leaderboard


    private void UpdateLeaderboard(PlayerId playerId)
    {
        if (!scores.TryGetValue(playerId, out int score))
            return;

        int index = FindLeaderboardIndex(playerId);

        if (index >= 0)
        {
            leaderboard[index] = new ScoreEntry()
            {
                PlayerId = playerId,
                Score = score
            };

            SortLeaderboard();
            return;
        }


        if (leaderboard.Count < maxLeaderboardEntries)
        {
            leaderboard.Add(new ScoreEntry()
            {
                PlayerId = playerId,
                Score = score
            });

            SortLeaderboard();
            return;
        }

        // Leaderboard cheio. Descobrimos quem está em último.
        int lowestIndex = FindLowestScoreIndex();

        if (lowestIndex < 0)
            return;

        int lowestScore = leaderboard[lowestIndex].Score;

        
          // Só entra no Top se tiver score maior.
         
        if (score > lowestScore)
        {
            leaderboard[lowestIndex] = new ScoreEntry()
            {
                PlayerId = playerId,
                Score = score
            };

            SortLeaderboard();
        }
    }

    private void RemoveFromLeaderboard(PlayerId playerId)
    {
        int index = FindLeaderboardIndex(playerId);

        if (index < 0)
            return;

        leaderboard.RemoveAt(index);
    }

    private int FindLeaderboardIndex(PlayerId playerId)
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            if (leaderboard[i].PlayerId == playerId)
                return i;
        }

        return -1;
    }

    private int FindLowestScoreIndex()
    {
        if (leaderboard.Count == 0)
            return -1;

        int lowestIndex = 0;

        for (int i = 1; i < leaderboard.Count; i++)
        {
            if (leaderboard[i].Score < leaderboard[lowestIndex].Score)
            {
                lowestIndex = i;
            }
        }

        return lowestIndex;
    }

    private void SortLeaderboard()
    {
        for (int i = 0; i < leaderboard.Count - 1; i++)
        {
            for (int j = i + 1; j < leaderboard.Count; j++)
            {
                if (leaderboard[j].Score > leaderboard[i].Score)
                {
                    ScoreEntry scoreEntry = leaderboard[i];

                    leaderboard[i] = leaderboard[j];
                    leaderboard[j] = scoreEntry;
                }
            }
        }
    }

    private bool IsValidPlayerId(PlayerId playerId)
    {
        if (playerId.Id < 0)
        {
            Debug.LogError($"PlayerId inválido: {playerId.Id}");
            return false;
        }

        return true;
    }

    #endregion
}