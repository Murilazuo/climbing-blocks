using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject[] tutorials;

    private void Start()
    {
        if (FindObjectsOfType<PlayerPlatform>().Length > 0)
            ClosePanel();
    }

    void SetPlayer(PlayerType playerType)
    {
        tutorials[0].SetActive((int)playerType == 0);
        tutorials[1].SetActive((int)playerType == 1);
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }
    void OpenPanel()
    {
        panel.SetActive(true);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += ClosePanel;
        MatchManager.OnSelectPlayerType += SetPlayer;
        MatchManager.OnPlayAgain += OpenPanel;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= ClosePanel;
        MatchManager.OnSelectPlayerType -= SetPlayer;
        MatchManager.OnPlayAgain -= OpenPanel;
        
    }
}
