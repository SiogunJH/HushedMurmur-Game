using UnityEngine;
using TMPro;

public static class TMPTextExtensions
{
    public static int GetAmountOfLineBreaks(this TextMeshProUGUI tmpText)
    {
        if (tmpText == null) return 0;
        return tmpText.text.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None).Length;
    }
}
