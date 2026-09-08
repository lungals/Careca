using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI playerIdText;

    private PlayerId ownerPlayer;

    public void Init(PlayerId playerId)
    {
        ownerPlayer = playerId;

        if (ownerPlayer != null && ownerPlayer != playerId)
        {
            Debug.Log("Diferente isso aqui: " + playerId);
        }

        playerIdText.text = playerId.Id.ToString();
    }

    public void UpdateScoreView(int score)
    {
        scoreText.text = score.ToString();
        Debug.Log("Atualizouuu" + score);
    }
}