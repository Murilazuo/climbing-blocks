using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour 
{
    [Header("Move")]
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Rigidbody2D rig;
    [SerializeField] Vector2 spawnPLatformPosition;
    [SerializeField] float pieceControllerPositionY;
    [SerializeField] Collider2D col;
    public static NetworkPlayer pieceControllerPlayer;
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

        if (Input.GetButtonDown("Jump"))
        {
            if (Physics2D.Raycast(rig.position, Vector2.down, groundCheckDistance, layerMask))
                rig.AddForce(jumpForce * Vector2.up);
        }
    }
    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        Vector3 velocity = rig.velocity;
        velocity.x = speed * Time.deltaTime * Input.GetAxis("Horizontal");
        rig.velocity = velocity;
    }
}
