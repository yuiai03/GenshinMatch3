using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class PvPPanel : MonoBehaviourPunCallbacks
{
    public GameObject mainBg;
    public TextMeshProUGUI connectionStatusText;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    
    public Button createRoomButton, joinRoomButton, startGameButton;
    public TMP_InputField roomNameInput;
    
    private void Awake()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }
    
    private void Start()
    {
        ConnectToPhoton();
    }

    // Added to replace PanelBase functionality and fix UIManager errors
    public void MainBgState(bool state)
    {
        mainBg.SetActive(state);
    }
    
    private void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected)
        {
            UpdateUI();
            return;
        }
        
        connectionStatusText.text = "Connecting to Photon...";
        
        // Disable buttons while connecting
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
        startGameButton.interactable = false;
        
        PhotonNetwork.ConnectUsingSettings();
    }
    
    private void UpdateUI()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                connectionStatusText.text = "Connected - In Room";
                roomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
                playerCountText.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
                
                // Enable start game button only for master client when room is full
                startGameButton.interactable = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2;
                
                createRoomButton.interactable = false;
                joinRoomButton.interactable = false;
            }
            else
            {
                connectionStatusText.text = "Connected - In Lobby";
                roomNameText.text = "Room: None";
                playerCountText.text = "Players: 0/2";
                
                createRoomButton.interactable = true;
                joinRoomButton.interactable = true;
                startGameButton.interactable = false;
            }
        }
        else
        {
            connectionStatusText.text = "Disconnected";
            roomNameText.text = "Room: None";
            playerCountText.text = "Players: 0/2";
            
            createRoomButton.interactable = false;
            joinRoomButton.interactable = false;
            startGameButton.interactable = false;
        }
    }
    
    private void OnCreateRoomClicked()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        string roomName = string.IsNullOrEmpty(roomNameInput.text) ? "Room_" + Random.Range(1000, 9999) : roomNameInput.text;
        
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2,
            IsOpen = true,
            IsVisible = true
        };
        
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        
        connectionStatusText.text = "Creating room...";
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    
    private void OnJoinRoomClicked()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            PhotonNetwork.JoinRoom(roomNameInput.text);
            connectionStatusText.text = "Joining room...";
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
            connectionStatusText.text = "Joining random room...";
        }
        
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }
    
    private void OnStartGameClicked()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        Debug.Log("Starting PvP Game");
        
        // Store player character types in room custom properties before loading scene
        Hashtable roomProps = new Hashtable();
        
        // Store each player's selected character type
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            // Get player's selected character type
            EntityType playerType = GetPlayerSelectedCharacter(players[i]);
            roomProps[$"Player{i}CharacterType"] = (int)playerType;
            roomProps[$"Player{i}ActorNumber"] = players[i].ActorNumber;
        }
        
        // Store player count and setup game state
        roomProps["GameState"] = (int)GameState.PvPGameStart;
        roomProps["PlayerCount"] = PhotonNetwork.CurrentRoom.PlayerCount;
        
        // Update room properties
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        
        // Close room to prevent new players from joining
        PhotonNetwork.CurrentRoom.IsOpen = false;
        
        // Use RPC to notify all players to transition to PvP game scene using LoadManager
        photonView.RPC("StartPvPGame", RpcTarget.All);
    }
    
    [PunRPC]
    private void StartPvPGame()
    {
        Debug.Log("Received StartPvPGame RPC - Transitioning to PvP Game");
        
        // Use LoadManager to transition to PvP game scene
        LoadManager.Instance.TransitionLevel(SceneType.PvPGame);
    }
    
    // Helper method to get player's selected character type
    private EntityType GetPlayerSelectedCharacter(Photon.Realtime.Player player)
    {
        // Try to get from player's custom properties first
        if (player.CustomProperties.TryGetValue("CharacterType", out object characterType))
        {
            return (EntityType)characterType;
        }
        
        // If not found in custom properties, check if it's the local player
        if (player.IsLocal)
        {
            return GameManager.Instance.PlayerType;
        }
        
        // Default fallback character
        return EntityType.Buba;
    }
    
    // Override OnJoinedRoom to set player's character type in custom properties
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        
        // Set local player's character type in custom properties
        Hashtable playerProps = new Hashtable();
        playerProps["CharacterType"] = (int)GameManager.Instance.PlayerType;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
        
        UpdateUI();
    }
    
    // --- Overriding other Photon Callbacks ---

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Photon: " + cause);
        UpdateUI();
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        UpdateUI();
    }
    
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully");
        UpdateUI();
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room: " + message);
        connectionStatusText.text = "Failed to create room";
        UpdateUI();
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room: " + message);
        connectionStatusText.text = "Failed to join room";
        UpdateUI();
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join random room: " + message);
        connectionStatusText.text = "No available rooms";
        UpdateUI();
    }
    
    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
        UpdateUI();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player entered room: " + newPlayer.NickName);
        UpdateUI(); // This ensures the UI updates when a player joins
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left room: " + otherPlayer.NickName);
        UpdateUI(); // This ensures the UI updates when a player leaves
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.Log("Master client switched to: " + newMasterClient.NickName);
        UpdateUI(); // Update UI to reflect new master client
    }
}

