using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject[] tutorials;


    private void Start()
    {
        tutorials[0].SetActive(false);
        tutorials[1].SetActive(false);
    }

    void SetPlayer(PlayerType playerType)
    {
        tutorials[0].SetActive(playerType == PlayerType.Piece);
        tutorials[1].SetActive(playerType == PlayerType.Character);
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }
    void OpenPanel()
    {
        panel.SetActive(true);
    }
    void PlayAgain()
    {
        OpenPanel();

        tutorials[0].SetActive(false);
        tutorials[1].SetActive(false);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += ClosePanel;
        SelectTeamController.OnClientSelectTeam += SetPlayer;
        MatchManager.OnPlayAgain += PlayAgain;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= ClosePanel;
        SelectTeamController.OnClientSelectTeam -= SetPlayer;
        MatchManager.OnPlayAgain -= PlayAgain;
        
    }
}
