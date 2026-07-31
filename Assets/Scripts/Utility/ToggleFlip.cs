using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mirrors a Toggle's artwork horizontally to show its on state, so one sprite
/// covers both knob positions instead of needing a second drawing.
/// Unity's Toggle stacks a checkmark over a background rather than swapping,
/// which double-draws when the artwork has a transparent interior.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleFlip : MonoBehaviour
{
    [Tooltip("The switch graphic to mirror - usually the Background image")]
    [SerializeField] private RectTransform target;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(Apply);
    }

    // Refreshed on enable as well as on change, so opening the panel draws the
    // saved state rather than whatever the prefab was left at.
    private void OnEnable()
    {
        if (toggle != null)
        {
            Apply(toggle.isOn);
        }
    }

    private void Apply(bool isOn)
    {
        if (target == null)
        {
            return;
        }

        // Reading the magnitude back off the current scale means whatever size
        // the switch was set to in the Inspector survives the flip.
        Vector3 scale = target.localScale;
        scale.x = isOn ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        target.localScale = scale;
    }
}
