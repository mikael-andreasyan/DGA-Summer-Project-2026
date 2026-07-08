using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;

    // Update is called once per frame
    void Update()
    {
        int score = GameManager.Instance.Score;
        scoreText.text = $"Score: {score}";

        int combo = GameManager.Instance.Combo;
        comboText.text = $"Combo: {combo}";
    }
}
