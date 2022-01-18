using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private GameAutomaton gameAutomaton;
    [SerializeField] private InfoScreen infoScreen;
    [SerializeField] private Text scorePlayer1, scorePlayer2;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
        scorePlayer1.text = "0";
        scorePlayer2.text = "0";
    }

    public void ShowScoreboard()
    {
        this.gameObject.SetActive(true);
    }

    public void AddPointForPlayer(GameLogic.Players player)
    {
        if (player == GameLogic.Players.PlayerOne)
        {
            AddPointToText(player, scorePlayer1);
        }
        else if (player == GameLogic.Players.PlayerTwo)
        {
            AddPointToText(player, scorePlayer2);
        }
        else
        {
            Debug.LogError("We don't expect this player here: " + player);
        }
    }

    private void AddPointToText(GameLogic.Players player, Text scoreText)
    {
        int currentScore = int.Parse(scoreText.text);
        currentScore += 1;
        scoreText.text = currentScore.ToString();
        if (currentScore >= infoScreen.pointsToWin)
        {
            Debug.Log("This player has reached the point limit to win: " + player);
            gameAutomaton.SetGameEnded();
        }
    }

    // called from Ability_ExtraPoint
    public void SubtractPointToText(string whoami)
    {
        
        if (whoami == "PlayerOne")
        {
            int currentScore = int.Parse(scorePlayer2.text);
            currentScore -= 1;
            scorePlayer2.text = currentScore.ToString();
        } else
        {
            int currentScore = int.Parse(scorePlayer1.text);
            currentScore -= 1;
            scorePlayer1.text = currentScore.ToString();
        }
    }

    public GameAutomaton.Results GetLeader()
    {
        int pointsPl1 = int.Parse(scorePlayer1.text);
        int pointsPl2 = int.Parse(scorePlayer2.text);
        bool iAmPl1 = PhotonNetwork.IsMasterClient;
        if ((pointsPl1 > pointsPl2 && iAmPl1) || (pointsPl1 < pointsPl2 && !iAmPl1))
        {
            return GameAutomaton.Results.Win ;
        }
        else if (pointsPl1 == pointsPl2)
        {
            return GameAutomaton.Results.Draw;
        }
        return GameAutomaton.Results.Defeat;
    }
}
