using Hierarchy2;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] GameObject playerPlatform, playerPiece, networkPlayer;
    [SerializeField] Transform playerNetworkPlayer;
    [SerializeField] Vector3 playerPosition;


    private void Awake()
    {
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            Instantiate(playerPiece, Vector3.zero, Quaternion.identity);
        else
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);

        if (Application.isEditor)
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(playerPosition, .5f);
    }
}
