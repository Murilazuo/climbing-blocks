using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] GameObject panel;
    [SerializeField] float timeToEnable;
    [SerializeField] float alphCanvasTime;
    [SerializeField] float showPosition, hidePosition;
    [SerializeField] float timeToMove;

    public static System.Action OnOpenEndGamePanel;

    public static EndGamePanel instance;

    private void Awake()
    {
        instance = this;

        panel.transform.localPosition = new(0, hidePosition, 0);
    }
    private void OnEnable()
    {
        MatchManager.OnEndGame += OnEndGame;
        MatchManager.OnPlayAgain += OnPlayAgain;
    }
    private void OnDisable()
    {
        MatchManager.OnEndGame -= OnEndGame;
        MatchManager.OnPlayAgain -= OnPlayAgain;
    }

    private void OnPlayAgain()
    {
        endGame = false;
        DisablePanel();
    }

    bool endGame;
    void OnEndGame(int eventCode)
    {
        if (endGame) return;

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
    }
    void ActivePanel()
    {
        LeanTween.moveLocalY(panel, showPosition, timeToMove).setOnComplete(() => OnOpenEndGamePanel?.Invoke());
    }
    public void DisablePanel()
    {
        LeanTween.moveLocalY(panel, hidePosition, timeToMove);
    }
}
