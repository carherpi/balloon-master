using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    [SerializeField]
    private GameAutomaton gameAutomaton;
    [SerializeField]
    private int totalTimeMinutes, totalTimeSeconds, changeColorForNLastSec;
    [SerializeField]
    string LastSecondsColorCode;
    private System.DateTime startTime;
    private bool gameRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        UpdateTextBox(totalTimeMinutes, totalTimeSeconds);
    }

    public void StartClock()
    {
        startTime = System.DateTime.UtcNow;
        gameRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            // get elapsed time
            System.TimeSpan elapsedTime = System.DateTime.UtcNow - startTime;
            int elapsedMin = elapsedTime.Minutes;
            int elapsedSec = elapsedTime.Seconds;
            // calculate remaining time
            int remainingMin = totalTimeMinutes - elapsedMin;
            int remainingSec = totalTimeSeconds - elapsedSec;
            while (remainingSec < 0)
            {
                remainingSec += 60;
                remainingMin -= 1;
            }
            // change color of numbers for the last seconds
            if (remainingMin == 0 && remainingSec <= changeColorForNLastSec)
            {
                Color redNumbers;
                ColorUtility.TryParseHtmlString(LastSecondsColorCode, out redNumbers);
                this.gameObject.GetComponent<Text>().color = redNumbers;
            }
            // if time runs out
            if (remainingMin < 0)
            {
                gameAutomaton.SetGameEnded();
            }
            else
            {
                UpdateTextBox(remainingMin, remainingSec);
            }
        }
    }

    private void UpdateTextBox(int remainingMin, int remainingSec)
    {
        string updatedTime = remainingMin + ":";
        if (remainingSec < 10)
        {
            updatedTime += "0";
        }
        updatedTime += remainingSec;
        this.gameObject.GetComponent<Text>().text = updatedTime;
    }

    /* This function should be called when the game ends. It stops the clock */
    public void endClock()
    {
        gameRunning = false;
    }
}
