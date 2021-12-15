using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MultiplayerLauncher : MonoBehaviourPunCallbacks, IMatchmakingCallbacks

{
    public TextMeshProUGUI roomTextMesh;

    #region Private Serializable Fields

    [SerializeField] private DisplayRoomNumber displayRoomNumber;
    [SerializeField] private Button buttonCreateRoom, buttonJoinRoom, buttonQuickMatch, buttonSinglePlayer;

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    #endregion

    #region Public Serializable Fields

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    public GameObject progressLabel;

    #endregion


    #region Private Fields


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    public enum MultiplayerMode
    {
        NotInitialized,
        CreateRoom,
        JoinRoom,
        QuickMatch,
        SinglePlayer
    }
    private MultiplayerMode multiplayerMode;

    private int curQuickMatches;
    #endregion


    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        // initialize multiplayer mode
        multiplayerMode = MultiplayerMode.NotInitialized;
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }


    #endregion


    #region MonoBehaviourPunCallbacks Callbacks


    
    public override void OnConnectedToMaster()
    {
        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.      
        Debug.Log("OnConnectedToMaster(): isConnecting=" + isConnecting + ". PhotonNetwork.IsConnected=" + PhotonNetwork.IsConnected);
        
        if (isConnecting)
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()

            RoomOptions roomOptions = GetRoomOptions();
            curQuickMatches = 1;

            switch (multiplayerMode)
            {
                // TODO: Should we delete this button?
                case MultiplayerMode.CreateRoom:
                    CreateRoom(roomOptions);
                    break;
                case MultiplayerMode.JoinRoom:
                    PhotonNetwork.JoinRoom(roomTextMesh.text);
                    Debug.Log("MultiplayerLauncher:ButtonJoinRoom() room joined");
                    // TODO OnJoinRoomFailed()
                    break;
                case MultiplayerMode.QuickMatch:
                    PhotonNetwork.JoinOrCreateRoom("QM_" + curQuickMatches.ToString(), roomOptions, TypedLobby.Default);
                    // TODO OnXXXRoomFailed()
                    break;
                case MultiplayerMode.SinglePlayer:
                    roomOptions.MaxPlayers = 1;
                    PhotonNetwork.JoinOrCreateRoom("SP_" + Random.Range(0, 10000).ToString(), roomOptions, TypedLobby.Default);
                    // TODO OnXXXRoomFailed()
                    break;
                default:
                    Debug.LogError("MultiplayerMode is not properly set: " + multiplayerMode);
                    break;
            }
            SetButtonsInteractable(true);
            isConnecting = false;
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnecting = false;

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("MultiplayerLauncher:OnCreatedRoom() room created: " + PhotonNetwork.CurrentRoom.Name);
        if (multiplayerMode == MultiplayerMode.CreateRoom)
        {
            // update menu
            displayRoomNumber.RoomCreated(PhotonNetwork.CurrentRoom.Name);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("MultiplayerLauncher:OnCreateRoomFailed() create room failed. Probably, it exists already. Try to create new room with different number.");

        switch (multiplayerMode)
        {
            case MultiplayerMode.CreateRoom:
                CreateRoom(GetRoomOptions());
                break;
            case MultiplayerMode.SinglePlayer:
                RoomOptions roomOptions = GetRoomOptions();
                roomOptions.MaxPlayers = 1;
                PhotonNetwork.JoinOrCreateRoom("SP_" + Random.Range(0, 10000).ToString(), roomOptions, TypedLobby.Default);
                break;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning("MultiplayerLauncher:OnJoinRoomFailed() join room failed.");

        switch (multiplayerMode)
        {
            case MultiplayerMode.JoinRoom:
                Debug.LogError("We could not join into the room");
                break;
            case MultiplayerMode.QuickMatch:
                Debug.Log("MultiplayerLauncher:OnJoinRoomFailed() try to join next room:" + curQuickMatches);
                curQuickMatches += 1; // try next room
                PhotonNetwork.JoinOrCreateRoom("QM_" + curQuickMatches.ToString(), GetRoomOptions(), TypedLobby.Default);
                break;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");
        Debug.LogError("MultiplayerLauncher:OnJoinRandomFailed(): We don't expect this function to be called.");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        Debug.Log("PhotonNetwork.IsMasterClient=" + PhotonNetwork.IsMasterClient);

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers
            && PhotonNetwork.CurrentRoom.MaxPlayers == 1) // only in SinglePlayer mode -> Otherwise use OnPlayerEnteredRoom
        {
            Debug.Log("We load the 'Arena' Scene ");
            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("Arena");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnPlayerEnteredRoom() called by PUN. Now this client is in a room.");
        Debug.Log("PhotonNetwork.IsMasterClient=" + PhotonNetwork.IsMasterClient);

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("We load the 'Arena' Scene ");
            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("Arena");
        }
    }


    #endregion


    #region Public Methods


    public void ButtonCreateRoom()
    {
        Debug.Log("MultiplayerLauncher:ButtonCreateRoom() called");
        multiplayerMode = MultiplayerMode.CreateRoom;
        Connect();
    }

    public void ButtonJoinRoom()
    {
        Debug.Log("MultiplayerLauncher:ButtonJoinRoom() called");
        multiplayerMode = MultiplayerMode.JoinRoom;
        Connect();
    }
    public void ButtonQuickMatch()
    {
        Debug.Log("MultiplayerLauncher:ButtonQuickMatch() called");
        multiplayerMode = MultiplayerMode.QuickMatch;
        Connect();
    }
    public void ButtonSinglePlayer()
    {
        Debug.Log("MultiplayerLauncher:ButtonSinglePlayer() called");
        multiplayerMode = MultiplayerMode.SinglePlayer;
        Connect();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    private void Connect()
    {
        Debug.Log("MultiplayerLauncher: Connect() was called with: " + PhotonNetwork.IsConnected);
        progressLabel.SetActive(true);
        //controlPanel.SetActive(false);
        SetButtonsInteractable(false);

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (!PhotonNetwork.IsConnected)
        {
            // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    private RoomOptions GetRoomOptions()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        return roomOptions;
    }
    private void CreateRoom(RoomOptions roomOptions)
    {
        // create room
        string roomNumber = Random.Range(0, 10000).ToString();
        PhotonNetwork.CreateRoom(roomNumber, roomOptions, TypedLobby.Default);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        buttonCreateRoom.interactable = interactable;
        buttonJoinRoom.interactable = interactable;
        buttonQuickMatch.interactable = interactable;
        buttonSinglePlayer.interactable = interactable;
    }
    #endregion

}
