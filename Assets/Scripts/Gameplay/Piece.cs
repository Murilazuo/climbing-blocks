using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviourPun
{
    public const string STOPED_PIECE_TAG = "StopedPiece";
    public const string MOVE_PIECE_TAG = "MovePiece";
    [SerializeField] GameObject piecePart;

    [SerializeField] float endPositionY;

    Vector2Int[] PartPosition
    {
        get
        {
            Vector2Int[] result = new Vector2Int[transform.childCount];
            int i = 0;
            foreach (Transform t in transform)
            {
                result[i].x = (int)t.position.x;
                result[i].y = (int)t.position.y;
                i++;
            }
            return result;
        }
    }
    bool pieceIsStoped;
    public System.Action OnPieceStop;
    public static Piece currentPiece;

    static int pieceCount = 0;
    public int pieceId;
    private void Awake()
    {
        currentPiece = this;
        pieceIsStoped = true;
        LeanTween.delayedCall(1, () => pieceIsStoped = false);
        tag = MOVE_PIECE_TAG;
        pieceCount++;
        pieceId = pieceCount;
    }

    public void MoveX(int moveX)
    {
        Vector3 pos = transform.position;
        pos.x += moveX;
        transform.position = pos;
        if (PieceController.Instance.HasPiece(PartPosition))
        {
            print("Has Piece");
            pos.x -= moveX;
            transform.position = pos;
        }
    }

    public void MoveDown()
    {
        Vector3 pos = transform.position;
        pos.y -= 1;
        transform.position = pos;
        if (PieceController.Instance.HasPiece(PartPosition))
        {
            print("Has Piece");
            pos.y += 1;
            transform.position = pos;
            currentPiece = null;
            OnPieceStop?.Invoke();
            PieceController.Instance.SetPiece(PartPosition);
            StopPiece();

            NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_STOP_EVENT);
        }
    }
    void StopPiece()
    {
        if (pieceIsStoped) return;
        pieceIsStoped = true;
        foreach (Transform t in transform)
        {
            t.gameObject.tag = STOPED_PIECE_TAG;

            if (endPositionY <= t.transform.position.y)
                MatchManager.Instance.PlayerPlatformWin();
        }
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkCliente_RisedEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkCliente_RisedEvent;
    }
    void NetworkCliente_RisedEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case NetworkEventSystem.PIECE_STOP_EVENT:
                StopPiece();
                break;
            case NetworkEventSystem.PIECE_DESTROY_EVENT:
                object[] data = (object[])eventData.CustomData;
                Vector3 piecePosition = (Vector3)data[0];

                foreach (Transform t in transform)
                {
                    if (V3ToV3Int(t.position) == V3ToV3Int(piecePosition))
                        DestroyImmediate(t.gameObject);
                }
                break;
        }
    }

    Vector2Int V3ToV3Int(Vector3 value) => new((int)value.x, (int)value.y);
}
