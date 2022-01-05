using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    /*
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    */

    #region Photon Callbacks


    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }


    #endregion


    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
        if (PlayerManager.LocalPlayerInstance == null)
        {

            string characterName = PlayerPrefs.GetString("CharacterName");

            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);


            /* boy
            GameObject boyClient = PhotonNetwork.Instantiate(characterName, this.gameObject.GetComponent<SimpleSampleCharacterControl>().spawnWaitPlayerPos, this.gameObject.GetComponent<SimpleSampleCharacterControl>().spawnWaitPlayerRot, 0);
            this.gameObject.GetComponent<SimpleSampleCharacterControl>().SetPlayer(boyClient);
            */

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            GameObject Character = PhotonNetwork.Instantiate(characterName, this.gameObject.GetComponent<SimpleSampleCharacterControl>().spawnWaitPlayerPos, this.gameObject.GetComponent<SimpleSampleCharacterControl>().spawnWaitPlayerRot, 0);
            this.gameObject.GetComponent<SimpleSampleCharacterControl>().SetPlayer(Character);
            
            
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Private Methods

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Arena");
    }

    #endregion

    #region Photon Callbacks


    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            // TODO: Question from Marc: Don't you want to load the main menu if the other player left?
            LoadArena();
        }
    }


    #endregion
}
