using UnityEngine;
using TMPro;

public class PlayerCombo : MonoBehaviour
{

    private TMP_Text comboText;

    private void Awake()
    {
        comboText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int combo = GameManager.Instance.Combo;
        comboText.text = combo > 0 ? $"{combo}" : "";
    }
}
