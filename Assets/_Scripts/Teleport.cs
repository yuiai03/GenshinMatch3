using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    [SerializeField] private LevelType _levelType;
    [SerializeField] private Button _levelButton;
    [SerializeField] private TextMeshProUGUI _levelText;
    private LevelData _levelData;

    private void Awake()
    {
        _levelButton.onClick.AddListener(OpenLevelPanel);
    }
    public void OpenLevelPanel()
    {
        if (!_levelData) return;
        EventManager.OpenLevelPanel(_levelData, _levelData.levelConfig.enemyType);
    }

    public void InitializeData()
    {
        _levelData = LoadManager.DataLoad<LevelData>($"Levels/{_levelType}");
        if (!_levelData) return;

        SetLevelText(_levelData.levelConfig.levelName);
    }

    private void SetLevelText(string level) => _levelText.text = level;
}
