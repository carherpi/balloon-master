using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField]
    private GameAutomaton gameAutomaton;
    [SerializeField]
    private Scoreboard scoreboard;

    // all states of the game
    public enum Players
    {
        PlayerOne,
        PlayerTwo
    }
    private Players playerToHitBalloon;
    public Players getPlayerToHitBalloon()
    {
        return playerToHitBalloon;
    }

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

    // Call this function when a player hits the balloon
    public void BalloonHitBy(Players player)
    {
        if (gameAutomaton.GetGameState() == GameAutomaton.GameStates.GameRunning)
        {
            if (player != playerToHitBalloon)
            {
                Debug.LogError(player + " cannot hit the balloon if it is not his turn.");
            }
            playerToHitBalloon = player;
        }
    }

    // Call this function when the balloon hits the ground
    public void BalloonHitGround()
    {
        // add point for the player who does not have to hit the balloon next
        if (playerToHitBalloon == Players.PlayerOne)
        {
            scoreboard.AddPointForPlayer(Players.PlayerTwo);
        }
        else
        {
            scoreboard.AddPointForPlayer(Players.PlayerOne);
        }
    }
}
