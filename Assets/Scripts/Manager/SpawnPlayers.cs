using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] GameObject playerPlatform, playerPiece;
    [SerializeField] Vector3 playerPosition;
    void Start()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            PhotonNetwork.Instantiate(playerPiece.name,Vector3.zero,Quaternion.identity);
        else
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(playerPosition, .5f);
    }
}
