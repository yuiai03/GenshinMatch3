using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Empty _empty;
    public Empty Empty
    {
        get => _empty;
        set
        {
            _empty = value;
            if (_empty)
            {
                _empty.Tile = this;
                transform.SetParent(_empty.transform); //Set parent sau khi nhận Empty
                if (_moveTween != null && _moveTween.IsActive()) _moveTween.Kill();
                _moveTween = transform.DOLocalMove(Vector2.zero, Config.TileMoveDuration).OnComplete(() =>
                {
                    var selectedTile = GameInputManager.Instance.SelectedTile;
                    var targetTile = GameInputManager.Instance.TargetTile;
                    if (this == selectedTile) EventManager.EndSwapTile(selectedTile, targetTile);
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
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = LoadManager.SpriteLoad($"Tile/{type}");
    }

    private void OnDisable()
    {
        _moveTween?.Kill();
    }
}
