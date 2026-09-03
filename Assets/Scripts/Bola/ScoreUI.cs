using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private PlayerId ownerPlayer;

    private int score;

    public void Init(PlayerId playerId)
    {
        ownerPlayer = playerId;
    }
    public void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreInUI(score);
    }

    public void UpdateScoreInUI(int score)
    {
        scoreText.text = score.ToString();
    }
}