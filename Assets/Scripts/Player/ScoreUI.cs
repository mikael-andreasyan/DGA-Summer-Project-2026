using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class ScoreUI : MonoBehaviour
{

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text currenthighScoreText;

    // Update is called once per frame
    void Update()
    {
        int score = GameManager.Instance.Score;
        scoreText.text = $"{score}";
        finalScoreText.text = $"{score}";

        int currenthighScore = GameManager.Instance.highScore;
        currenthighScoreText.text = $"{currenthighScore}";

        int combo = GameManager.Instance.Combo;
        comboText.text = $"{combo}";
    }
}
