using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public ActionPanel ActionPanel { get; private set; }
    public GamePanel GamePanel { get; private set; }
    public SceneTransiton SceneTransiton { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if(!ActionPanel) ActionPanel = GetComponentInChildren<ActionPanel>();
        if (!GamePanel) GamePanel = GetComponentInChildren<GamePanel>();
        if (!SceneTransiton) SceneTransiton = GetComponentInChildren<SceneTransiton>();
    }

    private void OnEnable()
    {
        EventManager.OnSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        EventManager.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(SceneType sceneType)
    {
        GamePanel.Menu.SetActive(sceneType == SceneType.Game);
    }
}
