using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] TMP_Text buttonText;
    private void Start()
    {
        buttonText.text = "Not Ready";
        isReady = false;
    }
    bool isReady;
    public void SetReady()
    {
        isReady = !isReady;

        buttonText.text = isReady ? "Ready" : "Not Ready";
        buttonText.color = isReady ? Color.green : Color.red;

        MatchManager.Instance.SerIsReady(isReady, PhotonNetwork.LocalPlayer.ActorNumber);
    }
}
