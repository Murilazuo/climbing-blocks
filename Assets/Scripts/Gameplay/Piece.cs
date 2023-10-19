using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : NetworkBehaviour
{
    [SerializeField] GameObject piecePart;
    Vector2Int[] PartPosition
    {
        get
        {
            Vector2Int[] result = new Vector2Int[transform.childCount];
            int i = 0;
            foreach (Transform t in transform)
            {
                result[i].x = (int)t.position.x;
                result[i].y = (int)t.position.y ;
                i++;
            }
            return result;
        }
    }
    bool pieceIsStoped ;
    public System.Action OnPieceStop;
    public static Piece currentPiece;
    private void Awake()
    {
        currentPiece = this;
        pieceIsStoped = true;
        LeanTween.delayedCall(1, () => pieceIsStoped = false);

    }
    private void Update()
    {
        if (pieceIsStoped) return;

        /*
        rig.transform.position = new Vector2((int)NetworkPlayer.pieceControllerPlayer.rig.transform.position.x, rig.transform.position.y);

        Vector2 pos = rig.position;
        pos.x = NetworkPlayer.pieceControllerPlayer.rig.position.x;
        
        transform.position = pos;

        if (rig.velocity.y >= 0 && !pieceIsStoped)
        {
            OnPieceStop?.Invoke();
            pieceIsStoped = true;
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        */
    }
    
    public void MoveX(int moveX)
    {
        Vector3 pos = transform.position;
        pos.x += moveX;
        transform.position = pos;
        if (MatchManager.Instance.HasPiece(PartPosition))
        {
            print("Has Piece");
            foreach (var piece in PartPosition)
            {
                print($"x {piece.x},y {piece.y} ");
            }
            pos.x -= moveX;
            transform.position = pos;
        }
    }

    public void MoveDown()
    {
        Vector3 pos = transform.position;
        pos.y -= 1;
        transform.position = pos;
        if (MatchManager.Instance.HasPiece(PartPosition))
        {
            print("Has Piece");
           
            pos.y += 1;
            transform.position = pos;
            currentPiece = null;
            OnPieceStop?.Invoke();
            MatchManager.Instance.SetPiece(PartPosition);
        }
    }

}
