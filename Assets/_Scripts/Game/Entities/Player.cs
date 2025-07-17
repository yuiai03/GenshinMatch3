using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Player : Entity, IPunObservable
{
    public int PlayerId => photonView.ViewID;
    private Coroutine attackCoroutine;

    protected override void CurrentTileTypeChanged()
    {
        EventManager.CurrentTileTypeChanged(CurrentTileType, GameManager.Instance.CurrentSceneType == SceneType.SinglePlayer ? true : PlayerId == 1001);
    }
    public override void GetData(EntityData entityData)
    {
        base.GetData(entityData);
        EventManager.MaxHPChanged(entityData.entityConfig.MaxHP, GameManager.Instance.CurrentSceneType == SceneType.SinglePlayer ? true : PlayerId == 1001);
    }

    protected override void HPChanged(float hp)
    {
        EventManager.HPChanged(hp, GameManager.Instance.CurrentSceneType == SceneType.SinglePlayer ? true : PlayerId == 1001);
    }

    public override void Attack(Entity target)
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(target));
    }
    
    private IEnumerator AttackCoroutine(Entity target)
    {
        var matchsHistory = SinglePlayerBoardManager.Instance.GetMatchHistory();
        foreach (var matchData in matchsHistory)
        {
            if(Helper.GetCharacterElemental(GameManager.Instance.CurrentPlayerType) == matchData.TileType)
            {
                matchData.Count++;
            }
            _entityAnim.Attack();
            
            yield return new WaitForSeconds(0.3f);
            var bullet = PoolManager.Instance.GetObject<PlayerBullet>(PoolType.PlayerBullet, shootPoint.position, transform);
            bullet.Initialize(matchData);
            yield return new WaitForSeconds(0.5f); 
        }
        yield return new WaitForSeconds(1f);
        EventManager.GameStateChanged(GameState.EnemyTurn);
    }
    
    /// <summary>
    /// RPC method để initialize player data trên tất cả clients
    /// </summary>
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
            HP = (float)stream.ReceiveNext();
            CurrentTileType = (TileType)stream.ReceiveNext();
            IsFreeze = (bool)stream.ReceiveNext();
            IsShield = (bool)stream.ReceiveNext();
        }
    }
}
