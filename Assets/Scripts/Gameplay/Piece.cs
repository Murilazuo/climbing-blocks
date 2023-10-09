using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : NetworkBehaviour
{
    [System.Serializable]
    public struct PieceData {
        public Vector2[] piecePositions;
        public Color color;
    }

    [SerializeField] GameObject piecePart;
    public Rigidbody2D rig;
    bool pieceIsStoped ;
    public System.Action OnPieceStop;
    private void Awake()
    {
        transform.position = new Vector2(NetworkPlayer.pieceControllerPlayer.transform.position.x, rig.transform.position.y);
    }
    private void Update()
    {
        if (pieceIsStoped) return;

        transform.position = new Vector2(NetworkPlayer.pieceControllerPlayer.transform.position.x, rig.transform.position.y);

        if (rig.velocity.y > 0 && !pieceIsStoped)
        {
            OnPieceStop?.Invoke();
            pieceIsStoped = true;
        }
    }
    public void Init(PieceData data)
    {
        pieceIsStoped = true;
        foreach (var piecePosition in data.piecePositions)
        {
            GameObject part = Instantiate(piecePart, transform);
            NetworkServer.Spawn(part);
            part.transform.localPosition = piecePosition;

            part.GetComponent<SpriteRenderer>().color = data.color;
        }
        LeanTween.delayedCall(1,() => pieceIsStoped = false);
    }
    void PieceStop()
    {
        OnPieceStop?.Invoke();
        pieceIsStoped = false;
    }
}
