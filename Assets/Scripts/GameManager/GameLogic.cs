using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField]
    private Scoreboard scoreboard;

    // all states of the game
    public enum Players
    {
        PlayerOne,
        PlayerTwo
    }
    public Players playerToHitBalloon;

    // Start is called before the first frame update
    void Start()
    {
        // set player for first serve
        if (Random.value > 0.5f)
        {
            playerToHitBalloon = Players.PlayerOne;
        }
        else
        {
            playerToHitBalloon = Players.PlayerTwo;
        }
    }

    public void BalloonHitBy(Players player)
    {
        if (player != playerToHitBalloon)
        {
            Debug.LogError(player + " cannot hit the balloon if it is not his turn.");
        }
        playerToHitBalloon = player;
    }

    public void BallHitGround()
    {

    }
}
