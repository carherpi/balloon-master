using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    [SerializeField]
    private Text exitMsg;
    private readonly string msgWin = "Too easy! You crushed him!";
    private readonly string msgDraw = "A draw! I smell a rematch!";
    private readonly string msgDefeat = "You lost! I expect a better you!";
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void StartExitMenu(GameAutomaton.Results result)
    {
        this.gameObject.SetActive(true);

        if (GameAutomaton.Results.Win == result)
        {
            // this player won
            exitMsg.text = msgWin;
        }
        else if (GameAutomaton.Results.Draw == result)
        {
            // draw (game ended on time)
            exitMsg.text = msgDraw;
        }
        else
        {
            // opponent won
            exitMsg.text = msgDefeat;
        }
    }
}
