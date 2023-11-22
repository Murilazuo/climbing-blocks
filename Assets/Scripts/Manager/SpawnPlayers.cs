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

    public static SpawnPlayers Instance;
    PlayerType playerType;   
    void SetPlayerType(PlayerType playerType)
    {
        this.playerType = playerType;
    }
    private void Awake()
    {
        Instance = this;
    }
    public void SpawnPlayer()
    {
        if (playerType == PlayerType.Piece)
            Instantiate(playerPiece, Vector3.zero, Quaternion.identity);
        else if (playerType == PlayerType.Character)
            PhotonNetwork.Instantiate(playerPlatform.name, playerPosition, Quaternion.identity);
    }
    private void OnEnable()
    {
        MatchManager.OnStarCounter += SpawnPlayer;
        MatchManager.OnSelectPlayerType += SetPlayerType;
    }
    private void OnDisable()
    {
        MatchManager.OnSelectPlayerType -= SetPlayerType;
        MatchManager.OnStarCounter -= SpawnPlayer;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(playerPosition, .5f);
    }
}
