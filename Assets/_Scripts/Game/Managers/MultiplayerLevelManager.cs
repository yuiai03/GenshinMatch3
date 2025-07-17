using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class MultiplayerLevelManager : Singleton<MultiplayerLevelManager>
{
    [SerializeField] protected GameObject _entitiesHolder;

    [Header("Spawn Points")]
    [SerializeField] private Transform _player1SpawnPoint;
    [SerializeField] private Transform _player2SpawnPoint;

    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }

    public MultiplayerBoardManager MultiplayerBoardManager { get; private set; }
    protected override void Awake()
    {
        base.Awake();
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
                EntityType playerType = (EntityType)EntityType.Olek;
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
        playerObj.transform.SetParent(_entitiesHolder.transform);
        playerObj.transform.localScale = new Vector3(isPlayer1 ? -1 : 1, 1, 1);

        PhotonView pv = playerObj.GetComponent<PhotonView>();
        pv.RPC("InitializePlayerData", RpcTarget.All, dataPath);

        if (isPlayer1) Player1 = playerObj;
        else Player2 = playerObj;
    }

    public void Player1Action()
    {
        Player1.Attack(Player2);
    }

    public void Player2Action()
    {
        Player2.Attack(Player1);
    }
}
