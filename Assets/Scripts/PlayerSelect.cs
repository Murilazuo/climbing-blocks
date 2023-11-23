using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    [SerializeField] PhotonView view;
    public bool IsMine { get => view.IsMine; }
    void OnStartGame()
    {
        MatchManager.Instance.SelectPlayerType(PlayerType.None);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += OnStartGame;
    }
    private void OnDisable()
    {
        MatchManager.OnStarCounter -= OnStartGame;
    }
}
