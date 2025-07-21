using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviourPunCallbacks
{
    private int energy = 0;
    [SerializeField] private TileType _tileType;
    [SerializeField] private Image _borderImage, _bgImage, _iconImage, _skillImage;
    [SerializeField] private Button _skillButton;

    private Tween _fillTween;

    private void Awake()
    {
        _skillButton.onClick.AddListener(SkillClick);
    }

    private void Start()
    {
        if (_tileType == TileType.None) return;
        if (!_borderImage || !_iconImage || !_skillImage) return;

        var tileConfig = Helper.GetTileConfig(_tileType);
        _borderImage.color = tileConfig.color;
        _iconImage.sprite = LoadManager.SpriteLoad($"Tile/{_tileType}");
        _bgImage.sprite = LoadManager.SpriteLoad($"Skill/{_tileType}Skill 1");
        _skillImage.sprite = LoadManager.SpriteLoad($"Skill/{_tileType}Skill");

        ReSetup();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EventManager.OnTileMatch += SetFillAmount;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        EventManager.OnTileMatch -= SetFillAmount;
        _fillTween?.Kill();
    }

    private void ReSetup()
    {
        energy = 0;
        SetFillAmount(new MatchData(_tileType, energy));
    }

    private void SkillClick()
    {
        SkillAction();
    }

    [PunRPC]
    public void SkillAction()
    {
        if (!CanActive()) return;

        if (PhotonNetwork.IsMasterClient)
        {
            if (MultiplayerLevelManager.Instance.IsPlayer1Turn() || MultiplayerLevelManager.Instance.IsPlayer1EndTurn())
            {
                photonView.RPC("SpawnSkill", RpcTarget.AllViaServer);
            }
        }
        else
        {
            if (MultiplayerLevelManager.Instance.IsPlayer2Turn() || MultiplayerLevelManager.Instance.IsPlayer2EndTurn())
            {
                photonView.RPC("SpawnSkill", RpcTarget.AllViaServer);
            }
        }

    }
    [PunRPC]
    public void SpawnSkill()
    {
        ReSetup();
        var bullet = PoolManager.Instance.GetObject<SkillBullet>(PoolType.SkillBullet, transform.position, null);
        var matchData = new MatchData(_tileType, 5);
        bullet.Initialize(matchData, true, 30);
    }

    private void SetFillAmount(MatchData matchData)
    {
        if(matchData.TileType != _tileType) return;
        if (PhotonNetwork.IsMasterClient)
        {
            if (MultiplayerLevelManager.Instance.IsPlayer1Turn() || MultiplayerLevelManager.Instance.IsPlayer1EndTurn())
            {
                energy += matchData.Count;
                float targetFillAmount = energy / 10f;

                _fillTween?.Kill();
                _fillTween = _skillImage.DOFillAmount(targetFillAmount, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (CanActive()) _skillImage.fillAmount = 1f;
                    });
            }
        }
        else
        {
            if (MultiplayerLevelManager.Instance.IsPlayer2Turn() || MultiplayerLevelManager.Instance.IsPlayer2EndTurn())
            {
                energy += matchData.Count;
                float targetFillAmount = energy / 10f;

                _fillTween?.Kill();
                _fillTween = _skillImage.DOFillAmount(targetFillAmount, 0.3f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        if (CanActive()) _skillImage.fillAmount = 1f;
                    });
            }
        }

    }

    private bool CanActive()
    {
        return energy >= 10;
    }
}
