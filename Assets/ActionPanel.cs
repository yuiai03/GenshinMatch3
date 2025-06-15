using UnityEngine;
using UnityEngine.UI;

public class ActionPanel : PanelBase
{
    [SerializeField] private Button actionButton;

    protected override void Awake()
    {
        base.Awake();
        if (actionButton)
        {
            actionButton.onClick.AddListener(OnActionButtonClicked);
        }
    }

    private void OnActionButtonClicked()
    {
        HidePanel();
        LoadManager.Instance.TransitionLevel(SceneType.Game);
    }
}
