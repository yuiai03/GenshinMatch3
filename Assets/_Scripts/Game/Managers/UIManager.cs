using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public MainPanel MainPanel { get; private set; }
    public MapPanel MapPanel { get; private set; }
    public CharacterPanel CharacterPanel { get; private set; }
    public PvPPanel PvpPanel { get; private set; }
    public SceneTransiton SceneTransiton { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        if(!MapPanel) MapPanel = GetComponentInChildren<MapPanel>();
        if (!PvpPanel) PvpPanel = GetComponentInChildren<PvPPanel>();
        if (!MainPanel) MainPanel = GetComponentInChildren<MainPanel>();
        if (!CharacterPanel) CharacterPanel = GetComponentInChildren<CharacterPanel>();
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
        MapPanel.MainBgState(sceneType == SceneType.MainMenu);
        MainPanel.MainBgState(sceneType == SceneType.MainMenu);
        PvpPanel.MainBgState(sceneType == SceneType.MainMenu);
        CharacterPanel.MainBgState(false);
    }

    public void ShowPanel(MainButtonType  mainButtonType)
    {
        MapPanel.MainBgState(mainButtonType ==  MainButtonType.Map);
        CharacterPanel.MainBgState(mainButtonType == MainButtonType.Character);
        PvpPanel.MainBgState(mainButtonType == MainButtonType.PvP);
    }
}
