using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MapPanel : PanelBase
{
    public bool IsActive => bg.gameObject.activeSelf;
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private Image _enemyImage;

    protected override void Awake()
    {
        base.Awake();
        if (_actionButton)
        {
            _actionButton.onClick.AddListener(OnActionButtonClicked);
        }
    }

    private void OnEnable()
    {
        EventManager.OnOpenLevelPanel += OnOpenLevelPanel;
    }
    private void OnDisable()
    {
        EventManager.OnOpenLevelPanel -= OnOpenLevelPanel;
    }
    private void OnOpenLevelPanel(LevelData levelData, EntityType entityType)
    {
        ShowPanel();
        SetInfo(levelData.levelConfig.turnsNumber, entityType);
    }
    private void OnActionButtonClicked()
    {
        HidePanel();
        LoadManager.Instance.TransitionLevel(SceneType.Game);
    }

    public void SetInfo(int turn, EntityType entityType)
    {
        SetTurnText(turn);
        SetEnemyImage(entityType);
    }
    private void SetTurnText(int turn) => _turnText.text = $"{turn} Lượt";
    private void SetEnemyImage(EntityType entityType)
    {
        var sprite = LoadManager.SpriteLoad($"Entities/{entityType}");
        if(sprite) _enemyImage.sprite = sprite;
    }
}
