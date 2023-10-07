using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] NetworkManagerLobby lobby;

    [SerializeField] GameObject loadingPanel;
    private void Start()
    {
        Screen.fullScreen = false;
    }
    public void HostLobby() 
    {
        lobby.StartHost();

        loadingPanel.SetActive(false);
    }
}
