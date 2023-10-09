using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;
    public static System.Action<Piece> OnSpawnPiece;
    [SerializeField] GameObject piecePrefab;
    [SerializeField] Piece.PieceData[] pieceDatas;
    [SerializeField] Vector2 spawPiecePosition;
    public Vector2 playerPlatformSpawnPosition;
    [SerializeField] GameObject player;
    Piece lastPiece;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }
    public void StartMatch()
    {
        NextPiece();
    }

    void NextPiece()
    {
        if(lastPiece) lastPiece.OnPieceStop -= NextPiece;

        Piece piece = Instantiate(piecePrefab,spawPiecePosition,Quaternion.identity).GetComponent<Piece>();
        piece.Init(pieceDatas[Random.Range(0, pieceDatas.Length)]);

        piece.OnPieceStop += NextPiece;

        OnSpawnPiece?.Invoke(piece);

        lastPiece = piece;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);
    }
}
