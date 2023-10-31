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
        MatchManager.OnEndGame += OnEndGame;

    }
    private void OnDisable()
    {
        MatchManager.OnEndGame -= OnEndGame;
    }
    bool endGame;
    void OnEndGame(int eventCode)
    {
        if (eventCode == NetworkEventSystem.PLATFORM_WIN_GAME_EVENT || eventCode == NetworkEventSystem.PIECE_WIN_GAME_EVENT && !endGame)
        {
            endGame = true;
            ActivePanel();

            titleText.text = eventCode == NetworkEventSystem.PLATFORM_WIN_GAME_EVENT ? "Platformers Win" : "Pieces Win";
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
