using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour 
{
    [Header("Platform")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector2 spawnPLatformPosition;

    [Header("Piece Controller")]
    [SerializeField] float clampPosition; 
    public static NetworkPlayer pieceControllerPlayer;
    [SerializeField] float pieceControllerPositionY;
    [SerializeField] float pieceControllerMult;
    [SerializeField] float timeToMove;
    
    [Header("Components")]
    [SerializeField] Collider2D col;
    [SerializeField] Collider2D trigger;
    [SerializeField] SpriteRenderer render;
    public Rigidbody2D rig;
    int playerId;

    const int PIECE_CONTROLLER_ID = 1;
   
    void PieceGravity()
    {
        if (Piece.currentPiece)
        {
            Piece.currentPiece.MoveDown();
        }
    }
    private void Update()
    {
        if (playerId == PIECE_CONTROLLER_ID)
        {
            if (Piece.currentPiece)
            {
                if (Input.GetButtonDown("Horizontal"))
                    Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));


                if (Input.GetButtonDown("MoveDown"))
                    Piece.currentPiece.MoveDown();
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask))
                {
                    Vector2 velocity = rig.velocity;
                    velocity.y = 0;
                    rig.velocity = velocity;
                    rig.AddForce(jumpForce * Vector2.up);
                }
            }
        }
    }
    private void FixedUpdate()
    {
        if(playerId != PIECE_CONTROLLER_ID)
        {
            Vector3 velocity = rig.velocity;
            velocity.x = speed * Input.GetAxis("Horizontal");
            rig.velocity = velocity;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
            switch (collision.tag)
            {
                case "End":
                    MatchManager.Instance.PlayerPlatformWin();
                    break;
                case Piece.MOVE_PIECE_TAG:
                    MatchManager.Instance.PlayerPiecesWin();
                    break;
            }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
}
