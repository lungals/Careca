using UnityEngine;

public class ColectableItem : MonoBehaviour, IInteractableObject
{
    [SerializeField] private int points;

    public void Interact()
    {
        ScoreManager.Instance.IncreaseScore(points);
        Destroy(gameObject);
    }
}
