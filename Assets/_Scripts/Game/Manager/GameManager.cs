using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}
