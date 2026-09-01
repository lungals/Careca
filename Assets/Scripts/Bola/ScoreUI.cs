using TMPro;
using UnityEngine;

namespace Assets.Scripts.Bola
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;

        public void UpdateScoreInUI(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}
