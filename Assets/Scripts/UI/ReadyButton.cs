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
        ResetButton();
    }
    bool isReady;
    public void SetReady()
    {
        isReady = !isReady;

        buttonText.text = isReady ? "Ready" : "Not Ready";
        buttonText.color = isReady ? Color.green : Color.red;

        MatchManager.Instance.SerIsReady(isReady);
    }
    void ResetButton()
    {
        buttonText.text = "Not Ready";
        isReady = false;
        buttonText.color = Color.red;

    }

    private void OnDisable()
    {
        ResetButton();
    }
}
