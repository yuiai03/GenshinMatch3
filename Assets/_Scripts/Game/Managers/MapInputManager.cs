using UnityEngine;

public class MapInputManager : Singleton<MapInputManager>
{
    [SerializeField] private bool _isTeleportTouch;
    private void Update()
    {
        HandleMapInput();
    }
    private void HandleMapInput()
    {
        if (UIManager.Instance.MapPanel.IsActive) return;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
            if (!hit.collider)
            {
                _isTeleportTouch = false; 
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeganPhase();
                    break;
                case TouchPhase.Ended:
                    EndedPhase(hit);
                    break;
            }
        }
    }
    private void BeganPhase()
    {
        _isTeleportTouch = true;
    }

    private void EndedPhase(RaycastHit2D hit)
    {
        if (!_isTeleportTouch) return;

        _isTeleportTouch = false;
        Teleport teleport = hit.collider.GetComponent<Teleport>();
        if (teleport) teleport.OpenLevelPanel();
    }
}
