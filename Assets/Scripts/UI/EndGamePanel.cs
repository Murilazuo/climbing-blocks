using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] CanvasGroup canvasGroup;

    public static EndGamePanel instance;
    private void Awake()
    {
        instance = this; 
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEndGame;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEndGame;
    }
    bool endGame;
    void OnEndGame(EventData eventData)
    {
        if (eventData.Code == NetworkEventSystem.PLATFORM_WIN_GAME_EVENT || eventData.Code == NetworkEventSystem.PIECE_WIN_GAME_EVENT && !endGame)
        {
            endGame = true;
            ActivePanel();

            titleText.text = eventData.Code == NetworkEventSystem.PLATFORM_WIN_GAME_EVENT ? "Platformers Win" : "Pieces Win";
        }
    }
    void ActivePanel()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void DisablePanel()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
