using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Skill : MonoBehaviour
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
    private void OnEnable()
    {
        EventManager.OnTileMatch += SetFillAmount;
    }
    private void OnDisable()
    {
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
        if (!CanActive()) return;

        ReSetup();
        var bullet = PoolManager.Instance.GetObject<PlayerBullet>(PoolType.PlayerBullet, transform.position, null);
        var matchData = new MatchData(_tileType, 5);
        bullet.Initialize(matchData, true, 30);
    }

    private void SetFillAmount(MatchData matchData)
    {
        if(matchData.TileType != _tileType) return;

        energy += matchData.Count;
        float targetFillAmount = energy / 10f;

        _fillTween?.Kill();
        _fillTween = _skillImage.DOFillAmount(targetFillAmount, 0.3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                if(CanActive()) _skillImage.fillAmount = 1f;
            });
    }

    private bool CanActive()
    {
        return energy >= 10;
    }
}
