using TMPro;

/// <summary>
/// Number formatting shared by the on-screen score and combo readouts.
/// </summary>
public static class NumberDisplay
{
    /// <summary>
    /// Makes so the big number font does not need to live in Resource folder
    /// </summary>
    public static void RegisterLeadDigitFont(TMP_FontAsset leadDigitFont)
    {
        MaterialReferenceManager.AddFontAsset(leadDigitFont);
    }

    /// <summary>
    /// Draws leading digit if not zero to be the big number
    /// </summary>
    public static string Format(int value, TMP_FontAsset leadDigitFont)
    {
        string digits = value.ToString();

        if (value <= 0)
            return digits;

        return $"<font=\"{leadDigitFont.name}\">{digits[0]}</font>{digits.Substring(1)}";
    }
}
