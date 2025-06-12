using DG.Tweening;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType TileType { get; private set; }

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
                transform.SetParent(_empty.transform); //Set lại parent sau khi đổi Empty
                transform.DOLocalMove(Vector2.zero, 0.5f).OnComplete(() => //Di chuyển tile về vị trí của Empty
                {
                    EventManager.EndSwapTileAction();
                });
            }
        }
    }
    private SpriteRenderer spriteRenderer;
    public void InitialData(TileType type, Empty empty)
    {
        TileType = type;
        Empty = empty;
        spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = LoadManager.SpriteLoad($"Tile/{type}");
    }
}
