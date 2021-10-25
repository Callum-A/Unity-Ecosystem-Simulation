using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLogController : MonoBehaviour
{
    // Unity GUI element
    public Text GUIText;
    // List of all logs
    private readonly List<string> logs = new List<string>();
    // How many we keep in the above list
    private readonly int maxLogs = 5;
    
    /// <summary>
    /// Method to add a log to the event log, simply provide it a string message.
    /// </summary>
    /// <param name="log">Message to add to the log</param>
    public void AddLog(string log)
    {
        logs.Add(log);

        if (logs.Count > maxLogs)
        {
            logs.RemoveAt(0);
        }

        string text = "";
 
        foreach (string logEvent in logs)
        {
            text += logEvent;
            text += "\n";
        }

        GUIText.text = text;
    }
}
