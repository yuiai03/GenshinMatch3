using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviourPunCallbacks
{
    public static Temp Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    [PunRPC]
    public void StartMultiplayerGame()
    {
        LoadManager.Instance.TransitionLevel(SceneType.Multiplayer);
    }
}
