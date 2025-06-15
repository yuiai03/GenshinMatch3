using UnityEngine;

public class GamePanel : MonoBehaviour
{
    public GameObject Menu;
    public GameObject MatchedTilesViewHolder;

    private void Awake()
    {
        Menu.SetActive(false);
    }
}
