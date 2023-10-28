using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatform : MonoBehaviour
{
    [SerializeField] PhotonView view;
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    [SerializeField] float groundCheckDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Vector2 spawnPlatformPosition;
    [SerializeField] Rigidbody2D rig;
    void Update()
    {
        if (!view.IsMine) return;
        
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
    private void FixedUpdate()
    {
        if (!view.IsMine) return;

        Vector3 velocity = rig.velocity;
        velocity.x = speed * Input.GetAxis("Horizontal");
        rig.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag(Piece.MOVE_PIECE_TAG))
        {
            MatchManager.Instance.PlayerPiecesWin();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            MatchManager.Instance.PlayerPlatformWin();
        }
    }

}
