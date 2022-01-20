using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    [SerializeField] private GameAutomaton gameAutomaton;
    [SerializeField] private SimpleSampleCharacterControl sSCC;
    [SerializeField] private Scoreboard scoreboard;
    [SerializeField] private Text serveIndicationPl1, serveIndicationPl2;

    public GameObject balloon;

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
    public void SetPlayerToHitBalloon(Players playerToHitBalloon)
    {
        this.playerToHitBalloon = playerToHitBalloon;
        sSCC.SetBalloonCollisionActive(playerToHitBalloon);
        // set scoreboard indication who serves
        if (playerToHitBalloon == Players.PlayerOne)
        {
            serveIndicationPl1.text = "->";
            serveIndicationPl2.text = "";

            // Update each time for PlayerOne
            balloon.GetComponent<MeshRenderer>().material.color = Color.cyan;
            
        }
        else
        {
            serveIndicationPl1.text = "";
            serveIndicationPl2.text = "->";

            // Update each time for PlayerTwo
            balloon.GetComponent<MeshRenderer>().material.color = Color.red;
            
            
        }
    }

    public Players GetOtherPlayer(Players player)
    {
        if (player == Players.PlayerOne)
        {
            return Players.PlayerTwo;
        }
        return Players.PlayerOne;
    }

    private PhotonView pV;
    private Players master;

    // Start is called before the first frame update
    void Start()
    {
        pV = this.GetComponent<PhotonView>();
    }
    public void GetFirstServant()
    {
        // set player for first serve
        if (PhotonNetwork.IsMasterClient)
        {
            Players servant;
            if (Random.Range(1, 3) == 1)
            {
                servant = Players.PlayerOne;
                PlayerPrefs.SetString("WhoAmI", "PlayerOne");
                GameObject.Find("Circle_Color").GetComponent<SpriteRenderer>().color = Color.cyan;
                master = servant;
            }
            else
            {
                servant = Players.PlayerTwo;
                PlayerPrefs.SetString("WhoAmI", "PlayerTwo");
                GameObject.Find("Circle_Color").GetComponent<SpriteRenderer>().color = Color.red;
                master = servant;
            }
            PlayerPrefs.Save();

            Debug.Log("Send Servant is " + servant);
            pV.RPC("SetFirstServant", RpcTarget.All, servant);
        }
    }
    [PunRPC]
    public void SetFirstServant(Players servant)
    {

        SetPlayerToHitBalloon(servant);
        gameAutomaton.firstServantReceived = true;
        Debug.Log("Received servant is " + servant);

        // Set player colors for opponent
        if (servant == Players.PlayerOne && servant != master)
        {
            PlayerPrefs.SetString("WhoAmI", "PlayerTwo");
            GameObject.Find("Circle_Color").GetComponent<SpriteRenderer>().color = Color.red;
        } else if (servant == Players.PlayerTwo && servant != master)
        {
            PlayerPrefs.SetString("WhoAmI", "PlayerOne");
            GameObject.Find("Circle_Color").GetComponent<SpriteRenderer>().color = Color.cyan;
        }
    }

    // Call this function when a player hits the balloon
    public void BroadcastBalloonHitBy(Players player)
    {
        Debug.Log("Send " + player);
        pV.RPC("BalloonHitBy", RpcTarget.All, player);
    }
    [PunRPC]
    public void BalloonHitBy(Players player)
    {
        Debug.Log(player + " hit the balloon.");
        

        if (gameAutomaton.GetGameState() == GameAutomaton.GameStates.GameRunning)
        {
            // is player allowed to hit the ball?
            if (player != getPlayerToHitBalloon())
            {
                Debug.LogError(player + " cannot hit the balloon if it the turn of " + getPlayerToHitBalloon());
            }
            // set next player to hit the ball
            SetPlayerToHitBalloon(GetOtherPlayer(player));
        }
    }

    // Call this function when the balloon hits the ground
    public void BroadcastBalloonHitGround()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Send BalloonHitGround");
            pV.RPC("BalloonHitGround", RpcTarget.All);
        }
    }
    [PunRPC]
    public void BalloonHitGround()
    {
        if (gameAutomaton.GetGameState() == GameAutomaton.GameStates.GameRunning)
        {
            // add point for the player who does not have to hit the balloon next
            scoreboard.AddPointForPlayer(GetOtherPlayer(getPlayerToHitBalloon()));
            gameAutomaton.ResetGameForNextServe();
        }
    }

    [PunRPC]
    public void OpponentIsReady(bool isReady)
    {
        GameObject.Find("GameManager").GetComponent<GameAutomaton>().opIsReady = isReady;
    }
}
