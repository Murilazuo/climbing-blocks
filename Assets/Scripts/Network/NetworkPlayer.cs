using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour 
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
    public override void OnStartClient()
    {
        base.OnStartClient();
        print("Start Client");
        if(playerId == PIECE_CONTROLLER_ID)
        {
            render.enabled = false;
        }
    }
    public override void OnStartLocalPlayer()
    {
        playerId = NetworkManagerLobby.Instance.playerId;
        if(playerId  == PIECE_CONTROLLER_ID)
        {
            pieceControllerPlayer = this;
            transform.position = new(0, pieceControllerPositionY);
            rig.constraints = RigidbodyConstraints2D.FreezePositionY;
            col.enabled = false;
            trigger.enabled = false;
            InvokeRepeating(nameof(PieceGravity), 0, timeToMove);
        }
        else
        {
            rig.constraints = RigidbodyConstraints2D.None;
            transform.position = spawnPLatformPosition;
        }
  
    }
    void PieceGravity()
    {
        if (Piece.currentPiece)
        {
            Piece.currentPiece.MoveDown();
        }
    }
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (playerId == PIECE_CONTROLLER_ID)
        {
            if (Piece.currentPiece)
            {
                /*
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Piece.currentPiece.transform.localEulerAngles = new(0, 0, Piece.currentPiece.transform.localEulerAngles.z + 90);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Piece.currentPiece.transform.localEulerAngles = new(0, 0, Piece.currentPiece.transform.localEulerAngles.z - 90);
                }
                 */

                if (Input.GetButtonDown("Horizontal"))
                    Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));


                if (Input.GetButtonDown("MoveDown"))
                    Piece.currentPiece.MoveDown();
            }

            /*
            if (Input.GetButtonDown("Horizontal"))
            {
                Vector2 pos = rig.position;
                pos.x += Input.GetAxisRaw("Horizontal");
                if (pos.x < clampPosition && pos.x > -clampPosition)
                {
                    rig.MovePosition(pos);
                }
            }
             */
        }
        else
        {
            /*
            if (Input.GetButtonDown("Horizontal"))
            {
                Vector2 pos = transform.position;
                int dir = (int)Input.GetAxisRaw("Horizontal");
                pos.x += dir;
                transform.position = pos;
                Vector2Int[] posInt = new Vector2Int[1];
                posInt[0].x = (int)pos.x;
                posInt[0].y = (int)pos.y;

                if (MatchManager.Instance.HasPiece(posInt))
                {
                    print("Has Piece");
                    
                    pos.x -= dir;
                    transform.position = pos;
                }

            }
             */

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
        if (!isLocalPlayer) return;
        
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
