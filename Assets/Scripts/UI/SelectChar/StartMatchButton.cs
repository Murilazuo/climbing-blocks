using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchButton : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    bool isMaster;

    private void Start()
    {
        OnSetPlayerReady(false);
    }
    void UpdateMasterClient(bool isMaster)
    {
        this.isMaster = isMaster;
        canvasGroup.alpha = isMaster ? 1 : 0;
        canvasGroup.blocksRaycasts = isMaster;
    }
    void OnSetPlayerReady(bool allPlayerReady)
    {
        canvasGroup.interactable = allPlayerReady || (Application.isEditor && PhotonNetwork.CurrentRoom.PlayerCount == 1);
    }
    public void StartMatch()
    {
        MasterClientManager.Instance.StartMatch();
    }
    void PlayAgain()
    {
        canvasGroup.interactable = false;
    }
    void OnSwitchMasterClient(Player newMasterClient)
    {
        if(newMasterClient.IsLocal)
        {

        }
    }
    private void OnEnable()
    {
        MasterClientManager.OnSetMasterClient += UpdateMasterClient;
        MasterClientManager.OnPlayersReady += OnSetPlayerReady;
        MatchManager.OnPlayAgain += PlayAgain;
    }
    private void OnDisable()
    {
        MasterClientManager.OnSetMasterClient -= UpdateMasterClient;
        MasterClientManager.OnPlayersReady -= OnSetPlayerReady;
        MatchManager.OnPlayAgain -= PlayAgain;
    }
}
