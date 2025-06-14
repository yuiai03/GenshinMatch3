using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private BoardManager _boardManager;
    [SerializeField] private GameObject _entitiesHolder;

    protected override void Awake()
    {
        base.Awake();

        _boardManager = GetComponentInChildren<BoardManager>();
    }

}
