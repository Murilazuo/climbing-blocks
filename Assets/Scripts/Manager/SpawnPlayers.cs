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
    public int playerId;
    public static System.Action<int> OnSpawnPlayer;

    public static SpawnPlayers Instance;

    private void Awake()
    {
        Instance = this;
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        playerId = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerId == 1)
            Instantiate(playerPiece, Vector3.zero, Quaternion.identity);
        else if(playerId == 2)
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);

        if (Application.isEditor)
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);

        OnSpawnPlayer?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(playerPosition, .5f);
    }
}
