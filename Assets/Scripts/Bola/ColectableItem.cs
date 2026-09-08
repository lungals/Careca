using UnityEngine;

public class ColectableItem : MonoBehaviour, IInteractableObject
{
    [SerializeField] private int points;

    public void Interact(PlayerId playerId)
    {
        ScoreManager.Instance.IncreaseScoreRPC(playerId, points);
        Destroy(gameObject);
    }
}
