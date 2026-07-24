using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_FontAsset leadDigitFont;

    private int displayedScore = -1;
    private int displayedCombo = -1;

    private void Awake()
    {
        NumberDisplay.RegisterLeadDigitFont(leadDigitFont);
    }

    // Update is called once per frame
    void Update()
    {
        int score = GameManager.Instance.Score;
        if (score != displayedScore)
        {
            displayedScore = score;
            scoreText.text = NumberDisplay.Format(score, leadDigitFont);
        }

        int combo = GameManager.Instance.Combo;
        if (combo != displayedCombo)
        {
            displayedCombo = combo;
            comboText.text = NumberDisplay.Format(combo, leadDigitFont);
        }
    }
}
