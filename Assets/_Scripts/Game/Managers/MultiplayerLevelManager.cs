using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] protected GameObject _entitiesHolder;

    [Header("Spawn Points")]
    [SerializeField] private Transform _player1SpawnPoint;
    [SerializeField] private Transform _player2SpawnPoint;

    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    public MultiplayerBoardManager MultiplayerBoardManager { get; private set; }
    public static MultiplayerLevelManager Instance { get; private set; }
    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        MultiplayerBoardManager = GetComponentInChildren<MultiplayerBoardManager>();
    }
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeData();
        }
    }

    public void InitializeData()
    {
        MultiplayerBoardManager.InitializeBoard();
        InitializeEntities();
    }

    public void InitializeEntities()
    {
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        for(var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (roomProps.TryGetValue($"Player{i}EntityType", out object entityTypeObj))
            {
                EntityType playerType = (EntityType)entityTypeObj;
                SpawnPlayer(PhotonNetwork.PlayerList[i], playerType);
            }
        }
    }
    private void SpawnPlayer(Photon.Realtime.Player player, EntityType type)
    {
        var dataPath = $"Entities/Player/{type}";
        var playerPath = $"Prefabs/Entities/Player";

        bool isPlayer1 = player == PhotonNetwork.PlayerList[0];
        Vector2 spawnPos = isPlayer1 ? _player1SpawnPoint.position : _player2SpawnPoint.position;

        GameObject entity = PhotonNetwork.Instantiate(playerPath, spawnPos, Quaternion.identity);
        var playerObj = entity.GetComponent<Player>();

        PhotonView pv = playerObj.GetComponent<PhotonView>();

        playerObj.transform.SetParent(_entitiesHolder.transform);
        playerObj.transform.localScale = new Vector3(isPlayer1 ? -1 : 1, 1, 1);

        photonView.RPC("AssignPlayerReference", RpcTarget.AllViaServer, pv.ViewID, isPlayer1, dataPath);
    }

    [PunRPC]
    public void AssignPlayerReference(int playerViewID, bool isPlayer1, string dataPath)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        var playerObj = playerPV.GetComponent<Player>();

        if (isPlayer1)
        {
            Player1 = playerObj;
        }
        else
        {
            Player2 = playerObj;
        }

        playerObj.GetComponent<PhotonView>().RPC("InitializePlayerData", RpcTarget.AllViaServer, dataPath);
    }

    [PunRPC]
    public void Player1Action()
    {
        Player1.Attack(Player2);
    }

    [PunRPC]
    public void Player2Action()
    {
        Player2.Attack(Player1);
    }

    public Player CurrentPlayer()
    {
        if (PhotonNetwork.IsMasterClient && MultiplayerGameManager.Instance.GameState == GameState.Player1EndTurn) return Player1;
        else return Player2;
    }
    public bool IsPlayer1Turn()
    {
        return MultiplayerGameManager.Instance.GameState == GameState.Player1Turn;
    }
    public bool IsPlayer1EndTurn()
    {
        return MultiplayerGameManager.Instance.GameState == GameState.Player1EndTurn;
    }
    public bool IsPlayer2Turn()
    {
        return MultiplayerGameManager.Instance.GameState == GameState.Player2Turn;
    }
    public bool IsPlayer2EndTurn()
    {
        return MultiplayerGameManager.Instance.GameState == GameState.Player2EndTurn;
    }

}
