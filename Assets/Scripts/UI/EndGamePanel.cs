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
        MatchManager.OnPlayerPieceWin += OnPieceWin;
        MatchManager.OnPlayerPlatformWin += OnPlatformWin;
    }
    private void OnDisable()
    {
        MatchManager.OnPlayerPieceWin -= OnPieceWin;
        MatchManager.OnPlayerPlatformWin -= OnPlatformWin;
    }
    void OnPieceWin()
    {
        ActivePanel();
        titleText.text = "Pieces Win";
    }
    void OnPlatformWin()
    {
        ActivePanel();
        titleText.text = "Platformers Win";
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
