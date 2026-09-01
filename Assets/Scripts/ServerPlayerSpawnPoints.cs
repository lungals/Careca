using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServerPlayerSpawnPoints : Singleton<ServerPlayerSpawnPoints>
{
    [SerializeField]
    private List<GameObject> m_SpawnPoints;

    public List<GameObject> SpawnPoints { get => m_SpawnPoints; set => m_SpawnPoints = value; }

    public GameObject GetRandomSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0)
            return null;
        return m_SpawnPoints[Random.Range(0, m_SpawnPoints.Count)];
    }
}