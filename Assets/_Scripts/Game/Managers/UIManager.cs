using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public GamePanel GamePanel { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        GamePanel = GetComponentInChildren<GamePanel>();
    }
}
