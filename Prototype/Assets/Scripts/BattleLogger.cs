using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BattleLogger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    private List<string> logs = new List<string>();
    [SerializeField] private int maxLines = 5;

    public void AddEntry(string message)
    {
        logs.Add(message);
        if (logs.Count > maxLines) logs.RemoveAt(0);
        logText.text = string.Join("\n", logs.ToArray());
    }
}