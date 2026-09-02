using Assets.Scripts.Bola;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private ScoreUI scorePrefab;
    [SerializeField] private Transform scoreParent;

    private readonly Dictionary<PlayerId, ScoreUI> scoresPerPlayer = new();

    private void Start()
    {
        PlayerId[] players = PlayerRegister.GetAllPlayersConnected();

        foreach (PlayerId player in players)       
            CreateScoreUI(player);

        PlayerRegister.OnPlayerRegister += PlayerRegister_OnPlayerRegister;
        PlayerRegister.OnPlayerUnregister += PlayerRegister_OnPlayerUnregister;
    }

    private void OnDestroy()
    {
        PlayerRegister.OnPlayerRegister -= PlayerRegister_OnPlayerRegister;
        PlayerRegister.OnPlayerUnregister -= PlayerRegister_OnPlayerUnregister;
    }

    private void PlayerRegister_OnPlayerRegister(PlayerId playerId)
    {
        CreateScoreUI(playerId);
    }

    private void PlayerRegister_OnPlayerUnregister(PlayerId playerId)
    {
        RemoveScoreUI(playerId);
    }

    private void CreateScoreUI(PlayerId playerId)
    {
        ScoreUI instance = Instantiate(scorePrefab, scoreParent);
        instance.Init(playerId);
        scoresPerPlayer.Add(playerId, instance);
    }

    private void RemoveScoreUI(PlayerId playerId)
    {
        if (!scoresPerPlayer.ContainsKey(playerId))
            return;

        ScoreUI instance = scoresPerPlayer[playerId];
        Destroy(instance);
        scoresPerPlayer.Remove(playerId);
    }

    public void IncreaseScore(PlayerId playerId, int score)
    {
        if (!scoresPerPlayer.ContainsKey(playerId))
            return;

        scoresPerPlayer[playerId].IncreaseScore(score);
    }
}