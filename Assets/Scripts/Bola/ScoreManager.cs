using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private LeaderboardUI leaderboardUI;

    private readonly Dictionary<PlayerId, int> scores = new();

    public NetworkVariable<ScorePoint> Testando = new();

    public struct ScorePoint : INetworkSerializable
    {
        public PlayerId PlayerId;
        public int Score;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref Score);
        }
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        
    }

    public override void OnNetworkSpawn()
    {
        Testando.OnValueChanged += Teste;
        base.OnNetworkSpawn();

        Debug.Log("Se inscreveu");
    }
    public override void OnNetworkDespawn()
    {
        Testando.OnValueChanged -= Teste;
        base.OnNetworkDespawn();
    }

    private void Teste(ScorePoint lastValue, ScorePoint newValue)
    {
        if (!scores.ContainsKey(newValue.PlayerId))
        {
            scores[newValue.PlayerId] = newValue.Score;
        }
        else
        {
            scores[newValue.PlayerId] += newValue.Score;
        }

       /* if (!scores.ContainsKey(newValue.PlayerId))
            return;*/

        leaderboardUI.UpdateScore(newValue.PlayerId, scores[newValue.PlayerId]);
        Debug.Log("Nem chamou");
    }

    [Rpc(SendTo.Server)]
    public void IncreaseScoreRPC(PlayerId playerId, int amount)
    {
        if (playerId == null)
        {
            Debug.Log("É null");
        }

        ScorePoint score = new ScorePoint();
        score.Score = amount;
        score.PlayerId = playerId;

        Testando.Value = score;
        return;

        if (!IsServer)
            return;

        if (!scores.ContainsKey(playerId))
            return;


        scores[playerId] += amount;
        leaderboardUI.UpdateScore(playerId, amount);
    }
}