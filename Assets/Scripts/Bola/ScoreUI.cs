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

    public void UpdateScoreView(int score)
    {
        scoreText.text = score.ToString();
    }
}