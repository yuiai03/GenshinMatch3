using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public ActionPanel ActionPanel;
    public GamePanel GamePanel;
    public SceneTransiton LevelTransiton;
    protected override void Awake()
    {
        base.Awake();
    }


}
