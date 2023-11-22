using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviourPun
{
    [System.Serializable]
    struct PieceData
    {
        public Color color;
        public Vector2[] positions;
    }
    [SerializeField] PieceData[] blockPositions;

    public const string STOPED_PIECE_TAG = "StopedPiece";
    public const string MOVE_PIECE_TAG = "MovePiece";

    [SerializeField] GameObject piecePart;

    [SerializeField] float endPositionY;

    public static System.Action OnStopPiece;

    bool fixInGrid;

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
    public static Piece lastPieceStoped;

    static int pieceCount = 0;
    public int pieceId;

    [SerializeField] PhotonView view;
    [SerializeField] GameObject blockPrefab;
    private void Awake()
    {
        currentPiece = this;
        pieceIsStoped = true;
        LeanTween.delayedCall(1, () => pieceIsStoped = false);
        tag = MOVE_PIECE_TAG;
        pieceCount++;
        pieceId = pieceCount;

        if (view.IsMine)
        {
            view.RPC(nameof(SetBlock), RpcTarget.All, Random.Range(0, blockPositions.Length));
        }
    }
    [PunRPC]
    void SetBlock(int dataId)
    {
        int id = pieceId * 4;
        PieceData data = blockPositions[dataId];
        foreach (var pos in data.positions)
        {
            GameObject obj = Instantiate(blockPrefab, transform);
            obj.transform.localPosition = pos;
            obj.GetComponent<Block>().Init(data.color);
            id++;
        }
    }

    public void MoveX(int moveX)
    {
        Vector3 pos = transform.position;
        pos.x += moveX;
        transform.position = pos;
        if (PieceController.Instance.HasPiece(PartPosition))
        {
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
            pos.y += 1;
            transform.position = pos;
            CallStopPiece();
        }
        else
        {
            pos.y -= 1;
            transform.position = pos;
            if (PieceController.Instance.HasPiece(PartPosition))
            {
                pos.y += 1;
                transform.position = pos;
                CallStopPiece();
            }
            else
            {
                pos.y += 1;
                transform.position = pos;
            }
        }
    }
    void CallStopPiece()
    {
        currentPiece = null;
        OnPieceStop?.Invoke();
        PieceController.Instance.SetPiece(PartPosition);
        StopPiece();

        NetworkEventSystem.CallEvent(NetworkEventSystem.PIECE_STOP_EVENT);
    }
    void StopPiece()
    {
        if (pieceIsStoped) return;
        pieceIsStoped = true;
        foreach (Transform t in transform)
        {
            LeanTween.delayedCall(.1f, () => t.gameObject.tag = STOPED_PIECE_TAG);

            if (endPositionY <= t.transform.position.y)
                MatchManager.Instance.PieceReachTop();
        }
        lastPieceStoped = this;
        OnStopPiece?.Invoke();    
    }
    void OnOpenEndGamePanel()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += NetworkCliente_RisedEvent;
        MatchManager.OnDestroyBlock += DestroyBlock;
        EndGamePanel.OnOpenEndGaemPanel += OnOpenEndGamePanel;
    }
    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
        PhotonNetwork.NetworkingClient.EventReceived -= NetworkCliente_RisedEvent;
        MatchManager.OnDestroyBlock -= DestroyBlock;
        EndGamePanel.OnOpenEndGaemPanel -= OnOpenEndGamePanel;
    }
    void NetworkCliente_RisedEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case NetworkEventSystem.PIECE_STOP_EVENT:
                StopPiece();
                break;
        }
    }
    void DestroyBlock(Vector2 pos)
    {
        foreach(Transform t in transform)
        {
            //print(Vector3.Distance(pos, t.position));
            if (Vector3.Distance(pos, t.position) < .3f)
            {
                t.gameObject.GetComponent<Block>().Hit();
            }
        }
    }
    Vector2Int V3ToV3Int(Vector3 value) => new((int)value.x, (int)value.y);
}
