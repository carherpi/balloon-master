using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField]
    private GameAutomaton gameAutomaton;
    private System.DateTime startTime;
    private bool countdownRunning;

    void Start()
    {
        this.gameObject.SetActive(false);
        this.gameObject.GetComponent<Text>().text = "";
        countdownRunning = false;
    }

    public void StartCountdown()
    {
        this.gameObject.SetActive(true);
        countdownRunning = true;
        startTime = System.DateTime.UtcNow;
    }

    void Update()
    {
        if (countdownRunning)
        {
            // get elapsed time
            System.TimeSpan elapsedTime = System.DateTime.UtcNow - startTime;
            if (elapsedTime.Seconds < 1)
            {
                this.gameObject.GetComponent<Text>().text = "3";
            }
            else if (elapsedTime.Seconds < 2)
            {
                this.gameObject.GetComponent<Text>().text = "2";
            }
            else if (elapsedTime.Seconds < 3)
            {
                this.gameObject.GetComponent<Text>().text = "1";
            }
            else if (elapsedTime.Seconds < 4)
            {
                this.gameObject.GetComponent<Text>().text = "Start";
            }
            else
            {
                this.gameObject.GetComponent<Text>().text = "";
                countdownRunning = false;
                gameAutomaton.SetGameRunning();
            }
        }
    }
}
