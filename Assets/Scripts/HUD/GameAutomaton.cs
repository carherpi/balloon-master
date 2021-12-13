using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/** Determines which state the game is in,
 * and tells other classes if they need to know
 */
public class GameAutomaton : MonoBehaviour
{
    [SerializeField] private InfoScreen infoScreen;
    [SerializeField] private Countdown countdown;
    [SerializeField] private GameClock clock;
    [SerializeField] private Scoreboard scoreboard;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private ExitMenu exitMenu;
    [SerializeField] private BalloonMovement balloon;


    // all states of the game
    public enum GameStates
    {
        Uninitialized,
        EnteringArena,
        CountdownRunning,
        GameRunning,
        GameEnded,
        LeavingArena
    }

    private GameStates gameState;

    // results of the game
    public enum Results
    {
        Win,
        Draw,
        Defeat
    }
    private Results result;


    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStates.Uninitialized;
        SetEnteringArena();
    }

    public GameStates GetGameState()
    {
        return gameState;
    }
    private bool SetStateIfAllowed(GameStates newState)
    {
        bool transitionAllowed;
        // set state if transition is allowed
        switch (GetGameState())
        {
            case GameStates.Uninitialized:
                transitionAllowed = newState == GameStates.EnteringArena;
                break;
            case GameStates.EnteringArena:
                transitionAllowed = newState == GameStates.CountdownRunning;
                break;
            case GameStates.CountdownRunning:
                transitionAllowed = newState == GameStates.GameRunning;
                break;
            case GameStates.GameRunning:
                transitionAllowed = newState == GameStates.GameEnded || newState == GameStates.EnteringArena;
                break;
            case GameStates.GameEnded:
                transitionAllowed = newState == GameStates.LeavingArena;
                break;
            case GameStates.LeavingArena:
                transitionAllowed = false;
                break;

            default:
                Debug.LogError("This GameState is not implemented in switch: " + GetGameState());
                transitionAllowed = false;
                break;
        }
        // log if transition not allowed
        if (transitionAllowed)
        {
            gameState = newState;
            Debug.Log("The GameState is now " + GetGameState());
        }
        else
        {
            Debug.LogError("The GameState " + GetGameState() + " cannot be transitioned to  " + newState);
        }
        // return if transition is allowed
        return transitionAllowed;
    }

    public void SetEnteringArena()
    {
        if (SetStateIfAllowed(GameStates.EnteringArena))
        {
            // call classes who need to know
            infoScreen.StartInfoScreen();
        }
    }

    public void SetCountdownRunning()
    {
        if (SetStateIfAllowed(GameStates.CountdownRunning))
        {
            // call classes who need to know
            clock.ShowClock();
            scoreboard.ShowScoreboard();
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
            exitMenu.StartExitMenu(scoreboard.GetLeader());
        }
    }

    public void SetLeavingArena()
    {
        if (SetStateIfAllowed(GameStates.LeavingArena))
        {
            // call classes who need to know

            // switch to Main Menu
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ResetGameForNextServe()
    {
        SetEnteringArena();
        balloon.ResetBalloon();
        // TODO ResetPositionsOfPlayers()
    }
}
