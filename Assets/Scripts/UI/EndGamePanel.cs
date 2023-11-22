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
    [SerializeField] float timeToEnable;
    [SerializeField] float alphCanvasTime;
    public static System.Action OnOpenEndGaemPanel;

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
        if (endGame) return;

        print(eventCode);
        print(NetworkEventSystem.IsEndGame((byte)eventCode));
        if (NetworkEventSystem.IsEndGame((byte)eventCode))
        { 
            LeanTween.delayedCall(timeToEnable, () => OpenPanel(eventCode));
        }
    }
    void OpenPanel(int eventCode)
    {
        endGame = true;
        ActivePanel();

        titleText.text = NetworkEventSystem.PlatfomrWin((byte)eventCode) ? "Platformers Win" : "Pieces Win";
        OnOpenEndGaemPanel?.Invoke();
    }
    void ActivePanel()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, alphCanvasTime);
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
