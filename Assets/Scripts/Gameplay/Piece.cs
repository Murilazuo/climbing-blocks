using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : NetworkBehaviour
{
    [SerializeField] GameObject piecePart;
    public Rigidbody2D rig;
    bool pieceIsStoped ;
    public System.Action OnPieceStop;
    public static Piece currentPiece;
    private void Awake()
    {
        print("Test");
        currentPiece = this;
        transform.position = new Vector2(NetworkPlayer.pieceControllerPlayer.transform.position.x, rig.transform.position.y);

        pieceIsStoped = true;
        LeanTween.delayedCall(1, () => pieceIsStoped = false);
    }
    private void Update()
    {
        if (pieceIsStoped) return;

        rig.velocity = new Vector2(NetworkPlayer.pieceControllerPlayer.rig.velocity.x, rig.velocity.y);
        

        if (rig.velocity.y >= 0 && !pieceIsStoped)
        {
            OnPieceStop?.Invoke();
            pieceIsStoped = true;
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
