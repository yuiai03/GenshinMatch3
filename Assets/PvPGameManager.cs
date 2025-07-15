using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
public class PvPGameManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayers();
        }
    }
    
    private void SpawnPlayers()
    {
        // Lấy thông tin character types từ room properties
        Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
        
        // Spawn player cho mỗi người chơi
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (roomProps.TryGetValue($"Player{i}CharacterType", out object characterType))
            {
                EntityType playerType = (EntityType)characterType;
                Vector3 spawnPosition = GetSpawnPosition(i);
                
                // Spawn player prefab với EntityType tương ứng
                PhotonNetwork.Instantiate($"Prefabs/Entities/Player", spawnPosition, Quaternion.identity);
            }
        }
    }
    
    private Vector3 GetSpawnPosition(int playerIndex)
    {
        // Trả về vị trí spawn cho player (player 0 ở bên trái, player 1 ở bên phải)
        return playerIndex == 0 ? new Vector3(-2, 0, 0) : new Vector3(2, 0, 0);
    }
}