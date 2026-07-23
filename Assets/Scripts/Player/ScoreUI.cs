using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ScoreUI : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text scoreTextFinal;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text highScoreText;

    // Update is called once per frame
    void Update()
    {
        int score = GameManager.Instance.Score;
        scoreText.text = $"{score}";
        scoreTextFinal.text = $"{score}";

        int combo = GameManager.Instance.Combo;
        comboText.text = $"{combo}";

        int highScore = GameManager.Instance.highScore;
        highScoreText.text = $"{highScore}";
    }
}
