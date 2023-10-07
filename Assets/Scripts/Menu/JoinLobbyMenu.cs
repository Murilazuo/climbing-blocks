using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] NetworkManagerLobby lobbyManager;

    [SerializeField] GameObject landingPagePanel;
    [SerializeField] TMP_InputField ipAdressInputField;
    [SerializeField] Button joinButton;


    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnect;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnect;
    }
    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnect;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnect;
    }

    private void HandleClientDisconnect()
    {
        joinButton.interactable = false;
    }

    private void HandleClientConnect()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    public void JoinLobby()
    {
        string ipAdress = ipAdressInputField.text;

        lobbyManager.networkAddress = ipAdress;
        lobbyManager.StartClient();

        joinButton.interactable = false;
    }
}
