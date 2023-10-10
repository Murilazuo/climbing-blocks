using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : NetworkBehaviour
{
    static MatchManager instance;
    public static MatchManager Instance
    {
        get
        {
            if (!instance) instance = FindObjectOfType<MatchManager>();
            return instance;
        }
    }
    public static System.Action<Piece> OnSpawnPiece;
    [SerializeField] GameObject[] piecePrefabs;
    [SerializeField] Vector2 spawPiecePosition;
    public Vector2 playerPlatformSpawnPosition;
    Piece lastPiece;

    private void Awake()
    {
        instance = this;
    }
    public void StartMatch()
    {
        NextPiece();
    }

    void NextPiece()
    {
        if(lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)], spawPiecePosition,Quaternion.identity).GetComponent<Piece>();

        NetworkServer.Spawn(piece.gameObject);

        piece.OnPieceStop += NextPiece;

        OnSpawnPiece?.Invoke(piece);

        lastPiece = piece;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);
    }
}
