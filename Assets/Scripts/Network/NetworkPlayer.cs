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
        }
        else
        {
            rig.constraints = RigidbodyConstraints2D.None;
            transform.position = spawnPLatformPosition;
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
            }
        }
        else
        {
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
        Vector3 velocity = rig.velocity;
        velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
        rig.velocity = velocity;
    }
    private void LateUpdate()
    {
        if (!isLocalPlayer) return;

        rig.position = new(Mathf.Clamp(transform.position.x,-clampPosition,clampPosition),rig.position.y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new(0, pieceControllerPositionY),new(clampPosition*2,.3f));
    }
}
