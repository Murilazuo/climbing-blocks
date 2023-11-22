using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReady : MonoBehaviour
{
    [SerializeField] PhotonView view;

    void StartCounter()
    {
        if(view.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += StartCounter;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= StartCounter;
    }

}
