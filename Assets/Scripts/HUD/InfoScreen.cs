using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{
    [SerializeField]
    private GameAutomaton gameAutomaton;
    [SerializeField]
    private GameLogic gameLogic;
    [SerializeField]
    public int pointsToWin, visibilityTimeInSec;
    [SerializeField]
    private Text points, servant;
    private bool infoScreenShown;
    private System.DateTime startTime;



    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
        infoScreenShown = false;
    }

    public void StartInfoScreen()
    {
        points.text = pointsToWin.ToString();
        if (gameLogic.getPlayerToHitBalloon() == GameLogic.Players.PlayerOne)
        {
            servant.text = "1";
        }
        else
        {
            servant.text = "2";
        }
        this.gameObject.SetActive(true);
        infoScreenShown = true;
        startTime = System.DateTime.UtcNow;
    }

    public void StopInfoScreen()
    {
        this.gameObject.SetActive(false);
        infoScreenShown = false;
        gameAutomaton.SetCountdownRunning();
    }

    private void Update()
    {
        if (infoScreenShown)
        {
            System.TimeSpan elapsedTime = System.DateTime.UtcNow - startTime;
            if (elapsedTime.Seconds >= visibilityTimeInSec)
            {
                StopInfoScreen();
            }
        }
    }
}
