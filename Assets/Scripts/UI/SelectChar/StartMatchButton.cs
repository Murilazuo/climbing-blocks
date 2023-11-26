using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMatchButton : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    bool isMaster;
    void UpdateMasterClient(bool isMaster)
    {
        this.isMaster = isMaster;
        canvasGroup.alpha = isMaster ? 1 : 0;
        canvasGroup.blocksRaycasts = isMaster;
    }
    void OnSetPlayerReady(bool allPlayerReady)
    {
        canvasGroup.interactable = allPlayerReady;
    }
    public void StartMatch()
    {
        MasterClientManager.Instance.StartMatch();
    }
    private void OnEnable()
    {
        MasterClientManager.OnSetMasterClient += UpdateMasterClient;
        MasterClientManager.OnPlayersReady += OnSetPlayerReady;
    }
    private void OnDisable()
    {
        MasterClientManager.OnSetMasterClient -= UpdateMasterClient;
        MasterClientManager.OnPlayersReady -= OnSetPlayerReady;
    }
}
