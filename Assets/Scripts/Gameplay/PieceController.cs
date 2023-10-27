using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviourPunCallbacks
{
    [SerializeField] float timeToMoveDown;
    [SerializeField] GameObject[] piecePrefabs;
    [SerializeField] Vector2 spawPiecePosition;
    Piece lastPiece;
    [SerializeField] Vector2Int arenaSize;
    [SerializeField] bool[,] pieceMatrix;

    public static PieceController Instance;
    bool GetTileState(Vector2Int index) => pieceMatrix[index.x, index.y];
    void SetTileState(Vector2Int index, bool value) => pieceMatrix[index.x, index.y] = value;
    public void SetPiece(Vector2Int[] toSet)
    {
        foreach (var set in toSet)
            SetTileState(set, true);
    }
    public bool HasPiece(Vector2Int[] toCheck)
    {
        foreach (var pos in toCheck)
            if (GetTileState(pos)) return true;
        return false;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 1)
            StartMatch();
    }
    private void Awake()
    {

        pieceMatrix = new bool[arenaSize.x, arenaSize.y];

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    pieceMatrix[x, y] = true;

        Instance = this;
    }
   
    void PieceGravity()
    {
        Piece.currentPiece.MoveDown();
    }
    private void Update()
    {
        if (Piece.currentPiece)
        {
            if (Input.GetButtonDown("Horizontal"))
                Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));


            if (Input.GetButtonDown("MoveDown"))
                Piece.currentPiece.MoveDown();
        }
    }
    public void StartMatch()
    {
        NextPiece();
        InvokeRepeating(nameof(PieceGravity), timeToMoveDown, timeToMoveDown);
    }

    void NextPiece()
    {
        if (lastPiece) lastPiece.OnPieceStop -= NextPiece;


        Piece piece = PhotonNetwork.Instantiate(piecePrefabs[Random.Range(0, piecePrefabs.Length)].name, spawPiecePosition, Quaternion.identity).GetComponent<Piece>();

        piece.OnPieceStop += NextPiece;

        lastPiece = piece;
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(spawPiecePosition, .5f);

        for (int x = 0; x < arenaSize.x; x++)
            for (int y = 0; y < arenaSize.y; y++)
                if (y == 0 || x == 0 || x == (arenaSize.x - 1))
                    Gizmos.DrawWireCube(new(x, y), new(1, 1));
    }
}
