using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MotionDetectionStationScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] textRows;

    public void ClearAll()
    {
        foreach (var textRow in textRows)
            textRow.text = ">";
    }

    public void PushAlert(string message)
    {
        for (int i = 0; i < textRows.Length - 1; i++)
            textRows[i].text = textRows[i + 1].text;

        textRows[textRows.Length - 1].text = message;

    }
}
