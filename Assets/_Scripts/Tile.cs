using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    private Empty _empty;
    public Empty Empty
    {
        get => _empty;
        set
        {
            _empty = value;
            if (_empty != null)
            {
                _empty.Tile = this;
                transform.SetParent(_empty.transform);
                if (_moveTween != null && _moveTween.IsActive()) _moveTween.Kill();
                _moveTween = transform.DOLocalMove(Vector2.zero, Config.TileMoveDuration).OnComplete(() =>
                {
                    var selectedTile = GameManager.Instance.CurrentSceneType == SceneType.Multiplayer
                        ? MultiplayerInputManager.Instance.SelectedTile
                        : SinglePlayerInputManager.Instance.SelectedTile;

                    var targetTile = GameManager.Instance.CurrentSceneType == SceneType.Multiplayer
                        ? MultiplayerInputManager.Instance.TargetTile
                        : SinglePlayerInputManager.Instance.TargetTile;

                    if (this == selectedTile) 
                    {
                        EventManager.EndSwapTile(selectedTile, targetTile);
                    }
                });
            }
        }
    }

    public TileType TileType { get; set; }
    private Tween _moveTween;
    private SpriteRenderer spriteRenderer;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing on Tile.");
        }
    }

    public void InitialData(TileType type, Empty empty)
    {
        TileType = type;
        Empty = empty; 
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = LoadManager.SpriteLoad($"Tile/{type}");
    }

    private void OnDisable()
    {
        _moveTween?.Kill();
    }

}