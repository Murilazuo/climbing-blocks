using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReady : MonoBehaviour
{
    [SerializeField] PhotonView view;
    public bool IsMine { get => view.IsMine; }
    void StartCounter()
    {
        if(IsMine)
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
