using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/** Determines which state the game is in,
 * and tells other classes if they need to know
 */
public class GameAutomaton : MonoBehaviour
{
    [SerializeField] PublicVars publicVars;

    [SerializeField] private InfoScreen infoScreen;
    [SerializeField] private Countdown countdown;
    [SerializeField] private GameClock clock;
    [SerializeField] private Scoreboard scoreboard;
    [SerializeField] private GameLogic gameLogic;
    [SerializeField] private ExitMenu exitMenu;
    [SerializeField] private BalloonMovement balloon;
    [SerializeField] private SimpleSampleCharacterControl sSCC;
    [SerializeField] private GyroscopeHandler gyro;
    [SerializeField] private GameObject calibrateScreen;


    // all states of the game
    public enum GameStates
    {
        Calibrate,
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
    private System.DateTime startTime;
    private bool firstServantSet = false;
    public bool firstServantReceived = false;
    public bool calibrationStarted = false;

    public bool opIsReady = false;

    // Start is called before the first frame update
    void Start()
    {
        gameState = SetCalibrate();
        startTime = System.DateTime.UtcNow;
    }

    private void Update()
    {
        if (GetGameState() == GameStates.Uninitialized)
        {
            if (!firstServantSet)
            {
                gameLogic.GetFirstServant();
                firstServantSet = true;
            }
            if (firstServantReceived)
            {
                SetEnteringArena();
            }
        }
        else if (GetGameState() == GameStates.Calibrate)
        {

            if (opIsReady && calibrationStarted)
            {
                calibrateScreen.SetActive(false);
                SetUninitialized();
            }
        }
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
            case GameStates.Calibrate:
                transitionAllowed = newState == GameStates.Uninitialized;
                break;
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
            this.gameState = newState;
            Debug.Log("The GameState is now: " + GetGameState());
        }
        else
        {
            Debug.LogError("The GameState " + GetGameState() + " cannot be transitioned to  " + newState);
        }
        // return if transition is allowed
        return transitionAllowed;
    }

    private GameStates SetCalibrate()
    {
        Debug.Log("SetCalibrate");
        calibrateScreen.SetActive(true);
        return GameStates.Calibrate;
    }
    public void StartCalibration()
    {
        Debug.Log("StartCalibration");
        gyro.Calibrate();
        this.calibrationStarted = true;

        // Disable calibrate button
        GameObject button = EventSystem.current.currentSelectedGameObject;
        button.SetActive(false);
    }
    public void SetUninitialized()
    {
        Debug.Log("SetUninitialized");

        SetStateIfAllowed(GameStates.Uninitialized);
    }
    public void SetEnteringArena()
    {
        Debug.Log("SetEnteringArena");
        if (SetStateIfAllowed(GameStates.EnteringArena))
        {
            // call classes who need to know
            infoScreen.StartInfoScreen();
            // (re-)set balloon position
            balloon.ResetBalloon();
            // (re-)set player positions
            bool isServing;
            if (PhotonNetwork.IsMasterClient)
            {
                isServing = gameLogic.getPlayerToHitBalloon() == GameLogic.Players.PlayerOne;
            }
            else
            {
                isServing = gameLogic.getPlayerToHitBalloon() == GameLogic.Players.PlayerTwo;
            }
            Debug.Log("Prepare for serve as master(" + PhotonNetwork.IsMasterClient + "). Serving=" + isServing);
            sSCC.ResetPlayer(isServing);
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

            GameObject.Find("HUD/Canvas/Abilities").SetActive(true);
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

            // Remove character
            string characterObject = PlayerPrefs.GetString("CharacterName");
            Destroy(GameObject.Find(characterObject + "(Clone)"));
        }
    }

    public void ResetGameForNextServe()
    {
        SetEnteringArena();
    }
}
