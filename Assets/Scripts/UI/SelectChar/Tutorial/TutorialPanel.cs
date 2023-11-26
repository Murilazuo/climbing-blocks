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
        if (FindObjectsOfType<PlayerPlatform>().Length > 0)
            ClosePanel();
    }

    void SetPlayer()
    {
        PlayerType playerType = MasterClientManager.Instance.playersType[PhotonNetwork.LocalPlayer];

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
    private void OnEnable()
    {
        MatchManager.OnStarCounter += ClosePanel;
        MasterClientManager.OnPlayerSetTeam += SetPlayer;
        MatchManager.OnPlayAgain += OpenPanel;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= ClosePanel;
        MasterClientManager.OnPlayerSetTeam -= SetPlayer;
        MatchManager.OnPlayAgain -= OpenPanel;
        
    }
}
