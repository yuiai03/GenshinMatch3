using UnityEngine;

public class Map : Singleton<Map>
{
    private GameObject _teleportHolder;

    protected override void Awake()
    {
        base.Awake();
        if (!_teleportHolder) _teleportHolder = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        TeleportSetUp();
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

}
