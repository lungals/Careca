using UnityEngine;

public class ColectableItem : MonoBehaviour, IInteractableObject
{
    [SerializeField] private int points;

    public void Interact(PlayerId playerId)
    {
        ScoreManager.Instance.AddScoreRPC(playerId, points);
        Destroy(gameObject);
    }
}
