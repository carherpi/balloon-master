using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Determines which state the game is in,
 * and tells other classes if they need to know
 */
public class GameAutomaton : MonoBehaviour
{
    [SerializeField]
    private GameClock clock;
    [SerializeField]
    private Countdown countdown;
    [SerializeField]
    private Scoreboard scoreboard;
    [SerializeField]
    private GameLogic gameLogic;

    // all states of the game
    public enum GameStates
    {
        EnteringArena,
        CountdownRunning,
        GameRunning,
        GameEnded,
        LeavingArena
    }
    private GameStates gameState;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.EnteringArena;
        SetCountdownRunning();
    }

    public GameStates getGameState()
    {
        return gameState;
    }
    private bool SetStateIfAllowed(GameStates newState)
    {
        bool transitionAllowed;
        // set state if transition is allowed
        switch (getGameState())
        {
            case GameStates.EnteringArena:
                transitionAllowed = newState == GameStates.CountdownRunning;
                break;
            case GameStates.CountdownRunning:
                transitionAllowed = newState == GameStates.GameRunning;
                break;
            case GameStates.GameRunning:
                transitionAllowed = newState == GameStates.GameEnded;
                break;
            case GameStates.GameEnded:
                transitionAllowed = newState == GameStates.LeavingArena;
                break;
            case GameStates.LeavingArena:
                transitionAllowed = false;
                break;

            default:
                Debug.LogError("This GameState is not implemented in switch: " + getGameState());
                transitionAllowed = false;
                break;
        }
        // log if transition not allowed
        if (transitionAllowed)
        {
            gameState = newState;
            Debug.Log("The GameState is now " + getGameState());
        }
        else
        {
            Debug.LogError("The GameState " + getGameState() + " cannot be transitioned to  " + newState);
        }
        // return if transition is allowed
        return transitionAllowed;
    }

    public void SetCountdownRunning()
    {
        if (SetStateIfAllowed(GameStates.CountdownRunning))
        {
            // call classes who need to know
            countdown.StartCountdown();
        }
    }

    public void SetGameRunning()
    {
        if (SetStateIfAllowed(GameStates.GameRunning))
        {
            // call classes who need to know
            clock.StartClock();
        }
    }

    public void SetGameEnded()
    {
        if (SetStateIfAllowed(GameStates.GameEnded))
        {
            // call classes who need to know
            clock.endClock();
        }
    }

    public void SetLeavingArena()
    {
        if (SetStateIfAllowed(GameStates.LeavingArena))
        {
            // call classes who need to know
        }
    }

}
