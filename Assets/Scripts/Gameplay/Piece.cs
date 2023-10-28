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

    public const int PIECE_STOP_EVENT = 18;
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
    int pieceId;
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

            object[] datas = new object[] { };
            PhotonNetwork.RaiseEvent(PIECE_STOP_EVENT, datas,RaiseEventOptions.Default,SendOptions.SendUnreliable);
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
        if(eventData.Code == PIECE_STOP_EVENT)
        {
            StopPiece();
        }
    }

}
