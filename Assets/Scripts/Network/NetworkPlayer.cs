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
    public Rigidbody2D rig;
    int playerId;

    public override void OnStartLocalPlayer()
    {
        playerId = NetworkManagerLobby.Instance.playerId;
        if(playerId  == 1)
        {
            pieceControllerPlayer = this;
            transform.position = new(0, pieceControllerPositionY);
            rig.constraints = RigidbodyConstraints2D.FreezePositionY;
            col.enabled = false;
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

        if (playerId == 1)
        {
            if (Piece.currentPiece)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Piece.currentPiece.transform.localEulerAngles = new(0, 0, Piece.currentPiece.transform.localEulerAngles.z + 90);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Piece.currentPiece.transform.localEulerAngles = new(0, 0, Piece.currentPiece.transform.localEulerAngles.z - 90);
                }

                if (Input.GetButtonDown("Horizontal"))
                {
                    Piece.currentPiece.MoveX((int)Input.GetAxisRaw("Horizontal"));
                }


                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                        Piece.currentPiece.MoveDown();
                }
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

            if (Input.GetButtonDown("Jump"))
            {
                if (Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask))
                    rig.AddForce(jumpForce * Vector2.up);
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        if(playerId != 1)
        {
            /*
            Vector3 velocity = rig.velocity;
            velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
            rig.velocity = velocity;
            */
        }
    }
}
