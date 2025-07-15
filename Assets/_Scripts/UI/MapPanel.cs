using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class MapPanel : PanelBase
{
    [SerializeField] private Button _actionButton;
    [SerializeField] private TextMeshProUGUI _turnText;
    [SerializeField] private Image _enemyImage;
    [SerializeField] private GameObject _teleportHolder;
    [SerializeField] protected CanvasGroup infoBg, infoMenu;
    protected override void Awake()
    {
        base.Awake();
        if (!_teleportHolder) _teleportHolder = transform.GetChild(0).gameObject;
        if (infoBg) infoBg.gameObject.SetActive(false);
        if (infoMenu) infoMenu.gameObject.SetActive(false);

        _actionButton.onClick.AddListener(OnActionButtonClicked);
        infoBg.GetComponent<Button>().onClick.AddListener(() => HideInfoPanel());
    }
    private void Start()
    {
        TeleportSetUp();
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
        ShowInfoPanel();
        SetInfo(levelData.levelConfig.turnsNumber, entityType);
    }
    private void OnActionButtonClicked()
    {
        HideInfoPanel();
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
        if (sprite)
        {
            _enemyImage.sprite = sprite;
            _enemyImage.SetNativeSize();
        }
    }

    public void TeleportSetUp()
    {
        for (int i = 0; i < _teleportHolder.transform.childCount; i++)
        {
            var child = _teleportHolder.transform.GetChild(i);
            if (child.TryGetComponent<Teleport>(out var teleport))
            {
                teleport.InitializeData();
            }
        }
    }

    public virtual void ShowInfoPanel(float time = 0.5f)
    {
        infoMenu.gameObject.SetActive(true);
        infoBg.gameObject.SetActive(true);
        infoBg.alpha = infoMenu.alpha = 0;

        var sequence = DOTween.Sequence();
        sequence.Append(infoBg.DOFade(1, time).SetUpdate(true));
        sequence.Join(infoMenu.DOFade(1, time).SetUpdate(true));
    }
    public virtual void HideInfoPanel(float time = 0.5f)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(infoBg.DOFade(0, time).SetUpdate(true));
        sequence.Join(infoMenu.DOFade(0, time).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            infoMenu.gameObject.SetActive(false);
            infoBg.gameObject.SetActive(false);
        });
    }
}
