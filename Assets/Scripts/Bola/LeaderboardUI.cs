using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField]
    private ScoreUI scorePrefab;

    [SerializeField]
    private Transform scoreParent;

    private readonly Dictionary<PlayerId, ScoreUI> scoresPerPlayer = new();

    private ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.Instance;


        scoreManager.Leaderboard.OnListChanged += ScoreManager_OnLeaderboardChanged;

        Refresh();
    }

    private void OnDestroy()
    {
        if (scoreManager != null)
        {
            scoreManager.Leaderboard.OnListChanged -= ScoreManager_OnLeaderboardChanged;
        }
    }

    private void ScoreManager_OnLeaderboardChanged(NetworkListEvent<ScoreEntry> changeEvent)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (scoreManager == null)
            return;

        NetworkList<ScoreEntry> leaderboard = scoreManager.Leaderboard;

        RemovePlayersNotInLeaderboard(leaderboard);
        
        for (int i = 0; i < leaderboard.Count; i++)
        {
            ScoreEntry entry = leaderboard[i];

            if (!scoresPerPlayer.TryGetValue(entry.PlayerId, out ScoreUI scoreUI))
            {
                scoreUI = CreateScoreUI(entry.PlayerId);
            }

            scoreUI.UpdateScoreView(entry.Score);
        }

        RefreshUIOrder(leaderboard);
    }

    private ScoreUI CreateScoreUI(PlayerId playerId)
    {
        ScoreUI instance = Instantiate(scorePrefab, scoreParent);
        instance.Init(playerId);

        scoresPerPlayer.Add(playerId, instance);

        return instance;
    }

    private void RemovePlayersNotInLeaderboard(NetworkList<ScoreEntry> leaderboard)
    {
        List<PlayerId> playersToRemove = new();

        foreach (var pair in scoresPerPlayer)
        {
            bool found = false;

            for (int i = 0; i < leaderboard.Count; i++)
            {
                if (leaderboard[i].PlayerId == pair.Key)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                playersToRemove.Add(pair.Key);
        }

        foreach (PlayerId playerId in playersToRemove)
        {
            RemoveScoreUI(playerId);
        }
    }

    private void RemoveScoreUI(PlayerId playerId)
    {
        if (!scoresPerPlayer.TryGetValue(playerId, out ScoreUI instance))
            return;

        Destroy(instance.gameObject);

        scoresPerPlayer.Remove(playerId);
    }

    private void RefreshUIOrder(NetworkList<ScoreEntry> leaderboard)
    {
        for (int i = 0; i < leaderboard.Count; i++)
        {
            PlayerId playerId = leaderboard[i].PlayerId;

            if (!scoresPerPlayer.TryGetValue(playerId, out ScoreUI scoreUI))
                continue;

            scoreUI.transform.SetSiblingIndex(i);
        }
    }
}
