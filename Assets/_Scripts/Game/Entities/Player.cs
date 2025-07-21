using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Player : Entity, IPunObservable
{
    private Coroutine attackCoroutine;

    protected override void CurrentTileTypeChanged()
    {
        if(GameManager.Instance.IsSingleScene()) EventManager.CurrentTileTypeChanged(CurrentTileType, true);
        else EventManager.CurrentTileTypeChanged(CurrentTileType, this == MultiplayerLevelManager.Instance.Player1);
    }
    
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        if (GameManager.Instance.IsSingleScene()) EventManager.MaxHPChanged(entityData.entityConfig.MaxHP, true);
        else EventManager.MaxHPChanged(entityData.entityConfig.MaxHP, this == MultiplayerLevelManager.Instance.Player1);
    }

    protected override void HPChanged(float hp)
    {
        if (GameManager.Instance.IsSingleScene()) EventManager.HPChanged(hp, true);
        else EventManager.HPChanged(hp, this == MultiplayerLevelManager.Instance.Player1);
    }

    public override void Attack(Entity target)
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(target));
    }
    
    private System.Collections.IEnumerator AttackCoroutine(Entity target)
    {
        var matchsHistory = GameManager.Instance.IsSingleScene() 
            ? SinglePlayerBoardManager.Instance.GetMatchHistory() 
            : MultiplayerBoardManager.Instance.GetMatchHistory();
        foreach (var matchData in matchsHistory)
        {
            var data = matchData;

            if(GameManager.Instance.IsSingleScene())
            {
                if (Helper.GetCharacterElemental(GameManager.Instance.CurrentPlayerType) == data.TileType)
                {
                    data.Count++;
                }
            }
            else
            {
                Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
                if (MultiplayerGameManager.Instance.IsPlayer1EndTurn())
                {
                    if (roomProps.TryGetValue($"Player{0}EntityType", out object entityTypeObj))
                    {
                        if (Helper.GetCharacterElemental((EntityType)entityTypeObj) == data.TileType)
                        {
                            data.Count++;
                        }
                    }
                }
                else if (MultiplayerGameManager.Instance.IsPlayer2EndTurn())
                {
                    if (roomProps.TryGetValue($"Player{1}EntityType", out object entityTypeObj))
                    {
                        if (Helper.GetCharacterElemental((EntityType)entityTypeObj) == data.TileType)
                        {
                            data.Count++;
                        }
                    }
                }
            }
            _entityAnim.Attack();
            yield return new WaitForSeconds(0.5f);
            var bullet = PoolManager.Instance.GetObject<PlayerBullet>(
                PoolType.PlayerBullet, shootPoint.position, transform);
            bullet.Initialize(data);
            yield return new WaitForSeconds(0.5f); 
        }
        yield return new WaitForSeconds(1f);
        HandleAfterAttack();
    }

    private void HandleAfterAttack()
    {
        if (GameManager.Instance.IsSingleScene())
        {
            EventManager.GameStateChanged(GameState.EnemyTurn);
        }
        else
        {
            if (MultiplayerGameManager.Instance.IsPlayer1EndTurn())
            {
                EventManager.GameStateChanged(GameState.Player2Turn);
            }
            else if (MultiplayerGameManager.Instance.IsPlayer2EndTurn())
            {
                EventManager.GameStateChanged(GameState.EndRound);
            }
        }
    }
    
    [PunRPC]
    public void InitializePlayerData(string path)
    {
        var playerData = LoadManager.DataLoad<EntityData>(path);
        if (playerData) GetData(playerData);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HP);
            stream.SendNext(CurrentTileType);
            stream.SendNext(IsFreeze);
            stream.SendNext(IsShield);
        }
        else
        {
            // Nhận dữ liệu từ remote client
            HP = (float)stream.ReceiveNext();
            CurrentTileType = (TileType)stream.ReceiveNext();
            IsFreeze = (bool)stream.ReceiveNext();
            IsShield = (bool)stream.ReceiveNext();
        }
    }
}
