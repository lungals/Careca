using TMPro;
using UnityEngine;

namespace Assets.Scripts.Bola
{
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
            UpdateScoreInUI(amount);
        }

        public void UpdateScoreInUI(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
