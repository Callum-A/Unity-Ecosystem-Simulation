using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLogController : MonoBehaviour
{
    // Panel we add the text too
    public GameObject Panel;
    // Log prefab AKA the text
    public GameObject LogPrefab;
    // What number log are we
    private int textNum = 0;
    
    /// <summary>
    /// Method to add a log to the event log, simply provide it a string message.
    /// </summary>
    /// <param name="log">Message to add to the log</param>
    /// <param name="color">Color of the text</param>
    public void AddLog(string log, Color color)
    {
        GameObject textGo = Instantiate(LogPrefab);
        textGo.name = $"Log-{textNum}";
        textNum++;
        textGo.transform.SetParent(Panel.transform, false);
        Text myText = textGo.GetComponent<Text>();
        myText.color = color;
        myText.text = log;
    }

    /// <summary>
    /// Method to add a log to the event log, simply provide it a string message.
    /// </summary>
    /// <param name="log">Message to add to the log</param>
    public void AddLog(string log)
    {
        // cant use it as a default arg as it isnt a compile time constant
        AddLog(log, Color.black);
    }
}
