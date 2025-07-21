using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PvPPanel : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject mainBg;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;
    
    [Header("Buttons")]
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button joinRandomButton;
    public Button startGameButton;
    public Button leaveRoomButton;
    
    [Header("Input")]
    public TMP_InputField roomNameInput;
    
    private NetworkManager networkManager;
    
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        SetupButtons();
    }

    private void SetupButtons()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        joinRandomButton.onClick.AddListener(OnJoinRandomClicked);
        startGameButton.onClick.AddListener(OnStartGameClicked);
        leaveRoomButton.onClick.AddListener(OnLeaveRoomClicked);
    }

    public void MainBgState(bool state)
    {
        mainBg.SetActive(state);
    }

    private void OnCreateRoomClicked()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        string roomName = roomNameInput.text;
        networkManager.CreateRoom(roomName);
        
        SetButtonsInteractable(false);
        statusText.text = "Creating room...";
    }

    private void OnJoinRoomClicked()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
        {
            statusText.text = "Please enter room name";
            return;
        }
        
        networkManager.JoinRoom(roomName);
        
        SetButtonsInteractable(false);
        statusText.text = "Joining room...";
    }

    private void OnJoinRandomClicked()
    {
        if (!PhotonNetwork.IsConnected) return;
        
        networkManager.JoinRandomRoom();
        
        SetButtonsInteractable(false);
        statusText.text = "Finding random room...";
    }

    private void OnStartGameClicked()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;
        
        Hashtable roomProps = new Hashtable();
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        
        for (int i = 0; i < players.Length; i++)
        {
            EntityType entityType = GetPlayerEntityType(players[i]);
            roomProps[$"Player{i}EntityType"] = (int)entityType;
            Debug.LogError(entityType);
        }
        
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        
        // Start game for all players
        photonView.RPC("StartMultiplayerGame", RpcTarget.AllViaServer);
    }

    private void OnLeaveRoomClicked()
    {
        networkManager.LeaveRoom();
    }

    [PunRPC]
    public void StartMultiplayerGame()
    {
        LoadManager.Instance.TransitionLevel(SceneType.Multiplayer);
    }

    public void OnRoomActionFailed(string message)
    {
        statusText.text = message;
        SetButtonsInteractable(true);
    }

    private EntityType GetPlayerEntityType(Photon.Realtime.Player player)
    {
        if (player.CustomProperties.TryGetValue("EntityType", out object entityType))
        {
            return (EntityType)entityType;
        }
        
        return player.IsLocal ? GameManager.Instance.CurrentPlayerType : EntityType.Buba;
    }

    private void SetButtonsInteractable(bool interactable)
    {
        createRoomButton.interactable = interactable;
        joinRoomButton.interactable = interactable;
        joinRandomButton.interactable = interactable;
    }

    public void UpdateUI()
    {
        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Not Connected";
            roomNameText.text = "Room: None";
            playerCountText.text = "Players: 0/2";
            SetButtonsInteractable(false);
            startGameButton.interactable = false;
            leaveRoomButton.interactable = false;
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            // In room
            statusText.text = "In Room";
            roomNameText.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
            playerCountText.text = $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/2";
            
            SetButtonsInteractable(false);
            startGameButton.interactable = PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2;
            leaveRoomButton.interactable = true;
        }
        else
        {
            // In lobby
            statusText.text = "In Lobby";
            roomNameText.text = "Room: None";
            playerCountText.text = "Players: 0/2";
            
            SetButtonsInteractable(true);
            startGameButton.interactable = false;
            leaveRoomButton.interactable = false;
        }
    }
}

