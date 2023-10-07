using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [System.Serializable]
    public struct PieceData {
        public Vector2[] piecePositions;
        public Color color;
    }

    [SerializeField] GameObject piecePart;
    public Rigidbody2D rig;
    bool pieceIsStoped;
    public System.Action OnPieceStop;

    private void Update()
    {
        if(rig.velocity.y > 0 && !pieceIsStoped)
        {
            PieceStop();
        }
    }
    public void Init(PieceData data)
    {
        pieceIsStoped = true;
        foreach (var piecePosition in data.piecePositions)
        {
            GameObject part = Instantiate(piecePart, transform);
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
