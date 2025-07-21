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
                _moveTween = transform.DOLocalMove(Vector2.zero, Config.TileMoveDuration)
                    .OnComplete(() =>
                    {
                        var selectedTile = !GameManager.Instance.IsSingleScene()
                            ? MultiplayerInputManager.Instance.SelectedTile
                            : SinglePlayerInputManager.Instance.SelectedTile;

                        var targetTile = !GameManager.Instance.IsSingleScene()
                            ? MultiplayerInputManager.Instance.TargetTile
                            : SinglePlayerInputManager.Instance.TargetTile;

                        if (this == selectedTile)
                        {
                            EventManager.EndSwapTile(selectedTile, targetTile);
                            if (GameManager.Instance.IsSingleScene()) return;

                            MultiplayerInputManager.Instance.photonView.RPC(
                                "TileEndSwap", RpcTarget.Others,
                                (Vector2)selectedTile.Empty.IntPos,
                                (Vector2)targetTile.Empty.IntPos);
                        }
                    });
            }
        }
    }

    public TileType TileType { get; set; }
    private Tween _moveTween;
    private SpriteRenderer spriteRenderer;

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